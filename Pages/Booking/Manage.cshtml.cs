using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Booking
{
    public class ManageModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Booking> listBookings = new List<Booking>();

        public String errorMessage = "";
        public String successMessage = "";

        public ManageModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sqlQuery = "";
                if(User.Identity.IsAuthenticated && User.IsInRole("agent"))
                {
                    var agencyClaim = User.FindFirst("agent");
                    string agencyValue = agencyClaim != null ? agencyClaim.Value : "";

                    sqlQuery = "SELECT * FROM booking WHERE agent_id = " + "'"+agencyValue+"'";
                }
                else
                {
                    sqlQuery = "SELECT * FROM booking";
                }
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        listBookings.Clear();                        

                        while (reader.Read())
                        {
                            Booking booking = new Booking();
                            booking.Id = reader.GetInt32(reader.GetOrdinal("id"));
                            booking.Fullname = reader.GetString(reader.GetOrdinal("fullname"));
                            booking.Email = reader.GetString(reader.GetOrdinal("email"));
                            booking.Flight = reader.GetString(reader.GetOrdinal("flight_id"));
                            booking.FlightClass = reader.GetString(reader.GetOrdinal("class"));
                            booking.trip = reader.GetString(reader.GetOrdinal("trip"));
                            if (!reader.IsDBNull(reader.GetOrdinal("agent_id")))
                            {
                                booking.Agency = reader.GetString(reader.GetOrdinal("agent_id"));
                            }
                            else
                            {                                
                                booking.Agency = "No Agency";
                            }
                            booking.Created_at = reader.GetDateTime(reader.GetOrdinal("created_at"));

                            listBookings.Add(booking);
                        }
                    }
                }
            }
        }

        public class Booking
        {
            public int Id { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Flight { get; set; }
            public string FlightClass { get; set; }
            public string trip { get; set; }
            public string Agency { get; set; }
            public DateTime Created_at { get; set; }
        }

        public class Agencies
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }

    }
}
