using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.specialservice
{
    public class Segment
    {
        public string SpeakerId { get; set; }
        public string Text { get; set; }
        public TimeSpan Offset { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
