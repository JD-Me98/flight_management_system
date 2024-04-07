using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace flight_management_system.Pages.Airport
{
    [Authorize]
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Airports airportInfo = new Airports();
        public String errorMessage = "";
        public String successMessage = "";

        public RegisterModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        public void OnGet()
        {
        }

        public void OnPost()
        {

            airportInfo.Id = Request.Form["id"];
            airportInfo.Name = Request.Form["name"];
            airportInfo.Location = Request.Form["location"];
            airportInfo.Image = Request.Form.Files["Image"];

            if (airportInfo.Id.Length == 0 || airportInfo.Name.Length == 0 || airportInfo.Location.Equals(null) 
                || airportInfo.Image == null)
            {
                errorMessage = "All fields are required!";
                return;
            }

            var uniqueFilename = getUniqueImageName(airportInfo.Image.FileName);

            airportInfo.ImageUrl = uniqueFilename;

            var pathToUpload = _environment.WebRootPath + "/uploads/" + uniqueFilename;
            using (var stream = System.IO.File.Create(pathToUpload))
            {
                airportInfo.Image.CopyTo(stream);
            }

            registerAirport();
            
        }
        public void registerAirport()
        {
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "INSERT INTO airport values(@id, @name, @location, @image)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Airports airport = new Airports();
                        cmd.Parameters.AddWithValue("@id", airportInfo.Id);
                        cmd.Parameters.AddWithValue("@name", airportInfo.Name);
                        cmd.Parameters.AddWithValue("@location", airportInfo.Location);
                        cmd.Parameters.AddWithValue("@image", airportInfo.ImageUrl);

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

        public string getUniqueImageName(string filename)
        {
            filename = Path.GetFileName(filename);
            return Guid.NewGuid().ToString().Substring(0, 8) + Path.GetExtension(filename);
        }

        public class Airports
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public IFormFile Image { get; set; }
            public string ImageUrl { get; set; }    
        }
    }
}
