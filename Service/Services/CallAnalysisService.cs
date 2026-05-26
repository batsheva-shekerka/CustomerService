using Azure;
using Azure.AI.TextAnalytics;
using DataContext;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Transcription;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Microsoft.Extensions.Configuration;
using Service.Services;
using Service.specialservice;


public class CallAnalysisService
{
    private readonly string _speechKey;
    private readonly string _region = "westeurope";
    private readonly CustomerServiceContext _context;
    private readonly TextAnalyticsClient _textClient;
    private readonly IConfiguration _configuration;

    
    public CallAnalysisService(CustomerServiceContext context, TextAnalyticsClient textClient, IConfiguration configuration)
    {
        _context = context;
        _textClient = textClient;
        _configuration = configuration;
        _speechKey = _configuration["AzureServices:SpeechKey"];

    }

    public async Task ProcessFullCallChain(string filePath, int operatorId)
    {
        // 1. שליפת נתוני נציגה וחברה
        var op = await _context.Set<Operator>()
            .Include(o => o.Company)
            .FirstOrDefaultAsync(o => o.OperatorId == operatorId);
        if (op == null) return;

        // 2. תמלול השיחה (Azure Speech SDK)
        var segments = await TranscribeAudio(filePath);
        if (segments == null || !segments.Any()) return;

        // 3. recognice the spokers
        string agentId = IdentifyAgent(segments, op.Company?.IntroPhrase ?? "");
        //if its not founed took the first to be the operator
        var customerSegmentObj = segments.FirstOrDefault(s => s.SpeakerId != agentId);
        string customerId = customerSegmentObj?.SpeakerId ?? "Unknown-Customer";
        // 4. take the volume (NAudio)
        var agentWav = Path.Combine(Path.GetTempPath(), $"agent_{Guid.NewGuid()}.wav");
        var customerWav = Path.Combine(Path.GetTempPath(), $"customer_{Guid.NewGuid()}.wav");

        // ניתוח נציגה (תמיד קיים)
        AudioSplitter.CreateSpeakerWav(filePath, segments, agentId, agentWav);
        var agentVol = VolumeAnalyzer.Analyze(agentWav);

        // הגדרת ערכי ברירת מחדל ללקוח
        var customerVol = (avg: 0.0, peak: 0.0);
        var customerSegments = segments.Where(s => s.SpeakerId == customerId).ToList();

        // הרצה של הפיצול רק אם הדובר ידוע ויש לו קטעים
        if (customerId != "Unknown-Customer" && customerSegments.Any())
        {
            try
            {
                AudioSplitter.CreateSpeakerWav(filePath, segments, customerId, customerWav);
                customerVol = VolumeAnalyzer.Analyze(customerWav);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Non-Critical Error] Could not analyze customer audio: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"[Info] Skipping customer audio analysis - Speaker is Unknown or has no segments.");
        }

        // 5. איסוף טקסטים
        var agentSegments = segments.Where(s => s.SpeakerId == agentId).ToList();

        //to cast from list to string
        string agentText = string.Join(" ", agentSegments.Select(s => s.Text));
        string customerText = string.Join(" ", customerSegments.Select(s => s.Text));

        // 6. הכנת משתנים זמניים לניתוח רגש (כדי למנוע שגיאות קומפילציה)
        string opSentiment = "Neutral";
        double custSentimentStart = 0;
        double custSentimentEnd = 0;

        // ניתוח רגש נציגה
        if (!string.IsNullOrWhiteSpace(agentText))
        {
            var result = await _textClient.AnalyzeSentimentAsync(agentText);
            opSentiment = result.Value.Sentiment.ToString();
        }
       
        //how many sentents the cutomer have
        int totalCustomerSegments = customerSegments.Count;
        int segmentsToTake = 3; // the 3 first sententens of the customer

        if (totalCustomerSegments < 6)//if the total sentenses less than 6
        {
            //we will take only half of the sentenses
            segmentsToTake = Math.Max(1, totalCustomerSegments / 2);
        }

        // select the sentenses by the culculated sum
        var customerStartText = string.Join(" ", customerSegments.Take(segmentsToTake).Select(s => s.Text));
        var customerEndText = string.Join(" ", customerSegments.TakeLast(segmentsToTake).Select(s => s.Text));
        // ------------------------------------------------

