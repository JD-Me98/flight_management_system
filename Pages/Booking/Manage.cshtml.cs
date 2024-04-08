using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

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
                string sqlQuery = "SELECT * FROM booking";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        listBookings.Clear();
                        Booking booking = new Booking();
                        while (reader.Read())
                        {
                            booking.Id = reader.GetInt32(0);
                            booking.Fullname = reader.GetString(1);
                            booking.Email = reader.GetString(2);
                            booking.Flight = reader.GetString(3);
                            booking.FlightClass = reader.GetString(4);
                            booking.trip = reader.GetString(5);
                            booking.Agency = reader.GetString(6);
                            booking.Created_at = DateTime.Parse(reader.GetString(7));

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
            public DateTime Created_at {  get; set; }
        }
    }
}
