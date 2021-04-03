using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.DataProcessor.ImportDtos
{
    public class ImportWritersDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Name { get; set; }

       
        [RegularExpression(@"^[A-Z]{1}[a-z]{2,} [A-Z]{1}[a-z]{2,}$")]
        public string Pseudonym { get; set; }
    }
}
