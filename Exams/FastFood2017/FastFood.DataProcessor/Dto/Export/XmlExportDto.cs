using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Export
{
    [XmlType("Category")]
    public class XmlExportDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("MostPopularItem")]
        public MostPopularItem MostPopularItem { get; set; }
    }

    [XmlType("MostPopularItem")]
    public class MostPopularItem
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("TotalMade")]
        public decimal TotalMade { get; set; }

        [XmlElement("TimesSold")]
        public int TimesSold { get; set; }
    }
}
