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

// הוסיפי את ה-using המדויק למיקום ה-Models שלך, למשל:
// using CustomerService.webApi.Repository.Models;

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

    //public async Task ProcessFullCallChain(string filePath, int operatorId)
    //{
    //    // 1. שינוי קטן: שימוש ב-Include כדי להביא גם את נתוני החברה (עבור משפט הפתיחה)
    //    var op = await _context.Set<Operator>()
    //        .Include(o => o.Company)
    //        .FirstOrDefaultAsync(o => o.OperatorId == operatorId);

    //    if (op == null) return;

    //    // 2. תמלול (ללא שינוי)
    //    var segments = await TranscribeAudio(filePath);
    //    if (!segments.Any()) return;

    //    // --- תוספת: זיהוי מי הנציגה מבין הדוברים ---
    //    string introPhrase = op.Company?.IntroPhrase ?? "";
    //    string agentSpeakerId = IdentifyAgent(segments, introPhrase);
    //    string customerSpeakerId = (agentSpeakerId == "Guest-1") ? "Guest-2" : "Guest-1";
    //    // ------------------------------------------

    //    // 3. יצירת רשומת שיחה ראשית (ללא שינוי)
    //    var newCall = new Call
    //    {
    //        CompanyId = op.CompanyId,
    //        CallDate = DateTime.Now,
    //        Duration = segments.Max(s => s.Offset + s.Duration),
    //        CallParticipants = new List<CallParticipantAnalysis>()
    //    };

    //    // 4. ניתוח נציגה - שימי לב ששיניתי כאן ל-agentSpeakerId הדינמי
    //    var agentText = string.Join(" ", segments.Where(s => s.SpeakerId == agentSpeakerId).Select(s => s.Text));
    //    var agentAnalysis = new CallParticipantAnalysis
    //    {
    //        ParticipantType = "Operator",
    //        OperatorId = operatorId,
    //        Transcript = agentText,
    //        // כאן הנתונים האמיתיים שלך מ-NAudio
    //        AvgVolume = 0.5,
    //        PeakVolume = 0.8
    //    };

    //    // 5. ניתוח לקוח - שימי לב ששיניתי ל-customerSpeakerId הדינמי
    //    var customerText = string.Join(" ", segments.Where(s => s.SpeakerId == customerSpeakerId).Select(s => s.Text));
    //    var customerAnalysis = new CallParticipantAnalysis
    //    {
    //        ParticipantType = "Customer",
    //        Transcript = customerText
    //    };

    //    // 6. הוספה ושמירה (ללא שינוי)
    //    newCall.CallParticipants.Add(agentAnalysis);
    //    newCall.CallParticipants.Add(customerAnalysis);

    //    _context.Set<Call>().Add(newCall);
    //    await _context.SaveChangesAsync();
    //}
    //public async Task ProcessFullCallChain(string filePath, int operatorId)
    //{
    //    // 1. שליפת נתונים מקדימה (נציגה + חברה)
    //    var op = await _context.Set<Operator>()
    //        .Include(o => o.Company)
    //        .FirstOrDefaultAsync(o => o.OperatorId == operatorId);

    //    if (op == null) return;

    //    // 2. תמלול השיחה (Azure Speech SDK)
    //    var segments = await TranscribeAudio(filePath);
    //    if (segments == null || !segments.Any()) return;

    //    // 3. זיהוי מי הדובר שהוא הנציגה (לפי משפט הפתיחה)
    //    string agentSpeakerId = IdentifyAgent(segments, op.Company?.IntroPhrase ?? "");
    //    string customerSpeakerId = (agentSpeakerId == "Guest-1") ? "Guest-2" : "Guest-1";

    //    // 4. יצירת אובייקט השיחה הראשי
    //    var newCall = new Call
    //    {
    //        CompanyId = op.CompanyId,
    //        CallDate = DateTime.Now,
    //        Duration = segments.Max(s => s.Offset + s.Duration),
    //        CallParticipants = new List<CallParticipantAnalysis>()
    //    };

    //    // 5. ניתוח נציגה (Operator)
    //    var agentAnalysis = await AnalyzeParticipant(segments, agentSpeakerId, "Operator", operatorId, filePath);

    //    // 6. ניתוח לקוח (Customer)
    //    var customerAnalysis = await AnalyzeParticipant(segments, customerSpeakerId, "Customer", null, filePath);

    //    // הוספה לרשימת המשתתפים של השיחה
    //    newCall.CallParticipants.Add(agentAnalysis);
    //    newCall.CallParticipants.Add(customerAnalysis);

    //    // 7. שמירה ראשונה ל-DB (כדי לקבל CallId ו-ParticipantId)
    //    _context.Set<Call>().Add(newCall);
    //    await _context.SaveChangesAsync();

    //    // 8. יצירת רשומות בטבלת Score עבור כל משתתף
    //    foreach (var participant in newCall.CallParticipants)
    //    {
    //        var scoreEntry = new Score
    //        {
    //            CallId = newCall.CallId,
    //            ParticipantId = participant.ParticipantId,

    //            // שקלול הנתונים לתוך עמודות הניקוד
    //            AvgVolumeScore = participant.AvgVolume * 100, // המרה לאחוזים אם צריך
    //            PeakVolumeScore = participant.PeakVolume * 100,
    //            WordsPerSecondScore = CalculateWpsScore(participant.WordsPerSecond),
    //            OverallScore = participant.Score,
    //            Notes = participant.ImprovementNotes
    //        };

    //        _context.Set<Score>().Add(scoreEntry);
    //    }

    //    // שמירה סופית של הציונים
    //    await _context.SaveChangesAsync();
    //}

    // --- פונקציות עזר תומכות ---

    //private async Task<CallParticipantAnalysis> AnalyzeParticipant(List<Segment> allSegments, string speakerId, string type, int? opId, string filePath)
    //{
    //    var mySegments = allSegments.Where(s => s.SpeakerId == speakerId).ToList();
    //    var fullText = string.Join(" ", mySegments.Select(s => s.Text)).Trim(); // הוספתי Trim למניעת רווחים מיותרים

    //    // ניתוח ווליום (כאן תחברי את NAudio בהמשך)
    //    double avgVol = 0.4;
    //    double peakVol = 0.7;

    //    // חישוב צפיפות (Words Per Second)
    //    double totalSeconds = mySegments.Any() ? (mySegments.Last().Offset + mySegments.Last().Duration - mySegments.First().Offset).TotalSeconds : 0;
    //    double wps = totalSeconds > 0 ? (fullText.Split(' ').Length / totalSeconds) : 0;

    //    // --- השינוי המרכזי מתחיל כאן ---
    //    DocumentSentiment fullSentiment = null;
    //    DocumentSentiment lastWordsSentiment = null;

    //    // בודקים שיש טקסט לפני שפונים ל-Azure
    //    if (!string.IsNullOrWhiteSpace(fullText))
    //    {
    //        var sentimentResult = await _textClient.AnalyzeSentimentAsync(fullText);
    //        fullSentiment = sentimentResult.Value;

    //        var last20Words = string.Join(" ", fullText.Split(' ', StringSplitOptions.RemoveEmptyEntries).TakeLast(20));
    //        var lastWordsResult = await _textClient.AnalyzeSentimentAsync(last20Words);
    //        lastWordsSentiment = lastWordsResult.Value;
    //    }
    //    else
    //    {
    //        Console.WriteLine($"[Warning] No text found for {type} ({speakerId}). Skipping Azure Sentiment.");
    //    }

    //    string improvement;
    //    // שלחתי את האובייקטים גם אם הם null, הפונקציה CalculateIndividualScore תטפל בזה
    //    double score = CalculateIndividualScore(type, fullSentiment, lastWordsSentiment, wps, peakVol, out improvement);
    //    // --- סיום השינוי המרכזי ---

    //    return new CallParticipantAnalysis
    //    {
    //        ParticipantType = type,
    //        OperatorId = opId,
    //        Transcript = fullText,
    //        AvgVolume = avgVol,
    //        PeakVolume = peakVol,
    //        WordsPerSecond = wps,
    //        Score = score,
    //        ImprovementNotes = improvement,
    //        Duration = TimeSpan.FromSeconds(totalSeconds)
    //    };
    //}

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

        // 3. זיהוי דוברים
        string agentId = IdentifyAgent(segments, op.Company?.IntroPhrase ?? "");
        // שינוי כאן: מחפשים דובר שהוא לא הנציגה, ואם אין - פשוט לוקחים את הרשימה הריקה בלי לקרוס
        var customerSegmentObj = segments.FirstOrDefault(s => s.SpeakerId != agentId);
        string customerId = customerSegmentObj?.SpeakerId ?? "Unknown-Customer";
        // 4. ניתוח ווליום (NAudio)
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
        //var customerSegments = segments.Where(s => s.SpeakerId == customerId).ToList();

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

        // ניתוח רגש לקוח (התחלה וסוף)
        var customerStartText = string.Join(" ", customerSegments.Take(3).Select(s => s.Text));
        var customerEndText = string.Join(" ", customerSegments.TakeLast(3).Select(s => s.Text));

        if (!string.IsNullOrWhiteSpace(customerStartText))
        {
            var resStart = await _textClient.AnalyzeSentimentAsync(customerStartText);
            custSentimentStart = resStart.Value.ConfidenceScores.Positive - resStart.Value.ConfidenceScores.Negative;
        }

        if (!string.IsNullOrWhiteSpace(customerEndText))
        {
            var resEnd = await _textClient.AnalyzeSentimentAsync(customerEndText);
            custSentimentEnd = resEnd.Value.ConfidenceScores.Positive - resEnd.Value.ConfidenceScores.Negative;
        }

        // 7. עכשיו יוצרים את אובייקט ה-Call ומשתמשים במשתנים שהכנו
        var newCall = new Call
        {
            CompanyId = op.CompanyId,
            OperatorId = operatorId,
            CallDate = DateTime.Now,
            Duration = segments.Max(s => s.Offset + s.Duration),

            // נתוני טלפנית
            OperatorTranscript = agentText,
            OperatorSentiment = opSentiment,
            OperatorMaxVolume = agentVol.peak,
            OperatorWordsPerSecond = CalculateWPS(agentSegments, agentText),

            // נתוני לקוח
            CustomerTranscript = customerText,
            CustomerMaxVolume = customerVol.peak,
            CustomerSentimentStart = custSentimentStart,
            CustomerSentimentEnd = custSentimentEnd
        };

        // 8. יצירת אובייקט Score
        newCall.Score = CalculateCallScore(newCall);

        // 9. שמירה ל-DB
        _context.Set<Call>().Add(newCall);
        await _context.SaveChangesAsync();

        // ניקוי קבצים
        if (File.Exists(agentWav)) File.Delete(agentWav);
        if (File.Exists(customerWav)) File.Delete(customerWav);
    }


    private Score CalculateCallScore(Call call)
    {
        var score = new Score();

        // 1. ציון טון (מבוסס על סנטימנט הנציגה ו-ווליום)
        // אם הנציגה הייתה חיובית והווליום לא צורח - ציון גבוה
        //double toneBase = (call.OperatorSentiment == "Positive") ? 100 : 60;
        double toneBase =(double)(100 - call.OperatorMaxVolume*10);
        toneBase -= (double)(call.CustomerMaxVolume * 5);
        //score.OperatorToneScore = (call.OperatorMaxVolume < 0.8) ? toneBase : toneBase - 20;
        score.OperatorToneScore = toneBase;

        // 2. פתרון קונפליקטים (השינוי ברגש הלקוח)
        // חישוב: רגש סוף פחות רגש התחלה. אם הסוף חיובי יותר מההתחלה - הציון עולה.
        double emotionalShift = (call.CustomerSentimentEnd ?? 0) - (call.CustomerSentimentStart ?? 0);
        score.ConflictResolutionScore = Math.Clamp(70 + (emotionalShift * 30), 0, 100);

        // 3. מקצועיות (למשל: מהירות דיבור אופטימלית)
        // נניח שבין 2 ל-3 מילים בשנייה זה אידיאלי
        double fast = 100 - (double)(call.OperatorWordsPerSecond * 10);
        bool due = call.Duration <= TimeSpan.FromMinutes(3);
        score.ProfessionalismScore = fast+((due)?fast/100*10:0);
        //score.ProfessionalismScore = (call.OperatorWordsPerSecond >= 2.0 && call.OperatorWordsPerSecond <= 3.5) ? 100 : 70;

        // 4. ציון סופי משוקלל
        score.OverallScore = (score.OperatorToneScore + score.ConflictResolutionScore + score.ProfessionalismScore) / 3;
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
                score.ImprovementTips = ImprovementTipsEntity.Clarity;
            }
            else
            {
                score.ImprovementTips = ImprovementTipsEntity.TechnicalKnowledge;
            }
        }

        return score;
    }

    private double CalculateWPS(List<Segment> segments, string text)
    {
        if (!segments.Any() || string.IsNullOrWhiteSpace(text)) return 0;
        var totalTime = segments.Max(s => s.Offset + s.Duration) - segments.Min(s => s.Offset);
        var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return totalTime.TotalSeconds > 0 ? (wordCount / totalTime.TotalSeconds) : 0;
    }
    private double CalculateIndividualScore(string type, DocumentSentiment full, DocumentSentiment lastWords, double wps, double peak, out string notes)
    {
        double s = 100;
        var n = new List<string>();

        if (full == null)
        {
            notes = "לא זוהה דיבור בשיחה מצד משתתף זה.";
            return 0;
        }

        // דוגמה ללוגיקה: אם הסנטימנט הכללי שלילי
        if (full.Sentiment == TextSentiment.Negative) { s -= 30; n.Add("טון כללי שלילי"); }

        // אם הסיום (20 מילים) שלילי - משקל גבוה
        if (lastWords.Sentiment == TextSentiment.Negative) { s -= 20; n.Add("סיום שיחה לא נעים"); }

        // צפיפות (צפיפות מכריעה)
        if (wps > 3.5) { s -= 20; n.Add("דיבור מהיר מדי"); }

        notes = string.Join(", ", n);
        return Math.Clamp(s, 0, 100);
    }

    private double CalculateWpsScore(double? wps)
    {
        // המרה של צפיפות לציון בין 0 ל-100
        if (wps > 2.0 && wps < 3.2) return 100;
        return 70; // ציון בינוני לצפיפות לא אופטימלית
    }
    private async Task<List<Segment>> TranscribeAudio(string filePath)
    {
        var config = SpeechConfig.FromSubscription(_speechKey, _region);
        config.SpeechRecognitionLanguage = "he-IL"; // הגדרה לעברית

        // הגדרה קריטית: זיהוי שיחה עם הפרדת דוברים
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
                    SpeakerId = e.Result.SpeakerId, // כאן חוזר "Guest-1" או "Guest-2"
                    Text = e.Result.Text,           // המילים שנאמרו
                    Offset = TimeSpan.FromTicks(e.Result.OffsetInTicks), // מתי התחיל לדבר
                    Duration = e.Result.Duration    // כמה זמן דיבר
                });
            }
        };

        // כשנגמר הקובץ, אנחנו מסמנים שהסתיים
        transcriber.SessionStopped += (s, e) => stopRecognition.TrySetResult(0);

        // מתחילים את התמלול
        await transcriber.StartTranscribingAsync();

        // מחכים עד שהתמלול יסתיים (או עד שיעברו 5 דקות כהגנה)
        await Task.WhenAny(stopRecognition.Task, Task.Delay(TimeSpan.FromMinutes(5)));

        await transcriber.StopTranscribingAsync();

        return segments;
    }
    private string IdentifyAgent(List<Segment> segments, string introPhrase)
    {
        if (string.IsNullOrEmpty(introPhrase)) return "Guest-1"; // ברירת מחדל אם אין משפט פתיחה

        // לוקחים את 3 המשפטים הראשונים בשיחה (כי לפעמים הלקוח אומר "הלו" לפני הנציגה)
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
