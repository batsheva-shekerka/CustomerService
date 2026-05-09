using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace Common.Dto
{
    public class ScoreDto
    {
        //public int Id { get; set; }
   

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

        public ImprovementTips? ImprovementTips { get; set; }

    }
}
