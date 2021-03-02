using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Export
{
    [XmlType("car")]
    public class ExportCarswithPartDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]

        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public ExportPartInfoDto[] Parts { get; set; }
    }

    [XmlType("part")]
    public class ExportPartInfoDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }


}
