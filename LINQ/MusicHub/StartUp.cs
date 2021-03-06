namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();


             DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context, 4)); 

           
        }


        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var sb = new StringBuilder();

            var albums = context.Albums.Where(x => x.ProducerId == producerId)
                 .Select(x => new
                 {
                     x.Name,
                     ReleaseDate = x.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                     ProducerName = x.Producer.Name,
                     TotalPrice = x.Price,
                     Songs = x.Songs.Select(y => new
                     {
                         SongName = y.Name,
                         Price = y.Price,
                         WriterName = y.Writer.Name

                     }).OrderByDescending(y => y.SongName).ThenBy(y => y.WriterName)

                 }).OrderByDescending(x => x.TotalPrice).ToList();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");

                var i = 1;

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{i}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:f2}");
                    sb.AppendLine($"---Writer: {song.WriterName}");

                    i++;
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var sb = new StringBuilder();

            var songs = context.Songs.ToList().Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    Name = x.Name,
                    Writer = x.Writer.Name,
                    Performer = x.SongPerformers.Select(y => 
                    y.Performer.FirstName + " " + y.Performer.LastName
                    ).FirstOrDefault(),
                    
                    AlbumProducer = x.Album.Producer.Name,
                    Duration = x.Duration
                }).OrderBy(x => x.Name).ThenBy(x => x.Writer).ThenBy(x => x.Performer).ToList();

            int i = 1;

            foreach (var song in songs)
            {

                sb.AppendLine($"-Song #{i}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.Performer}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration.ToString("c")}");

                i++;
            }

            return sb.ToString().Trim();

        }
    }
}
