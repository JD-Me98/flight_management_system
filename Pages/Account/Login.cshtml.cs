using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace flight_management_system.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        
        public Account accountInfo = new Account();
        public String errorMessage = "";
        public String successMessage = "";
        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            accountInfo.username = Request.Form["username"];
            accountInfo.password = Request.Form["password"];

            try { 
            string conString = _configuration.GetConnectionString("DefaultConnection");
                using(SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT username, password, role, agent_id FROM account WHERE username = @username AND password = @password";
                    using(SqlCommand cmd = new SqlCommand(sqlQuery,con))
                    {
                        string pass = encrytPass(accountInfo.password);
                        
                        cmd.Parameters.AddWithValue("username", accountInfo.username);
                        cmd.Parameters.AddWithValue("password", pass);

                        cmd.ExecuteNonQuery();

                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Account logedin = new Account();

                            if(reader.Read()) { 
                            logedin.username = reader.GetString(0);
                            logedin.password = reader.GetString(1);
                            logedin.role = reader.GetString(2);
                            logedin.agent_id = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                            }
                            else
                            {
                                errorMessage = "Username or Password Incorrect";
                                return Page();
                            }

                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, logedin.username),
                                new Claim(ClaimTypes.Role, logedin.role),
                                new Claim("agent",logedin.agent_id)
                            };

                            var identity = new ClaimsIdentity(claims, "AuthCookie");
                            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                            await HttpContext.SignInAsync("AuthCookie", claimsPrincipal);

                            return RedirectToPage("/Index");
                        }
                    }
                }
            }catch (Exception ex)
            {
                errorMessage=ex.Message;
                return Page();
            }
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
            public string role { get; set; }
            public string agent_id { get; set; }
        }
    }
}
