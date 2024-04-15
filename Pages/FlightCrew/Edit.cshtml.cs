using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace flight_management_system.Pages.FlightCrew
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public FlightCrew flightCrewInfo = new FlightCrew();
        public List<Employees> listEmployee = new List<Employees>();
        public List<Flight> listFlight = new List<Flight>();
        public String errorMessage = "";
        public String successMessage = "";

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            string id = Request.Query["id"];
            getFlightCrewDetails(id);
            getFlight();

            // Load employee list (optional)
            getEmployees();
        }

        private void getFlightCrewDetails(string id)
        {
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM FlightCrews WHERE Id=@id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                flightCrewInfo = new FlightCrew
                                {
                                    Id = reader.GetString(0),
                                    Employee = reader.GetString(1),
                                    Flight = reader.GetString(2),
                                    Role = reader.GetString(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            flightCrewInfo.Id = Request.Form["id"];
            flightCrewInfo.Employee = Request.Form["employee"];
            flightCrewInfo.Flight = Request.Form["flight"];
            flightCrewInfo.Role = Request.Form["role"];

            if (flightCrewInfo.Id == "")
            {
                errorMessage = "Please provide all details.";
                return;
            }

            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "UPDATE FlightCrews SET employee_id=@employee, flight_id=@flight, role=@role WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", flightCrewInfo.Id);
                        cmd.Parameters.AddWithValue("@employee", flightCrewInfo.Employee);
                        cmd.Parameters.AddWithValue("@flight", flightCrewInfo.Flight);
                        cmd.Parameters.AddWithValue("@role", flightCrewInfo.Role);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Flight Crew Updated";

                        }
                        else
                        {
                            errorMessage = "No rows updated. Flight Crew ID may not exist.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                errorMessage = "SQL Exception: " + ex.Message;
            }
            catch (Exception ex)
            {
                errorMessage = "Exception: " + ex.Message;
            }
            Response.Redirect("/FlightCrew/Index");
        }


        public void getEmployees()
        {
            listEmployee.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Id, Fullname, DateOfBirth, Position, Availability FROM Employee;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employees employee = new Employees();
                                employee.Id = reader.GetString(0);
                                employee.Fullname = reader.GetString(1);
                                employee.DateOfBirth = reader.GetString(2);
                                employee.Position = reader.GetString(3);
                                employee.Availability = reader.GetInt32(4);
                                listEmployee.Add(employee);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception while fetching employees: " + ex.Message;
            }
        }

        public void getFlight()
        {
            listFlight.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Id, Departure, Arrival, departure_airport_id, destination_airport_id, aircraft_id FROM Flight;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Flight flight = new Flight();
                                flight.Id = reader.GetString(0);
                                flight.Departure = reader.GetDateTime(1);
                                flight.Arrival = reader.GetDateTime(2);
                                // Modify the below lines to fetch correct values from the reader
                                flight.DepartureAirportId = reader.GetString(3);
                                flight.DestinationAirportId = reader.GetString(4);
                                flight.AircraftId = reader.GetString(5);

                                listFlight.Add(flight);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Exception while fetching flights: " + ex.Message;
            }
        }

        public class Employees
        {
            public string Id { get; set; }
            public string Fullname { get; set; }
            public string DateOfBirth { get; set; }
            public string Position { get; set; }
            public int Availability { get; set; }
        }

        public class Flight
        {
            public string Id { get; set; }
            public DateTime Departure { get; set; }
            public DateTime Arrival { get; set; }
            public string DepartureAirportId { get; set; }
            public string DestinationAirportId { get; set; }
            public String AircraftId { get; set; }  
        }


        public class FlightCrew
        {
            public string Id { get; set; }
            public String Employee { get; set; }
            public String Flight { get; set; }
            public string Role { get; set; }
        }

    }
}