using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerDto
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

        [Range(12 ,110)]
        [XmlElement("Age")]
        public int Age { get; set; }

        [Range(typeof(decimal) , "0.01"  , "79228162514264337593543950335")]
        [XmlElement("Balance")]
        public decimal Balance { get; set; }

        [XmlArray("Tickets")]
        public ImportTickets[] Tickets { get; set; }
    }

    [XmlType("Ticket")]
    public class ImportTickets
    {
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}

