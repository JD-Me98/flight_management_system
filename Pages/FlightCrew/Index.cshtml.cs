using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.FlightCrew
{    public class IndexModel : PageModel

    {
        private readonly IConfiguration _configuration;
        public List<FlightCrew> listFlightCrews = new List<FlightCrew>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            listFlightCrews.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    // SELECT fc.id, e.fullname, fc.flight_id, fc.role FROM flightCrews AS fc JOIN employee AS e ON fc.employee_id = e.id;
                    string sqlQuery = "SELECT fc.id, e.fullname, fc.flight_id, fc.role FROM flightCrews AS fc JOIN employee AS e ON fc.employee_id = e.id;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FlightCrew flightCrew = new FlightCrew();
                                flightCrew.Id = reader.GetString(0);
                                flightCrew.Employee = reader.GetString(1);
                                flightCrew.Flight = reader.GetString(2);
                                flightCrew.Role = reader.GetString(3);
                                

                                listFlightCrews.Add(flightCrew);
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
        public class FlightCrew
        {
            public string Id { get; set; }
            public String Employee { get; set; }
            public String Flight { get; set; }
            public String Role { get; set; }
           
        }
    }
}
