using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace flight_management_system.Pages.Agents
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public Agent AgentInfo { get; set; } = new Agent();
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

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
                    string sqlQuery = "SELECT Id, Agency, Username FROM Agents WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AgentInfo.Id = reader.GetString(0);
                                AgentInfo.Agency = reader.GetString(1);
                                AgentInfo.Username = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while fetching agent information: " + ex.Message;
            }
        }

        public void OnPost()
        {
            AgentInfo.Id = Request.Form["id"];
            AgentInfo.Agency = Request.Form["agency"];
            AgentInfo.Username = Request.Form["username"];

            if (string.IsNullOrEmpty(AgentInfo.Id) || string.IsNullOrEmpty(AgentInfo.Agency) || string.IsNullOrEmpty(AgentInfo.Username))
            {
                ErrorMessage = "All fields are required!";
                return;
            }

            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "UPDATE Agents SET Agency=@agency, Username=@username WHERE Id=@id";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", AgentInfo.Id);
                        cmd.Parameters.AddWithValue("@agency", AgentInfo.Agency);
                        cmd.Parameters.AddWithValue("@username", AgentInfo.Username);

                        cmd.ExecuteNonQuery();
                    }
                }
                SuccessMessage = "Agent Updated Successfully";
                AgentInfo = new Agent(); // Reset form after successful update
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred: " + ex.Message;
            }
        }

        public class Agent
        {
            public string Id { get; set; }
            public string Agency { get; set; }
            public string Username { get; set; }
        }
    }
}
