
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using static flight_management_system.Pages.IndexModel;

namespace flight_management_system.Pages.Booking
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Flight> listFlights = new List<Flight>();
        public List<Airport> listAirport = new List<Airport>();

        public string errorMessage = "";
        public string successMessage = "";


        [BindProperty]
        public Search searchInfo { set; get; } = new Search();
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            string destinationId = Request.Query["destinationId"];
            string location = Request.Query["location"];
            string departureId = Request.Query["departureId"];

            if (!string.IsNullOrEmpty(Request.Query["departure"]) && !string.IsNullOrEmpty(Request.Query["destination"]) && !string.IsNullOrEmpty(Request.Query["departurepoint"]))
            {
                searchInfo.departure = DateTime.Parse(Request.Query["departure"]);
                searchInfo.destination = Request.Query["destination"];
                searchInfo.departurepoint = Request.Query["departurepoint"];
                getSearchResults(searchInfo);
            }
            else
            {
                getFlights();
            }

            if (!string.IsNullOrEmpty(destinationId))
            {
                getFlightsByDestination(destinationId);
            }

            if (!string.IsNullOrEmpty(location))
            {
                getFlightsByLocation(location);
            }

            getDestinations();

        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Invalid search criteria.";
                return Page();
            }
            else
            {
                errorMessage = searchInfo.destination;
                getSearchResults(searchInfo);
                return Page();
            }
        }

        public IActionResult getSearchResults([Bind] Search searchInfo)
        {
            errorMessage = "";

            if (!ModelState.IsValid)
            {
                errorMessage = "Provide all search details";
                return Page();
            }

            try
            {
                string sqlConnection = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(sqlConnection))
                {
                    con.Open();
                    string sqlQuery = @"
                SELECT F.Id, F.departure, F.arrival, F.departure_airport_id, F.destination_airport_id, F.aircraft_id, 
                       O.Name AS OriginName, D.Name AS DestinationName
                FROM flight F
                JOIN airport O ON F.departure_airport_id = O.Id
                JOIN airport D ON F.destination_airport_id = D.Id  WHERE CAST(departure AS DATE) = @date AND destination_airport_id = @destination AND departure_airport_id =@departurepoint";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@date", searchInfo.departure);
                        cmd.Parameters.AddWithValue("@destination", searchInfo.destination);
                        cmd.Parameters.AddWithValue("@departurepoint", searchInfo.departurepoint);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            listFlights.Clear();
                            while (reader.Read())
                            {
                                Flight flight = new Flight();
                                flight.Id = reader.GetString(0);
                                flight.Departure = reader.GetDateTime(1);
                                flight.Arrival = reader.GetDateTime(2);
                                flight.Origin = reader.GetString(3);
                                flight.Destination = reader.GetString(4);
                                flight.Aircraft = reader.GetString(5);

                                flight.OriginName = reader.GetString(6);
                                flight.DestinationName = reader.GetString(7);

                                listFlights.Add(flight);
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message;
                return Page();
            }
        }

        public IActionResult getFlightsByDestination(string destinationId)
        {
            try
            {
                string sqlConnection = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(sqlConnection))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM flight WHERE destination_airport_id = @destination";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@destination", destinationId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            listFlights.Clear();
                            while (reader.Read())
                            {
                                Flight flight = new Flight();
                                flight.Id = reader.GetString(0);
                                flight.Departure = reader.GetDateTime(1);
                                flight.Arrival = reader.GetDateTime(2);
                                flight.Origin = reader.GetString(3);
                                flight.Destination = reader.GetString(4);
                                flight.Aircraft = reader.GetString(5);

                                flight.OriginName = reader.GetString(6);
                                flight.DestinationName = reader.GetString(7);

                                listFlights.Add(flight);
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message;
                return Page();
            }
        }

        public IActionResult getFlightsByLocation(string location)
        {
            try
            {
                string sqlConnection = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(sqlConnection))
                {
                    con.Open();
                    string sqlQuery = @"
                SELECT F.Id, F.departure, F.arrival, F.departure_airport_id, F.destination_airport_id, F.aircraft_id, 
                       O.Name AS OriginName, D.Name AS DestinationName
                FROM flight F
                JOIN airport O ON F.departure_airport_id = O.Id
                JOIN airport D ON F.destination_airport_id = D.Id
                    WHERE A.location = @location ";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@location", location);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            listFlights.Clear();
                            while (reader.Read())
                            {
                                Flight flight = new Flight();
                                flight.Id = reader.GetString(0);
                                flight.Departure = reader.GetDateTime(1);
                                flight.Arrival = reader.GetDateTime(2);
                                flight.Origin = reader.GetString(3);
                                flight.Destination = reader.GetString(4);
                                flight.Aircraft = reader.GetString(5);

                                flight.OriginName = reader.GetString(6);
                                flight.DestinationName = reader.GetString(7);

                                listFlights.Add(flight);
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                errorMessage += ex.Message;
                return Page();
            }
        }
        private void getFlights()
        {
            listFlights.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = @"
                SELECT F.Id, F.departure, F.arrival, F.departure_airport_id, F.destination_airport_id, F.aircraft_id, 
                       O.Name AS OriginName, D.Name AS DestinationName
                FROM flight F
                JOIN airport O ON F.departure_airport_id = O.Id
                JOIN airport D ON F.destination_airport_id = D.Id;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Flight flight = new Flight();
                                flight.Id = reader.GetString(0);
                                flight.Departure = reader.GetDateTime(1);
                                flight.Arrival = reader.GetDateTime(2);
                                flight.Origin = reader.GetString(3);
                                flight.Destination = reader.GetString(4);
                                flight.Aircraft = reader.GetString(5);
                               flight.OriginName = reader.GetString(6);
                               flight.DestinationName = reader.GetString(7);

                                listFlights.Add(flight);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }

        private void getDestinations()
        {
            listAirport.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Id, Name, Location, image FROM airport;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Airport airport = new Airport
                                {
                                    Id = reader.GetString(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Location = reader.GetString(reader.GetOrdinal("Location")),
                                    Image = reader.GetString(reader.GetOrdinal("Image"))
                                };
                                listAirport.Add(airport);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }

        public class Flight
        {
            public string Id { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public string OriginName { get; set; }   
            public string DestinationName { get; set; }
            public String Aircraft { get; set; }
        }
        public class Airport
        {

            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public string Image { get; set; }
        }

        public class Search
        {                                                                            
            [Required(ErrorMessage = "Destination is required")]
            public DateTime departure { get; set; }
            [DataType(DataType.Date)]
            public string destination { get; set; }
            public string departurepoint { get; set; }
        }
    }
}
