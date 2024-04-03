using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace flight_management_system.Pages.Aircraft
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Aircrafts aircraftInfo { get; set; } = new Aircrafts();
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
                    string sqlQuery = "SELECT Id, Type, Capacity FROM aircraft WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                aircraftInfo.Id = reader.GetString(0);
                                aircraftInfo.Type = reader.GetString(1);
                                aircraftInfo.Capacity = reader.GetInt32(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                errorMessage = "An error occurred while fetching aircraft information.";
            }
        }

        public void OnPost()
        {
            aircraftInfo.Id = Request.Form["id"];
            aircraftInfo.Type = Request.Form["type"];
            aircraftInfo.Capacity = int.Parse(Request.Form["capacity"]);

            if (aircraftInfo.Id.Length == 0 || aircraftInfo.Type.Length == 0 || aircraftInfo.Capacity.Equals(null))
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
                    string sqlQuery = "UPDATE aircraft SET type=@type, capacity=@capacity WHERE id=@id";
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
            successMessage = "Aircraft Updated Successfully";
        }
    }

    public class Aircrafts
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
    }
}
