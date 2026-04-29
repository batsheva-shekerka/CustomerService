using Azure;
using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.specialservice
{
    public class TextAnalysisService
    {
        private readonly TextAnalyticsClient _client;

        public TextAnalysisService(string endpoint, string key)
        {
            _client = new TextAnalyticsClient(
                new Uri(endpoint),
                new AzureKeyCredential(key)
            );
        }

        public SentimentResult AnalyzeSentiment(string text)
        {
            // שולח את הטקסט ל-Azure ומקבל ניתוח רגש
            var response = _client.AnalyzeSentiment(text, language: "he");

            return new SentimentResult
            {
                Sentiment = response.Value.Sentiment.ToString(),
                Positive = response.Value.ConfidenceScores.Positive,
                Neutral = response.Value.ConfidenceScores.Neutral,
                Negative = response.Value.ConfidenceScores.Negative
            };
        }
    }
}
