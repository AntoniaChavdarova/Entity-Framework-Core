using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.DataProcessor.ImportDtos
{
    public class ImportProducersDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        public string Name { get; set; }

        [RegularExpression(@"^[A-Z]{1}[a-z]{2,} [A-Z]{1}[a-z]{2,}$")]
        public string Pseudonym { get; set; }

        [RegularExpression(@"^\+359 [0-9]{3} [0-9]{3} [0-9]{3}$")]
        public string PhoneNumber { get; set; }

        public AlbumDto[] Albums { get; set; }
    }

    public class AlbumDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        public string ReleaseDate { get; set; }

    }
}
