using Cinema.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportMovieDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Title { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        public string Genre { get; set; }

        [Required]
        public string Duration { get; set; }

        [Range(1 ,10)]
        public double Rating { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Director { get; set; }
    }
}

//"Title": "Little Big Man",
//    "Genre": "Western",
//    "Duration": "01:58:00",
//    "Rating": 28,
//    "Director": "Duffie Abrahamson"

