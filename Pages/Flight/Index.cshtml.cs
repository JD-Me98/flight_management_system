using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Flight
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Flight> listFlights = new List<Flight>();

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
                    string sqlQuery = "SELECT * FROM flight;";
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


        public class Flight
        {
            public string Id { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public String Aircraft { get; set; }
        }
    }
}