        //calculate the sentiment of the customer
        if (!string.IsNullOrWhiteSpace(customerStartText))
        {
            //start sentiment
            var resStart = await _textClient.AnalyzeSentimentAsync(customerStartText);
            custSentimentStart = resStart.Value.ConfidenceScores.Positive - resStart.Value.ConfidenceScores.Negative;
        }

        if (!string.IsNullOrWhiteSpace(customerEndText))
        {
            //end sentiment
            var resEnd = await _textClient.AnalyzeSentimentAsync(customerEndText);
            custSentimentEnd = resEnd.Value.ConfidenceScores.Positive - resEnd.Value.ConfidenceScores.Negative;
        }

        // 7.create call object
        var newCall = new Call
        {
            CompanyId = op.CompanyId,
            OperatorId = operatorId,
            CallDate = DateTime.Now,
            Duration = segments.Max(s => s.Offset + s.Duration),

            // operator details 
            OperatorTranscript = agentText,
            OperatorSentiment = opSentiment,
            OperatorMaxVolume = agentVol.peak,
            OperatorWordsPerSecond = CalculateWPS(agentSegments, agentText),

            //  customer details
            CustomerTranscript = customerText,
            CustomerMaxVolume = customerVol.peak,
            CustomerSentimentStart = custSentimentStart,
            CustomerSentimentEnd = custSentimentEnd
        };

        // 8. create Score object
        newCall.Score = CalculateCallScore(newCall);

        // 9.save to the -DB
        _context.Set<Call>().Add(newCall);
        await _context.SaveChangesAsync();

