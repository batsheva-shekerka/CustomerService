using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class CallDto
    {

        [Required(ErrorMessage = "חובה להזין תאריך שיחה")]
        public DateTime CallDate { get; set; }

        [Range(0, 1000, ErrorMessage = "משך שיחה חייב להיות מספר חיובי (בדקות)")]
        public TimeSpan Duration { get; set; }

        [Required]
        public int OperatorId { get; set; }

        // ניתן להוסיף שדות עזר לתצוגה בלבד
        public string OperatorName { get; set; }



        public int CallId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        // --- נתוני טלפנית ---

        //תמלול
        public string? OperatorTranscript { get; set; }

        //רגש בשיחה
        public string? OperatorSentiment { get; set; }

        //וויליום מקסימלי
        public double? OperatorMaxVolume { get; set; }

        //מילים לשניה
        public double? OperatorWordsPerSecond { get; set; }

        // --- נתוני לקוח ---
        public string? CustomerTranscript { get; set; }
        public double? CustomerSentimentStart { get; set; } // רגש התחלתי (למשל 0-1)
        public double? CustomerSentimentEnd { get; set; }   // רגש סופי
        public double? CustomerMaxVolume { get; set; }

        public string? GeneralNotes { get; set; }

    }
}
