using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Agency
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Agency agencyInfo { get; set; } = new Agency();
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
                    string sqlQuery = "SELECT Id, Name, Location FROM agency WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                agencyInfo.Id = reader.GetString(0);
                                agencyInfo.Name = reader.GetString(1);
                                agencyInfo.Location = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                errorMessage = "An error occurred while fetching agency information.";
            }
        }

        public void OnPost()
        {
            agencyInfo.Id = Request.Form["id"];
            agencyInfo.Name = Request.Form["name"];
            agencyInfo.Location = Request.Form["location"];

            if (agencyInfo.Name.Length == 0 || agencyInfo.Location.Length == 0)
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
                    string sqlQuery = "UPDATE agency SET name=@name, location=@location WHERE id=@id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Agency agency = new Agency();
                        cmd.Parameters.AddWithValue("@id", agencyInfo.Id);
                        cmd.Parameters.AddWithValue("@name", agencyInfo.Name);
                        cmd.Parameters.AddWithValue("@location", agencyInfo.Location);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return;
            }
            agencyInfo.Id = ""; agencyInfo.Name = ""; agencyInfo.Location = "";
            successMessage = "Agency Updated Successfully";
        }
    }

    public class Agency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
   
}
