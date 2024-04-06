using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace flight_management_system.Pages.Aircraft
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Aircrafts> listAircrafts = new List<Aircrafts>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            listAircrafts.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Id, Type, Capacity FROM aircraft;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Aircrafts aircraft = new Aircrafts
                                {
                                    Id = reader.GetString(reader.GetOrdinal("Id")),
                                    Type = reader.GetString(reader.GetOrdinal("Type")),
                                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity"))
                                };
                                listAircrafts.Add(aircraft);
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


        public class Aircrafts
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public int Capacity { get; set; }
        }

    }
}
