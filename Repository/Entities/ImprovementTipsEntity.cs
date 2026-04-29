using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public enum ImprovementTipsEntity
    {
        None = 0,
        SpeechRate = 1,      // קצב דיבור (מהיר/איטי מדי)
        ToneAndEmpathy = 2,  // טון דיבור ואמפתיה
        TechnicalKnowledge = 3, // ידע מקצועי חסר
        ConflictResolution = 4, // ניהול קונפליקטים
        Clarity = 5          // בהירות והסבר
    }
    
}
