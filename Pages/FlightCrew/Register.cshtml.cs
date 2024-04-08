using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace flight_management_system.Pages.FlightCrew
{
    public class RegisterModel : PageModel
    {
        public FlightCrew flightCrewInfo = new FlightCrew();
        public List<Employees> listEmployee = new List<Employees>();
        public List<Flight> listFlight = new List<Flight>();
        public string errorMessage = "";

        public string successMessage = "";
        private readonly IConfiguration _configuration;

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            getEmployees();
            getFlight();
        }
        public void OnPost()
        {

            flightCrewInfo.Id = Request.Form["id"];
            flightCrewInfo.Employee = Request.Form["employee"];
            flightCrewInfo.Flight = Request.Form["flight"];
            flightCrewInfo.Role = Request.Form["role"];

            if (flightCrewInfo.Id == "")
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
                    string sqlQuery = "INSERT INTO FlightCrews values(@id, @employee_id, @flight_id, @role)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        FlightCrew flightCrew = new FlightCrew();
                        cmd.Parameters.AddWithValue("@id", flightCrewInfo.Id);
                        cmd.Parameters.AddWithValue("@employee_id", flightCrewInfo.Employee);
                        cmd.Parameters.AddWithValue("@flight_id", flightCrewInfo.Flight);
                        cmd.Parameters.AddWithValue("@role", flightCrewInfo.Role);


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
            successMessage = "New Flight Crew Registered";
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
                    string sqlQuery = "SELECT Id, Fullname, DateOfBirth,Position,Availability FROM Employee;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Employees employee = new Employees
                                {
                                    Id = reader.GetString(reader.GetOrdinal("Id")),
                                    Fullname = reader.GetString(reader.GetOrdinal("Fullname")),
                                    DateOfBirth = reader.GetString(reader.GetOrdinal("DateOfBirth")),
                                    Position = reader.GetString(reader.GetOrdinal("Position")),
                                    Availability = reader.GetInt32(reader.GetOrdinal("Availability"))
                                };
                                listEmployee.Add(employee);
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

        public void getFlight()
        {
            listFlight.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM flight;";
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
                                flight.Origin = reader.GetString(3);
                                flight.Destination = reader.GetString(4);
                                flight.Aircraft = reader.GetString(5);

                                listFlight.Add(flight);
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
            public string Origin { get; set; }
            public string Destination { get; set; }
            public String Aircraft { get; set; }
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
