using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Flight
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Flight> listFlights = new List<Flight>();
        public string errorMessage = "";
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            listFlights.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM flight";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Flight flight = new Flight();
                                flight.Id = reader.GetString(reader.GetOrdinal("id"));
                                flight.Departure = reader.GetDateTime(reader.GetOrdinal("departure"));
                                flight.Arrival = reader.GetDateTime(reader.GetOrdinal("arrival"));
                                flight.Origin = reader.GetString(reader.GetOrdinal("departure_airport_id"));
                                flight.Destination = reader.GetString(reader.GetOrdinal("destination_airport_id"));
                                flight.Aircraft = reader.GetString(reader.GetOrdinal("aircraft_id"));
                                flight.Price = reader.GetDouble(reader.GetOrdinal("price"));

                                listFlights.Add(flight);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Console.WriteLine(ex.ToString());
            }
        }


        public class Flight
        {
            public string Id { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public String Aircraft { get; set; }
            public double Price { get; set; }
        }
    }
}
