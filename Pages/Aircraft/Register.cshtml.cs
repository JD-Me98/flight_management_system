using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using static flight_management_system.Pages.Aircraft.IndexModel;

namespace flight_management_system.Pages.Aircraft
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Aircrafts aircraftInfo = new Aircrafts();
        public String errorMessage = "";
        public String successMessage = "";

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
        }
        public void OnPost() {
            aircraftInfo.Id = Request.Form["id"];
            aircraftInfo.Type = Request.Form["type"];
            aircraftInfo.Capacity = int.Parse(Request.Form["capacity"]);

            if(aircraftInfo.Id.Length == 0 || aircraftInfo.Type.Length == 0 || aircraftInfo.Capacity.Equals(null))
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
                    string sqlQuery = "INSERT INTO aircraft values(@id, @type, @capacity)";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        Aircrafts aircraft = new Aircrafts();
                        cmd.Parameters.AddWithValue("@id", aircraftInfo.Id);
                        cmd.Parameters.AddWithValue("@type", aircraftInfo.Type);
                        cmd.Parameters.AddWithValue("@capacity", aircraftInfo.Capacity);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return;
            }
            aircraftInfo.Id = ""; aircraftInfo.Capacity = 0; aircraftInfo.Type = "";
            successMessage = "New Aircraft Registered";
        }
        public class Aircrafts
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public int Capacity { get; set; }
        }
    }
}
