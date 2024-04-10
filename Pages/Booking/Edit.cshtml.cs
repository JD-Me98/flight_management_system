using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Booking
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Agencies> listAgencies = new List<Agencies>();

        [BindProperty]
        public Booking bookingInfo { get; set; } = new Booking();

        public bool IsRoundTripSelected => bookingInfo.trip == "Round";
        public bool IsOneWayTripSelected => bookingInfo.trip == "One-Way";

        public string errorMessage = "";
        public string successMessage = "";

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            string id = Request.Query["id"];
            try
            {
                string sqlConnection = _configuration.GetConnectionString("DefaultConnection");
                using(SqlConnection con = new SqlConnection(sqlConnection))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM booking WHERE id = @id";
                    using(SqlCommand cmd = new SqlCommand(sqlQuery, con)) { 
                        cmd.Parameters.AddWithValue("id", id);

                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                bookingInfo.Id = reader.GetInt32(0);
                                bookingInfo.Fullname = reader.GetString(1);
                                bookingInfo.Email= reader.GetString(2);
                                bookingInfo.Flight= reader.GetString(3);
                                bookingInfo.FlightClass= reader.GetString(4);
                                bookingInfo.trip = reader.GetString(5);
                                if(!reader.IsDBNull(reader.GetOrdinal("agent_id")))
                                {
                                    bookingInfo.Agency = reader.GetString(6);
                                }
                                
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            
        }
        public void OnPost()
        {
            bookingInfo.Fullname = Request.Form["fullname"];
            bookingInfo.Email = Request.Form["email"];
            bookingInfo.Flight = Request.Form["flight"];
            bookingInfo.FlightClass = Request.Form["flightclass"];
            bookingInfo.trip = Request.Form["trip"];
            bookingInfo.Agency = Request.Form["agency"];

            //if (!ModelState.IsValid)
            //{
            //    errorMessage = "provide all infromation please";
            //    return;
            //}

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sqlQuery = "UPDATE booking SET fullname = @fullname, email = @email, class = @flightclass, trip = @trip WHERE id = @id";
                    
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", bookingInfo.Id);
                        cmd.Parameters.AddWithValue("@fullname", bookingInfo.Fullname);
                        cmd.Parameters.AddWithValue("@email", bookingInfo.Email);
                        cmd.Parameters.AddWithValue("@flightclass", bookingInfo.FlightClass);
                        cmd.Parameters.AddWithValue("@trip", bookingInfo.trip);


                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "Your booking has been Updated!";
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
