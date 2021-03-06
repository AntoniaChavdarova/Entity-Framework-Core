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

            Console.WriteLine(ExportAlbumsInfo(context, 9)); 

           
        }

        //a Producer Id.Export all albums which are produced by the provided Producer Id.For each Album, get the Name, Release date in format "MM/dd/yyyy", Producer Name, the Album Songs with each Song Name, Price (formatted to the second digit) and the Song Writer Name.Sort the Songs by Song Name(descending) and by Writer(ascending). At the end export the Total Album Price with exactly two digits after the decimal place.Sort the Albums by their Total Price (descending).

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var sb = new StringBuilder();

            var albums = context.Albums.ToList().Where(x => x.ProducerId == producerId)
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
            throw new NotImplementedException();
        }
    }
}
