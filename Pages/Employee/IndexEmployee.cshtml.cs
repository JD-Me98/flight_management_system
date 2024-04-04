using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Employee
{
    public class IndexEmployeeModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Employees> listEmployees = new List<Employees>();

        public IndexEmployeeModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            listEmployees.Clear();
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
                                listEmployees.Add(employee);
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
    }
}
