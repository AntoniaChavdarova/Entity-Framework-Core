namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
                var prisoner = context.Prisoners
              .Where(x => ids.Contains(x.Id))

              .Select(y => new 
              {
                  Id = y.Id,
                  Name = y.FullName,
                  CellNumber = y.Cell.CellNumber,
                  Officers = y.PrisonerOfficers.Select(x => new 
                  {
                      OfficerName = x.Officer.FullName,
                      Department = x.Officer.Department.Name
                  }).
                  OrderBy(x => x.OfficerName).ToArray(),
                  TotalOfficerSalary = decimal.Parse(y.PrisonerOfficers.Sum(y => y.Officer.Salary).ToString("f2"))

              }).OrderBy(x => x.Name).ThenBy(x => x.Id)
                .ToArray();

              
           string res = JsonConvert.SerializeObject(prisoner, Newtonsoft.Json.Formatting.Indented);
            return res;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisoners = context.Prisoners.Where(x => prisonersNames.Contains(x.FullName))
                .Select(x => new PrisonerExport
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    Mails = x.Mails.Select(a => new Mails
                    {
                        Description = string.Join("" , a.Description.Reverse())
                    }).ToArray()


                }).OrderBy(x => x.Name).ThenBy(x => x.Id).ToArray();

            return SerializeXml(prisoners, "Prisoners");

        }

        private static string SerializeXml<T>(T[] objects, string root)
        {
            var serializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(root));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), objects, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}