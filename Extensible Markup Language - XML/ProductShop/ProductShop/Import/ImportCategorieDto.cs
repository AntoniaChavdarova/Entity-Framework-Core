using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Import
{
    [XmlType("Category")]
    public class ImportCategorieDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

    }
}
