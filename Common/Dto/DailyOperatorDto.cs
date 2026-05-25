using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class DailyOperatorDto
    {
        public int CallId { get; set; }


        public int ScoreId { get; set; }

        [Required]


        //ציון לטון
        public double? OperatorToneScore { get; set; }  // ציון אינטונציה
        //כמה הצליחה להרגיע את הלקוח - כלומר הבדלים ברגש הלקוח בין ההתחלה לסוף
        public double? ConflictResolutionScore { get; set; } // ציון פתרון קונפליקט (השינוי ברגש הלקוח)
        //מהירות פתרון הבעיה - נניח שנמדד לפי משך השיחה או משך הטיפול בבעיה
        public double? ProfessionalismScore { get; set; }
        public double? OverallScore { get; set; }
        public int? SumDailyCalls { get; set; }
        public DayOfWeek? DayName { get; set; }

        public string? GeneralNotes { get; set; }
        public ImprovementTips? ImprovementTips { get; set; }
    }
}
