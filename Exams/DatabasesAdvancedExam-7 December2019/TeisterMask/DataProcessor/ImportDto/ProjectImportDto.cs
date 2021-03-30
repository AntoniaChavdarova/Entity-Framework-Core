using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Project")]
   public class ProjectImportDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; }

        public string OpenDate { get; set; }

        public string DueDate { get; set; }

        [XmlArray("Tasks")]
        public ImportTaskDto[] Tasks { get; set; }
    }

    [XmlType("Task")]
    public class ImportTaskDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [Required]
        [XmlElement("ExecutionType")]
        [EnumDataType(typeof(ExecutionType))]

        public string ExecutionType { get; set; }

        [Required]
        [XmlElement("LabelType")]
        [EnumDataType(typeof(LabelType))]
        public string LabelType { get; set; }
    }
}

//< Projects >
//  < Project >
//    < Name > S </ Name >
//    < OpenDate > 25 / 01 / 2018 </ OpenDate >
//    < DueDate > 16 / 08 / 2019 </ DueDate >
//    < Tasks >
//      < Task >
//        < Name > Australian </ Name >
//        < OpenDate > 19 / 08 / 2018 </ OpenDate >
//        < DueDate > 13 / 07 / 2019 </ DueDate >
//        < ExecutionType > 2 </ ExecutionType >
//        < LabelType > 0 </ LabelType >
//      </ Task >

