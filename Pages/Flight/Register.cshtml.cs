using flight_management_system.Pages.Airport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace flight_management_system.Pages.Flight
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public Flight flightInfo = new Flight();
        public List<Airports> listAirports = new List<Airports>();
        public List<Aircrafts> listAircrafts = new List<Aircrafts>();
        public String errorMessage = "";
        public String successMessage = "";

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            getAircrafts();
            getAirports();

        }
        public void OnPost()
        {
            flightInfo.Id = Request.Form["id"];
            flightInfo.Departure = DateTime.Parse(Request.Form["departure"]);
            flightInfo.Arrival = DateTime.Parse(Request.Form["arrival"]);
            flightInfo.Origin = Request.Form["origin"];
            flightInfo.Destination = Request.Form["destination"];
            flightInfo.Aircraft = Request.Form["aircraft"];

            if(flightInfo.Id == "")
            {
                errorMessage = "Provide all details";
                return;
            }

            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "INSERT INTO flight values(@id, @departure, @arrival, @origin, @destination, @aircraft)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Aircrafts aircraft = new Aircrafts();
                        cmd.Parameters.AddWithValue("@id", flightInfo.Id);
                        cmd.Parameters.AddWithValue("@departure", flightInfo.Departure);
                        cmd.Parameters.AddWithValue("@arrival", flightInfo.Arrival);
                        cmd.Parameters.AddWithValue("@origin", flightInfo.Origin);
                        cmd.Parameters.AddWithValue("@destination", flightInfo.Destination);
                        cmd.Parameters.AddWithValue("@aircraft", flightInfo.Aircraft);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                errorMessage = ex.Message;
                return;
            }
            successMessage = "New Aircraft Registered";
            Response.Redirect("/Flight/Index");
        }

        private void getAirports()
        {
            listAirports.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Id, Name, Location FROM airport;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Airports airport = new Airports();
                                airport.Id = reader.GetString(0);
                                airport.Name = reader.GetString(1);
                                airport.Location = reader.GetString(2);
                                
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

        private void getAircrafts()
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
                                Aircrafts aircraft = new Aircrafts();
                                aircraft.Id = reader.GetString(0);
                                    aircraft.Type = reader.GetString(1);
                                    aircraft.Capacity = reader.GetInt32(2);
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

        public class Flight
        {
            public string Id { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public string Origin { get; set; }
            public string Destination { get; set; }
            public String Aircraft { get; set; }
        }

        public class Airports
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }

        public class Aircrafts
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public int Capacity { get; set; }
        }
    }
}
