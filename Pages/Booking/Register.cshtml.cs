using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Booking
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Agencies> listAgencies = new List<Agencies>();
        [BindProperty]
        public Booking bookingInfo { get; set; } = new Booking();

        public String errorMessage = "";
        public String successMessage = "";

        public RegisterModel(IConfiguration configuration)
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
        public void OnPost()
        {
            bookingInfo.Fullname = Request.Form["fullname"];
            bookingInfo.Email = Request.Form["email"];
            bookingInfo.Flight = Request.Form["flight"];
            bookingInfo.FlightClass = Request.Form["flightclass"];
            bookingInfo.trip = Request.Form["trip"];
            bookingInfo.Agency = Request.Form["agecny"];

            //if (!ModelState.IsValid)
            //{
            //    errorMessage = "provide all infromation please";
            //    return;
            //}

            try { 
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using(SqlConnection con =  new SqlConnection(connectionString))
            {
                con.Open();
                string sqlQuery = "";
                    if(string.IsNullOrEmpty(bookingInfo.Agency))
                    {
                        sqlQuery = "INSERT INTO booking (fullname, email, flight_id, class, trip) values(@fullname, @email, @flight, @flightclass, @trip)";
                    }
                    else
                    {
                        sqlQuery = "INSERT INTO booking (fullname, email, flight_id, class, trip, agent_id) values(@fullname, @email, @flight, @flightclass, @trip, @agency)";
                    }
                using(SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    cmd.Parameters.AddWithValue("@fullname", bookingInfo.Fullname);
                    cmd.Parameters.AddWithValue("@email", bookingInfo.Email);
                    cmd.Parameters.AddWithValue("@flight", bookingInfo.Flight);
                    cmd.Parameters.AddWithValue("@flightclass", bookingInfo.FlightClass);
                    cmd.Parameters.AddWithValue("@trip", bookingInfo.trip);

                    if (!string.IsNullOrEmpty(bookingInfo.Agency))
                    {
                        cmd.Parameters.AddWithValue("@agency", bookingInfo.Agency);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            }catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "Your flight has been booked!";
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

        public class Agencies
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
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
        }
    }
}
