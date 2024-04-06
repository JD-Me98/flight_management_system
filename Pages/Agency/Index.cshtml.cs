using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Agency
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Agencies> listAgencies = new List<Agencies>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            listAgencies.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT id, name, location FROM agency;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Agencies agency = new Agencies
                                {
                                    Id = reader.GetString(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("name")),
                                    Location = reader.GetString(reader.GetOrdinal("location"))
                                };
                                listAgencies.Add(agency);
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


        public class Agencies
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }

    }

}


