using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [MaxLength(200)]
        public string? IntroPhrase { get; set; }

        public string? AudioFolderRoute { get; set; }

        // קשרי גומלין
        public virtual ICollection<Operator> Operators { get; set; }
        public virtual ICollection<Call> Calls { get; set; }
    }
}
