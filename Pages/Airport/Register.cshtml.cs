using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Airport
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Airports airportInfo = new Airports();
        public String errorMessage = "";
        public String successMessage = "";

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {
            airportInfo.Id = Request.Form["id"];
            airportInfo.Name = Request.Form["name"];
            airportInfo.Location = Request.Form["location"];

            if (airportInfo.Id.Length == 0 || airportInfo.Name.Length == 0 || airportInfo.Location.Equals(null))
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
                    string sqlQuery = "INSERT INTO airport values(@id, @name, @location)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Airports airport = new Airports();
                        cmd.Parameters.AddWithValue("@id", airportInfo.Id);
                        cmd.Parameters.AddWithValue("@name", airportInfo.Name);
                        cmd.Parameters.AddWithValue("@location", airportInfo.Location);

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
            airportInfo.Id = ""; airportInfo.Location = ""; airportInfo.Name = "";
            successMessage = "New Airport Registered";
        }

        public class Airports
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }
    }
}
