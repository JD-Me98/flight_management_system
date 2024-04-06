using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Agency
{
    [Authorize]
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Agency agencyInfo = new Agency();
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
            agencyInfo.Id = Request.Form["id"];
            agencyInfo.Name = Request.Form["name"];
            agencyInfo.Location = Request.Form["location"];

            if (agencyInfo.Id.Length == 0 || agencyInfo.Name.Length == 0 || agencyInfo.Location.Length ==0)
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
                    string sqlQuery = "INSERT INTO agency values(@id, @name, @location)";
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
                errorMessage = ex.Message;
                return;
            }
            agencyInfo.Id = ""; agencyInfo.Location = ""; agencyInfo.Name = "";
            successMessage = "New Aircraft Registered";
        }
        public class Agency
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }
    }
   
}
