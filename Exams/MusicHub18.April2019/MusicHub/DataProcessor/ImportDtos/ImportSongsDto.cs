using MusicHub.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Song")]
    public class ImportSongsDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Duration")]
        public string Duration { get; set; }

        [XmlElement("CreatedOn")]
        public string CreatedOn { get; set; }

        [Required]
        [XmlElement("Genre")]
        [EnumDataType(typeof(Genre))]
        public string Genre { get; set; }

        [XmlElement("AlbumId")]
        public int? AlbumId { get; set; }

        [XmlElement("WriterId")]
        public int WriterId { get; set; }

        [XmlElement("Price")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }
    }
}
