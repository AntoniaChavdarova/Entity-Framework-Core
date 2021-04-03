namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter 
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone 
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong 
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var writersDtos = JsonConvert.DeserializeObject<ImportWritersDto[]>(jsonString);
            var sb = new StringBuilder();
            var list = new List<Writer>();

            foreach (var writerDto in writersDtos)
            {
                if (!IsValid(writerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var writer = new Writer
                {
                    Name = writerDto.Name,
                    Pseudonym = writerDto.Pseudonym
                };

                list.Add(writer);
                sb.AppendLine(String.Format(SuccessfullyImportedWriter, writer.Name));
            }

            context.Writers.AddRange(list);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var producersDtos = JsonConvert.DeserializeObject<ImportProducersDto[]>(jsonString);
            var sb = new StringBuilder();
            var list = new List<Producer>();

            foreach (var producerDto in producersDtos)
            {
                if (!IsValid(producerDto) )
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var producer = new Producer
                {
                    Name = producerDto.Name,
                    Pseudonym = producerDto.Pseudonym,
                    PhoneNumber = producerDto.PhoneNumber,
                  
                };

                bool isValidAlbum = true;

                foreach (var albumDto in producerDto.Albums)
                {
                    if (!IsValid(albumDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        isValidAlbum = false;
                        continue;

                    }

                    var date = DateTime.ParseExact(albumDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var album = new Album
                    {
                        Name = albumDto.Name,
                        ReleaseDate = date
                    };

                    producer.Albums.Add(album);
                }

                if (isValidAlbum)
                {
                    list.Add(producer);
                }
                
                if(producer.PhoneNumber != null && isValidAlbum)
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithPhone,producer.Name , producer.PhoneNumber , producer.Albums.Count));
                }
                else if(producer.PhoneNumber == null && isValidAlbum)
                {
                    sb.AppendLine(String.Format(SuccessfullyImportedProducerWithNoPhone ,producer.Name  , producer.Albums.Count));
                }
               
            }

            context.Producers.AddRange(list);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {

            var songsDtos = DeserializeObject<ImportSongsDto>("Songs", xmlString);
            var sb = new StringBuilder();
            var list = new List<Song>();

            foreach (var songDto in songsDtos)
            {
                if (!IsValid(songDto))
                {
                    sb.AppendLine(ErrorMessage);
                    
                    continue;
                }

                var genre = Enum.Parse<Genre>(songDto.Genre);
                var timespan = TimeSpan.ParseExact(songDto.Duration, "c", CultureInfo.InvariantCulture);
                var date = DateTime.ParseExact(songDto.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                Album album = context.Albums.FirstOrDefault(x => x.Id == songDto.AlbumId);
                if (album == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Writer writer = context.Writers.FirstOrDefault(x => x.Id == songDto.WriterId);
                if (writer == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var song = new Song
                {
                    Name = songDto.Name,
                    CreatedOn = date,
                    Duration = timespan,
                    Genre = genre,
                    WriterId = songDto.WriterId,
                    AlbumId = songDto.AlbumId,
                    Price = songDto.Price
                };

                context.Songs.Add(song);
                context.SaveChanges();
                sb.AppendLine(String.Format(SuccessfullyImportedSong, song.Name , song.Genre , song.Duration));
            }
            
            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            var performersDtos = DeserializeObject<ImportPerformersDto>("Performers", xmlString);
            var sb = new StringBuilder();
            var list = new List<Performer>();

            foreach (var performerDto in performersDtos)
            {
                //todo id
                if (!IsValid(performerDto) )
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var performer = new Performer
                {
                    FirstName = performerDto.FirstName,
                    LastName = performerDto.LastName,
                    Age = performerDto.Age,
                    NetWorth = performerDto.NetWorth,
                };
                var isSongValid = true;
                foreach (var i in performerDto.PerformersSongs)
                {
                   
                    var song = context.Songs.FirstOrDefault(x => x.Id == i.Id);
                    if(song == null)
                    {
                        isSongValid = false;
                        break;
                    }

                    performer.PerformerSongs.Add(new SongPerformer
                    {
                        Song = song,
                        Performer = performer

                    }) ;
                }

                if (!isSongValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                list.Add(performer);

                sb.AppendLine(String.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformerSongs.Count));
            }
            context.Performers.AddRange(list);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }

        private static T[] DeserializeObject<T>(string rootElement, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootElement));
            var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));
            return deserializedDtos;
        }
    }
}