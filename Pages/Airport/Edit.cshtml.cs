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
        private readonly IWebHostEnvironment _environment;
        [BindProperty]
        public Airports airportInfo { get; set; } = new Airports();

        [BindProperty]
        public IFormFile NewImage { get; set; }
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";
        public EditModel(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
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
                    string sqlQuery = "SELECT Id, Name, Location, image FROM airport WHERE Id = @id";
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
                                airportInfo.ImageUrl = reader.GetString(3);
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
            airportInfo.ImageUrl = Request.Form["imageurl"];
            airportInfo.Image = Request.Form.Files["newImage"];

            if (airportInfo.Id.Length == 0 || airportInfo.Name.Length == 0 || airportInfo.Location.Equals(null))
            {
                errorMessage = "All fields are required!";
                return;
            }

            if (NewImage != null && NewImage.Length > 0)
            {
 
                var uniqueFilename = getUniqueImageName(airportInfo.Image.FileName);

                airportInfo.ImageUrl = uniqueFilename;

                var pathToUpload = _environment.WebRootPath + "/uploads/" + uniqueFilename;
                using (var stream = System.IO.File.Create(pathToUpload))
                {
                    airportInfo.Image.CopyTo(stream);
                }

                try
                {
                    string conString = _configuration.GetConnectionString("DefaultConnection");
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        con.Open();
                        string sqlQuery = "UPDATE airport SET name=@name, location=@location, image=@imageurl WHERE id=@id";
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                        {
                            Airports airport = new Airports();
                            cmd.Parameters.AddWithValue("@id", airportInfo.Id);
                            cmd.Parameters.AddWithValue("@name", airportInfo.Name);
                            cmd.Parameters.AddWithValue("@location", airportInfo.Location);
                            cmd.Parameters.AddWithValue("@imageurl", airportInfo.ImageUrl);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    successMessage = "Airport Updated Successfully";
                }
                catch (Exception ex)
                {
                    errorMessage= ex.Message;
                    return;
                }                
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
                successMessage = "Airport Updated Successfully";
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message;
                return;
            }
            airportInfo.Id = ""; airportInfo.Name = ""; airportInfo.Location = "";
            
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

