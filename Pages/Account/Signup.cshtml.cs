using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static flight_management_system.Pages.Agency.IndexModel;

namespace flight_management_system.Pages.Account
{
    public class SignupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<Agencies> listAgencies = new List<Agencies>();

        public Account accountInfo = new Account();

        public String errorMessage = "";
        public String successMessage = "";
        public SignupModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void OnGet()
        {
            listAgencies.Clear();
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT id, name, location FROM agency;";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Agencies agency = new Agencies
                                {
                                    Id = reader.GetString(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("name")),
                                    Location = reader.GetString(reader.GetOrdinal("location"))
                                };
                                listAgencies.Add(agency);
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
        public void OnPost()
        {
            accountInfo.username = Request.Form["username"];
            accountInfo.password = Request.Form["password"];
            accountInfo.confirmPass = Request.Form["confirmPass"];
            accountInfo.Role = Request.Form["Role"];
            accountInfo.Agency = Request.Form["agency"];

            if (accountInfo.username == "" || accountInfo.password == "" ||
               accountInfo.confirmPass == "")
            {
                errorMessage = "Fill all the fields please!";
                Response.Redirect("/Account/Signup");
            }
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");

                string sqlQuery = "";
                if (accountInfo.Agency == "")
                {
                    sqlQuery = "INSERT INTO account (username, password, role) VALUES (@username, @password, @role)";
                }
                else
                {
                    sqlQuery = "INSERT INTO account (username, password, role, agent_id) VALUES (@username, @password, @role, @agency)";
                }

                if (accountInfo.password == accountInfo.confirmPass)
                {
                    string pass = encrytPass(accountInfo.password);
                    using(SqlConnection con = new SqlConnection(conString))
                    {
                        con.Open();
                        using(SqlCommand cmd = new SqlCommand(sqlQuery, con))
                        {
                            cmd.Parameters.AddWithValue ("@username", accountInfo.username);
                            cmd.Parameters.AddWithValue ("@password", pass);
                            cmd.Parameters.AddWithValue("@role", accountInfo.Role);
                            cmd.Parameters.AddWithValue("@agency", accountInfo.Agency);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    errorMessage = "your passwords do not match";
                    Response.Redirect("/Account/Signup");
                }


            } catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "account created!";
            //Response.Redirect("/Account/Login");
        } 

        private String encrytPass(String password)
        {
            //Hash computation
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public class Account
        {
            public string username { get; set; }
            public string password { get; set; }
            public string confirmPass { get; set; }
            public string Role { get; set; }
            public string Agency { get; set; }
        }

        public class Agency
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
        }
    }
}
