using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

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
                        cmd.Parameters.AddWithValue("username", accountInfo.username);
                        cmd.Parameters.AddWithValue("password", accountInfo.password);

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

        public class Account
        {
            public string username { get; set; }
            public string password { get; set; }
            public string role { get; set; }
            public string agent_id { get; set; }
        }
    }
}
