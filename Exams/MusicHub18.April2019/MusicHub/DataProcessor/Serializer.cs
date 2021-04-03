namespace MusicHub.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {

            var albums = context.Albums.Where(x => x.ProducerId == producerId)
                .Select(x => new
                {
                    AlbumName = x.Name,
                    ReleaseDate = x.ReleaseDate.ToString("MM/dd/yyyy" , CultureInfo.InvariantCulture),
                    ProducerName = x.Producer.Name,
                    Songs = x.Songs.Select(y => new
                    {
                        SongName = y.Name,
                        Price = y.Price.ToString("F2"),
                        Writer = y.Writer.Name
                    }).OrderByDescending(a => a.SongName).ThenBy(a => a.Writer).ToArray(),
                    AlbumPrice = x.Price.ToString("F2")
                }).OrderByDescending(x => x.AlbumPrice).ToArray();

            return JsonConvert.SerializeObject(albums, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            //Use the method provided in the project skeleton, which receives a song duration(in seconds)
            //    .Export the songs which are above the given duration.
            //    For each song, export its name, performer full name, writer name,
            //    album producer and duration in format("c").
            //    Sort the songs by their name(ascending),
            //    by writer(ascending) and by performer(ascending).

            var songs = context.Songs.Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new ExportDto()
                {
                    SongName = x.Name,
                    Writer = x.Writer.Name,
                    Performer = x.SongPerformers.Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName).FirstOrDefault(),
                    AlbumProducer = x.Album.Producer.Name,
                    Duration = x.Duration.ToString("c")
                })
                .OrderBy(x => x.SongName)
                .ThenBy(x => x.Writer)
                .ThenBy(x => x.Performer)
                .ToArray();

            return SerializeXml(songs, "Songs");
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