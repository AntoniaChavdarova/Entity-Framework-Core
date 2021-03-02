using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Export
{
    [XmlType("sale")]
    public class ExportSaleDiscountDto
    {
        [XmlElement("car")]
        public ExportCarInfoDto Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]

        public string CustomerName { get; set; }

        [XmlElement("price")]

        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]

        public decimal PriceWithDiscount { get; set; }

    }

    [XmlType("car")]
    public class ExportCarInfoDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]

        public long TraveledDistance { get; set; }
    }
}
