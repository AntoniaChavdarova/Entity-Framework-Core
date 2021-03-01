using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Import
{
    [XmlType("CategoryProduct")]
    public class ImportCategoryProductDto
    {
        [XmlElement("CategoryId")]
        public int CategoryId { get; set; }


        [XmlElement("ProductId")]
        public int ProductId { get; set; }
    }
}
