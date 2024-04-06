using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Airport
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Airports airportInfo { get; set; } = new Airports();
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";
        public EditModel(IConfiguration configuration)
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
                    string sqlQuery = "SELECT Id, Name, Location FROM airport WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                airportInfo.Id = reader.GetString(0);
                                airportInfo.Name = reader.GetString(1);
                                airportInfo.Location = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                errorMessage = "An error occurred while fetching airport information.";
            }
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
                    string sqlQuery = "UPDATE airport SET name=@name, location=@location WHERE id=@id";
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
                return;
            }
            airportInfo.Id = ""; airportInfo.Name = ""; airportInfo.Location = "";
            successMessage = "Airport Updated Successfully";
        }
    }
    public class Airports
    {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
    }
}

