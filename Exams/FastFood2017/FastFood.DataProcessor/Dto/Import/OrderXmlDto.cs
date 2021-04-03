using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Order")]
    public class OrderXmlDto
    {
        [XmlElement("Customer")]
        [Required]
        public string Customer { get; set; }

        [XmlElement("Employee")]
        public string Employee { get; set; }

        [XmlElement("DateTime")]
        [Required]
        public string DateTime { get; set; }

        
        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlArray("Items")]
        public ItemsXmlDto[] Items { get; set; }

    }

    [XmlType("Item")]
    public class ItemsXmlDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(3)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Quantity")]
        [Required]
        [Range(1 , int.MaxValue)]
        public int Quantity { get; set; }
    }
}


