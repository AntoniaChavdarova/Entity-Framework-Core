using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Export
{

    public class ExportUserCountDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public ExportUserProductDto[] Users { get; set; }
    }

    [XmlType("User")]
   public class ExportUserProductDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("soldProducts")]
        public SoldProductInfoDto SoldProduct { get; set; }
    }

    [XmlType("SoldProducts")]
    public class SoldProductInfoDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public SoldProductDto[] Products { get; set; }

    }

    [XmlType("Product")]
    public class SoldProductDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }


}
