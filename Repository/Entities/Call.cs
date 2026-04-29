using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Repository.Entities
{
    public class Call
    {
        [Key]
        public int CallId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public int? OperatorId { get; set; } // הוספנו כאן ישירות את המפעיל

        [Required]
        public DateTime CallDate { get; set; }

        public TimeSpan? Duration { get; set; }

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

        // קשרי גומלין
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("OperatorId")]
        public virtual Operator? Operator { get; set; }

        public virtual Score Score { get; set; } // קשר של 1 ל-1 לציון
    }
}
