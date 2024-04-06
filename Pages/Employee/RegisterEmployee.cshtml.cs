using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Employee
{
    [Authorize]
    public class RegisterEmployeeModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Employees employeeInfo = new Employees();
        public String errorMessage = "";
        public String successMessage = "";

        public RegisterEmployeeModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            employeeInfo.Id = Request.Form["id"];
            employeeInfo.Fullname = Request.Form["fullname"];
            employeeInfo.DateOfBirth = Request.Form["dateOfBirth"];
            employeeInfo.Position = Request.Form["position"];
            employeeInfo.Availability = int.Parse(Request.Form["availability"]);

            if (employeeInfo.Id.Length == 0 || employeeInfo.Fullname.Length == 0 || employeeInfo.DateOfBirth.Equals(null) ||employeeInfo.Position.Length == 0 || employeeInfo.Availability.Equals(null))
            {
                errorMessage = "All fields are required!";
                return;
            }

            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "INSERT INTO employee values(@id, @fullname, @dateOfBirth, @position, @availability)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Employees employee = new Employees();
                        cmd.Parameters.AddWithValue("@id", employeeInfo.Id);
                        cmd.Parameters.AddWithValue("@fullname", employeeInfo.Fullname);
                        cmd.Parameters.AddWithValue("@dateOfBirth", employeeInfo.DateOfBirth);
                        cmd.Parameters.AddWithValue("@position", employeeInfo.Position);
                        cmd.Parameters.AddWithValue("@availability", employeeInfo.Availability);

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
            employeeInfo.Id = ""; employeeInfo.Fullname = ""; employeeInfo.DateOfBirth = null ;
            employeeInfo.Position = ""; employeeInfo.Availability = 0;
            successMessage = "New Employee Registered";
        }

        public class Employees
        {
            public string Id { get; set; }
            public string Fullname { get; set; }
            public string? DateOfBirth { get; set; }

            public string Position { get; set; }
            public int Availability { get; set; }
        }
    }
}
