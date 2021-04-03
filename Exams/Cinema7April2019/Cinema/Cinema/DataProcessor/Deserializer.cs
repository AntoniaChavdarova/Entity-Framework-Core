namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDtos = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);
            var sb = new StringBuilder();


            foreach (var movieDto in moviesDtos)
            {
                if (!IsValid(movieDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Genre = Enum.Parse<Genre>(movieDto.Genre),
                    Duration = TimeSpan.ParseExact(movieDto.Duration, "hh\\:mm\\:ss", CultureInfo.InvariantCulture),
                    Rating = movieDto.Rating,
                    Director = movieDto.Director

                };

                context.Movies.Add(movie);
                context.SaveChanges();
                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating.ToString("f2")));
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsDtos = JsonConvert.DeserializeObject<ImportHalls[]>(jsonString);
            var sb = new StringBuilder();

            foreach (var hallDto in hallsDtos)
            {
                if (!IsValid(hallDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = new Hall
                {
                    Name = hallDto.Name,
                    Is3D = hallDto.Is3D,
                    Is4Dx = hallDto.Is4Dx,

                };

                for (int i = 0; i < hallDto.Seats; i++)
                {
                    hall.Seats.Add(new Seat() { Hall = hall });
                }

                if (hall.Seats.Count <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string projectionType;
                if (hall.Is3D && hall.Is4Dx)
                {
                    projectionType = "4Dx/3D";
                }
                else if (hall.Is3D)
                {
                    projectionType = "3D";
                }
                else if (hall.Is4Dx)
                {
                    projectionType = "4Dx";
                }
                else
                {
                    projectionType = "Normal";
                }

                context.Halls.Add(hall);
                context.SaveChanges();
                sb.AppendLine(String.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hall.Seats.Count));

            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var projectionsDtos = DeserializeObject<ImportDtoXmlProcetion>("Projections", xmlString);
            var sb = new StringBuilder();

            foreach (var projectDto in projectionsDtos)
            {
                var movie = context.Movies.FirstOrDefault(x => x.Id == projectDto.MovieId);

                if (movie == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = context.Halls.FirstOrDefault(x => x.Id == projectDto.HallId);

                if (hall == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projection = new Projection
                {
                    MovieId = movie.Id,
                    HallId = hall.Id,
                    DateTime = DateTime.ParseExact(projectDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };

                projection.Movie = movie;
                projection.Hall = hall;

                context.Projections.Add(projection);
                context.SaveChanges();
                sb.AppendLine(String.Format(SuccessfulImportProjection, projection.Movie.Title, projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)));
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var customerDtos = DeserializeObject<ImportCustomerDto>("Customers", xmlString);
            var sb = new StringBuilder();


            foreach (var customerDto in customerDtos)
            {
                if (!IsValid(customerDto) )
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = new Customer
                {
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    Age = customerDto.Age,
                    Balance = customerDto.Balance
                };



                foreach (var ticketDto in customerDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var ticket = new Ticket
                    {
                        ProjectionId = ticketDto.ProjectionId,
                        Price = ticketDto.Price
                    };

                    customer.Tickets.Add(ticket);
                }

                context.Customers.Add(customer);
                context.SaveChanges();
                sb.AppendLine(String.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName , customer.Tickets.Count));
            }
            return sb.ToString().TrimEnd();
        }


        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

        private static T[] DeserializeObject<T>(string rootElement, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootElement));
            var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));
            return deserializedDtos;
        }
    }
}