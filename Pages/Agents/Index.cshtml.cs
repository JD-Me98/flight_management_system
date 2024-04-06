using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Agents
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Agents> listAgents = new List<Agents>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            listAgents.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM Agents;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Agents agent = new Agents
                                {
                                    id = reader.GetString(reader.GetOrdinal("id")),
                                    agency = reader.GetString(reader.GetOrdinal("agency")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                    password = reader.GetString(reader.GetOrdinal("password"))
                                };
                                listAgents.Add(agent);
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
        public class Agents
        {
            public string id { get; set; }
            public string agency { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }
    }
    
}


