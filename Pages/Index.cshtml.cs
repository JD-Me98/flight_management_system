using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public List<Destination> listDestinations = new List<Destination>();
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnGet()
        {
            listDestinations.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT A.name, A.image, A.location, F.destination_airport_id, COUNT(*) AS Visits " +
                        "FROM booking B " +
                        "JOIN flight F ON B.flight_id = F.id " +
                        "JOIN airport A ON F.destination_airport_id = A.id " +
                        "GROUP BY A.name, A.image, A.location, F.destination_airport_id " +
                        "ORDER BY Visits DESC;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            listDestinations.Clear();
                            while (reader.Read())
                            {
                                Destination destination = new Destination();
                                destination.Name = reader.GetString(reader.GetOrdinal("name"));
                                destination.image = reader.GetString(reader.GetOrdinal("image"));
                                destination.Location = reader.GetString(reader.GetOrdinal("location"));
                                destination.id = reader.GetString(reader.GetOrdinal("destination_airport_id"));
                                listDestinations.Add(destination);
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
        public class Destination
        {
            public string Name { get; set; }
            public string image { get; set; }
            public string Location { get; set; }
            public string id { get; set; }
        }
    }
}
