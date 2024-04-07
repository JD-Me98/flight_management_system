using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Xml;

namespace flight_management_system.Pages.Airport
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Airports> listAirports = new List<Airports>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            listAirports.Clear();
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
                                Airports airport = new Airports
                                {
                                    Id = reader.GetString(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Location = reader.GetString(reader.GetOrdinal("Location")),
                                    Image = reader.GetString(reader.GetOrdinal("Image"))
                                };
                                listAirports.Add(airport);
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
        public class Airports
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public string Image { get; set; }
        }
    }
}
