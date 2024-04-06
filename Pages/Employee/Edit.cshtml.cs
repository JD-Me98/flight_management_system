using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Employee
{
    [Authorize]
    public class EditEmployeeModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Employees employeeInfo { get; set; } = new Employees();
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";

        public EditEmployeeModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            string id = Request.Query["id"];
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT Id, Fullname, DateOfBirth,Position,Availability FROM Employee WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employeeInfo.Id = reader.GetString(0);
                                employeeInfo.Fullname = reader.GetString(1);
                                employeeInfo.DateOfBirth = reader.GetString(2);
                                employeeInfo.Position = reader.GetString(3);
                                employeeInfo.Availability = reader.GetInt32(4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                errorMessage = "An error occurred while fetching employee information.";
            }
        }

        public void OnPost()
        {
            employeeInfo.Id = Request.Form["id"];
            employeeInfo.Fullname = Request.Form["fullname"];
            employeeInfo.DateOfBirth = Request.Form["dateOfBirth"];
            employeeInfo.Position = Request.Form["position"];
            employeeInfo.Availability = int.Parse(Request.Form["availability"]);

            if (employeeInfo.Id.Length == 0 || employeeInfo.Fullname.Length == 0 || employeeInfo.DateOfBirth.Equals(null) || employeeInfo.Position.Length == 0 || employeeInfo.Availability.Equals(null))
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
                    string sqlQuery = "UPDATE employee SET fullname=@fullname, dateOfBirth=@dateOfBirth, position=@position, availability=@availability  WHERE id=@id";
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
                return;
            }
            employeeInfo.Id = ""; employeeInfo.Availability = 0; employeeInfo.Fullname = "";
            employeeInfo.DateOfBirth = ""; employeeInfo.Position = "";
            successMessage = "Employee Updated Successfully";
        }

        public class Employees
        {
            public string Id { get; set; }
            public string Fullname { get; set; }
            public string DateOfBirth { get; set; }
            public string Position { get; set; }
            public int Availability { get; set; }
        }
    }
}
