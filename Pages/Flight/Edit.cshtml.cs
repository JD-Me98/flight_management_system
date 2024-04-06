using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace flight_management_system.Pages.Flight
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public Flight flightInfo = new Flight();
        public List<Airports> listAirports = new List<Airports>();
        public List<Aircrafts> listAircrafts = new List<Aircrafts>();
        public String errorMessage = "";
        public String successMessage = "";

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            String id = Request.Query["id"];
            getAircrafts();
            getAirports();

            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM flight WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Aircrafts aircraft = new Aircrafts();
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                flightInfo.Id = reader.GetString(0);
                                flightInfo.Departure = reader.GetDateTime(1);
                                flightInfo.Arrival = reader.GetDateTime(2);
                                flightInfo.Origin = reader.GetString(3);
                                flightInfo.Destination = reader.GetString(4);
                                flightInfo.Aircraft = reader.GetString(5);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                errorMessage = ex.Message;
                return;
            }

        }
        public void OnPost() {
            flightInfo.Id = Request.Form["id"];
            flightInfo.Departure = DateTime.Parse(Request.Form["departure"]);
            flightInfo.Arrival = DateTime.Parse(Request.Form["arrival"]);
            flightInfo.Origin = Request.Form["origin"];
            flightInfo.Destination = Request.Form["destination"];
            flightInfo.Aircraft = Request.Form["aircraft"];

            if (flightInfo.Id == "")
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
                    string sqlQuery = "UPDATE flight SET departure=@departure, arrival=@arrival, departure_airport_id=@origin, destination_airport_id=@destination, aircraft_id=@aircraft WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", flightInfo.Id);
                        cmd.Parameters.AddWithValue("@departure", flightInfo.Departure);
                        cmd.Parameters.AddWithValue("@arrival", flightInfo.Arrival);
                        cmd.Parameters.AddWithValue("@origin", flightInfo.Origin);
                        cmd.Parameters.AddWithValue("@destination", flightInfo.Destination);
                        cmd.Parameters.AddWithValue("@aircraft", flightInfo.Aircraft);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Flight Updated";
                            //Response.Redirect("/Flight/Index");
                        }
                        else
                        {
                            errorMessage = "No rows updated. Flight ID may not exist.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Exception: " + ex.Message);
                errorMessage = "SQL Exception: " + ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                errorMessage = "Exception: " + ex.Message;
            }

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