        // clean the temp files
        if (File.Exists(agentWav)) File.Delete(agentWav);
        if (File.Exists(customerWav)) File.Delete(customerWav);
    }


    // Calculate the scire of the call
    private Score CalculateCallScore(Call call)
    {
        List<string> notesBuilder = new List<string>();
        var score = new Score();
        //only 20%
        //by the tone's operator
        if (call.OperatorMaxVolume > 1)
            call.OperatorMaxVolume = 1;
        double toneBase =(double)(100 - call.OperatorMaxVolume*100);
        
        if (toneBase < 40)
        if (toneBase < 20)
            notesBuilder.Add("טון דיבור גבוה מידי.");
        if (call.OperatorSentiment == "Negative")
        {
            toneBase -= 50;
            if(toneBase < 0) toneBase = 0;
            notesBuilder.Add("פנייה בצורה לא נעימה ללקוח.");
        }
        score.OperatorToneScore = toneBase;

        //70%
        // 2. if the sentiment of the customer improved during the call
        double emotionalShift = (call.CustomerSentimentEnd ?? 0) - (call.CustomerSentimentStart ?? 0);
        if (emotionalShift > 5)
        {
            notesBuilder.Add("ניהול שיחה גרוע!!.");
        }
        var operatorSentiment = call.OperatorSentiment;
        emotionalShift = 95 - (emotionalShift * 100);
        if (emotionalShift >100)
        {
            emotionalShift = 100;
            notesBuilder.Add("ניהול שיחה מעולה!!.");
        }
        
        score.ConflictResolutionScore = emotionalShift;

        //10%
        // 3. profetionl by the words per second and durition of the call
        double fast = 100 - (double)(call.OperatorWordsPerSecond * 10);
        if (call.OperatorWordsPerSecond > 3|| call.OperatorWordsPerSecond < 0.5) notesBuilder.Add("שפה לא ברורה ונעימה ללקוח.");
        bool due = call.Duration <= TimeSpan.FromMinutes(3);
        bool due10 = call.Duration <= TimeSpan.FromMinutes(10);
        if (!due10) notesBuilder.Add("משך שיחה ארוך מידי.");
        score.ProfessionalismScore = fast+((due)?fast/100*10:0);

        call.GeneralNotes = string.Join(" | ", notesBuilder);
        if (call.GeneralNotes == "") { call.GeneralNotes = "את/ה היית נהדר/ת בשיחה"; };

        // 4. calculate the final score
        score.OverallScore = (score.OperatorToneScore*0.2 + score.ConflictResolutionScore*0.7 + score.ProfessionalismScore*0.1);
        double min = Math.Min((double)score.OperatorToneScore, (double)score.ConflictResolutionScore);
        min = Math.Min(min, (double)score.ProfessionalismScore);
        if (min == (double)score.ProfessionalismScore)
        {
            score.ImprovementTips = ImprovementTipsEntity.ConflictResolution;
        }
        else
        {
            if (min == (double)score.OperatorToneScore)
            {
                score.ImprovementTips = ImprovementTipsEntity.ToneAndEmpathy;
            }
            else
            {
                score.ImprovementTips = ImprovementTipsEntity.TechnicalKnowledge;
            }
        }

        return score;
    }

    //calculate the words per second
    private double CalculateWPS(List<Segment> segments, string text)
    {
        if (!segments.Any() || string.IsNullOrWhiteSpace(text)) return 0;
        var totalTime = segments.Max(s => s.Offset + s.Duration) - segments.Min(s => s.Offset);
        var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return totalTime.TotalSeconds > 0 ? (wordCount / totalTime.TotalSeconds) : 0;
    }
    //private double CalculateIndividualScore(string type, DocumentSentiment full, DocumentSentiment lastWords, double wps, double peak, out string notes)
    //{
    //    double s = 100;
    //    var n = new List<string>();

    //    if (full == null)
    //    {
    //        notes = "לא זוהה דיבור בשיחה מצד משתתף זה.";
    //        return 0;
    //    }

    //    // דוגמה ללוגיקה: אם הסנטימנט הכללי שלילי
    //    if (full.Sentiment == TextSentiment.Negative) { s -= 30; n.Add("טון כללי שלילי"); }

    //    // אם הסיום (20 מילים) שלילי - משקל גבוה
    //    if (lastWords.Sentiment == TextSentiment.Negative) { s -= 20; n.Add("סיום שיחה לא נעים"); }

    //    // צפיפות (צפיפות מכריעה)
    //    if (wps > 3.5) { s -= 20; n.Add("דיבור מהיר מדי"); }

    //    notes = string.Join(", ", n);
    //    return Math.Clamp(s, 0, 100);
    //}


    //private double CalculateWpsScore(double? wps)
    //{
    //    // המרה של צפיפות לציון בין 0 ל-100
    //    if (wps > 2.0 && wps < 3.2) return 100;
    //    return 70; // ציון בינוני לצפיפות לא אופטימלית
    //}
   

    private async Task<List<Segment>> TranscribeAudio(string filePath)//to transcribe the audio
    {
        var config = SpeechConfig.FromSubscription(_speechKey, _region);
        config.SpeechRecognitionLanguage = "he-IL"; //define to hebrow

        //define the conversation transcription to return the speaker id in the result
        config.SetProperty("ConversationTranscriptionInRoomAndOnline", "true");

        using var audioConfig = AudioConfig.FromWavFileInput(filePath);
        using var transcriber = new ConversationTranscriber(config, audioConfig);

        var segments = new List<Segment>();
        var stopRecognition = new TaskCompletionSource<int>();

        // אירוע שקופץ בכל פעם ש-Azure מזהה משפט/קטע דיבור
        transcriber.Transcribed += (s, e) => {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                segments.Add(new Segment
                {
                    SpeakerId = e.Result.SpeakerId, // here return  "Guest-1" or "Guest-2"
                    Text = e.Result.Text,           // the text
                    Offset = TimeSpan.FromTicks(e.Result.OffsetInTicks), // who began to talk
                    Duration = e.Result.Duration    // how duration the speaker talk
                });
            }
        };

        // to mark that we finish the recognition
        transcriber.SessionStopped += (s, e) => stopRecognition.TrySetResult(0);

        // begin the trancribe
        await transcriber.StartTranscribingAsync();

        // מחכים עד שהתמלול יסתיים (או עד שיעברו 5 דקות כהגנה)
        await Task.WhenAny(stopRecognition.Task, Task.Delay(TimeSpan.FromMinutes(5)));

        await transcriber.StopTranscribingAsync();

        return segments;
    }
    private string IdentifyAgent(List<Segment> segments, string introPhrase)
    {
        if (string.IsNullOrEmpty(introPhrase)) return "Guest-1"; // default
        //take the 3 sentenses
        var initialSegments = segments.Take(3).ToList();

        foreach (var segment in initialSegments)
        {
            // בדיקה אם משפט הפתיחה (או חלק ממנו) מופיע בטקסט של הדובר
            if (segment.Text.Contains(introPhrase, StringComparison.OrdinalIgnoreCase))
            {
                return segment.SpeakerId; // מצאנו את הנציגה!
            }
        }

        // אם לא מצאנו התאמה מדויקת, נחזור לברירת המחדל (בדרך כלל הדובר הראשון)
        return segments.First().SpeakerId;
    }
}
