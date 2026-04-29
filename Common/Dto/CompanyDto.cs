using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dto
{
    public class CompanyDto
    {
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "שם חברה הוא שדה חובה")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "שם חברה חייב להיות בין 2 ל-100 תווים")]
        public string CompanyName { get; set; }

        [MaxLength(200)]
        public string? IntroPhrase { get; set; }

        public string? AudioFolderRoute { get; set; }
    }
}
