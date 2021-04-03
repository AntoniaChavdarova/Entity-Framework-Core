using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Performer")]
    public class ImportPerformersDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        [XmlElement("LastName")]
        public string LastName { get; set; }

        [Range(18 ,70)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [XmlElement("NetWorth")]
        public decimal NetWorth { get; set; }

        [XmlArray("PerformersSongs")]
        public PerformersSongsDto[] PerformersSongs { get; set; }


    }

    [XmlType("Song")]
    public class PerformersSongsDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}

