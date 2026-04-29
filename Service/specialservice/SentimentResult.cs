using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.specialservice
{
    public class SentimentResult
    {
        public string Sentiment { get; set; }
        public double Positive { get; set; }
        public double Neutral { get; set; }
        public double Negative { get; set; }
    }
}
