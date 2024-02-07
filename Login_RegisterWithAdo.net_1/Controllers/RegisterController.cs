using Login_RegisterWithAdo.net_1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.Pkcs;

namespace Login_RegisterWithAdo.net_1.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger)
        {
            _logger = logger;
        }

        public IActionResult Register()
        {
            ViewBag.hobbieslist = GetHobbieslist();
            return View();
        }
        [HttpPost]
        public IActionResult Register(Register register)
        {
            var dbconfig = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json").Build();
            string dbconnectionStr = dbconfig["ConnectionStrings:ADONET"];
            SqlConnection connection = new SqlConnection(dbconnectionStr);
            {
                string sql = "SpCRUD";
                SqlCommand cmd = new SqlCommand(sql, connection);
                {
                    register.Hobbies = string.Join(", ", register.HobbiesList);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Event", "Add");
                    cmd.Parameters.AddWithValue("@name", register.Name);
                    cmd.Parameters.AddWithValue("@email", register.Email);
                    cmd.Parameters.AddWithValue("@password", register.Password);
                    cmd.Parameters.AddWithValue("@address", register.Address);
                    cmd.Parameters.AddWithValue("@salary", register.Salary);
                    cmd.Parameters.AddWithValue("@hobbies", register.Hobbies);
                    cmd.Parameters.AddWithValue("@about", register.About);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Dispose();
                    return RedirectToAction("Login");
                }
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            var dbconfig = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json").Build();
            string dbconnectionStr = dbconfig["ConnectionStrings:ADONET"];
            SqlConnection connection = new SqlConnection(dbconnectionStr);
            {
                string sql = "SpCRUD";
                SqlCommand cmd = new SqlCommand(sql, connection);
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Event", "login");
                    cmd.Parameters.AddWithValue("@email", login.Email);
                    cmd.Parameters.AddWithValue("@password", login.Password);
                    connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        var claims = new List<Claim>
                         {
                         new Claim(ClaimTypes.NameIdentifier,login.Email),
                         new Claim(ClaimTypes.Email, login.Email),
                         new Claim(ClaimTypes.Name,login.Email)
                         };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties { IsPersistent = true });

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                        return View();
                    }
                    return RedirectToAction("Index");
                }
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.hobbieslist = GetHobbieslist();
            return View();
        }
        [Authorize]
        [HttpPost]
        public JsonResult GetAll()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            //int pageSize = length != null ? Convert.ToInt32(length) : 0;
            //int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;
            int filteredrecords = 0;
            var dbconfig = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json").Build();
            string dbconnectionStr = dbconfig["ConnectionStrings:ADONET"];
            List<Register> Users = new List<Register>();
            using (SqlConnection con = new SqlConnection(dbconnectionStr))
            {
                SqlCommand cmd = new SqlCommand("SpCRUD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Event", "Select");
                cmd.Parameters.AddWithValue("@DisplayLength", length);
                cmd.Parameters.AddWithValue("@DisplayStart", start);
                cmd.Parameters.AddWithValue("@SortCol", sortColumn);
                cmd.Parameters.AddWithValue("@SortDir", sortColumnDirection);
                cmd.Parameters.AddWithValue("@Search", searchValue);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {

                    Register user = new Register();
                    user.Id = Convert.ToInt32(row["Id"]);
                    user.Name = row["Name"].ToString();
                    user.Email = row["Email"].ToString();
                    user.Hobbies = row["Hobbies"].ToString();
                    user.Address = row["Address"].ToString();
                    user.About = row["About"].ToString();
                    user.Salary = row["Salary"].ToString();

                    Users.Add(user);
                    totalRecords = Convert.ToInt32(row["TotalCount"]);
                }
                filteredrecords = Users.Count;
            }
            //var data = Users.Skip(skip).Take(pageSize).ToList();
            var jsondata = new { Draw = draw, iTotalRecords = totalRecords, recordsFiltered = filteredrecords, Data = Users };
            return Json(jsondata);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public List<SelectListItem> GetHobbieslist()
        {
            return new List<SelectListItem>
         {
            new SelectListItem { Value = "Singing", Text = "Singing",},
            new SelectListItem { Value = "Dancing", Text = "Dancing" },
            new SelectListItem { Value = "Reading", Text = "Reading" },
            new SelectListItem { Value = "Playing", Text = "Playing" },
            };
        }
        [HttpGet]
        public IActionResult updatetable(int? id)
        {
            var dbconfig = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json").Build();
            string dbconnectionStr = dbconfig["ConnectionStrings:ADONET"];
            List<Register> Users = new List<Register>();
            SqlConnection connection = new SqlConnection(dbconnectionStr);
            {

                string sql = "SpCRUD";
                SqlCommand cmd = new SqlCommand(sql, connection);
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Event", "edit");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        Register user = new Register();
                        user.Id = Convert.ToInt32(row["Id"]);
                        user.Name = row["Name"].ToString();
                        user.Email = row["Email"].ToString();
                        user.Hobbies = row["Hobbies"].ToString();
                        user.Address = row["Address"].ToString();
                        user.About = row["About"].ToString();
                        user.Salary = row["Salary"].ToString();
                        Users.Add(user);
                    }
                }
            }
            return Json(new { data = Users });
        }
        [HttpPost]
        public IActionResult updatetable(Register register)
        {
            var dbconfig = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json").Build();
            string dbconnectionStr = dbconfig["ConnectionStrings:ADONET"];
            SqlConnection connection = new SqlConnection(dbconnectionStr);
            {
                connection.Open();
                string sql = "SpCRUD";
                SqlCommand cmd = new SqlCommand(sql, connection);
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", register.Id);
                    cmd.Parameters.AddWithValue("@Event", "update");
                    cmd.Parameters.AddWithValue("@Name", register.Name);
                    cmd.Parameters.AddWithValue("@Email", register.Email);
                    cmd.Parameters.AddWithValue("@Hobbies", register.Hobbies);
                    cmd.Parameters.AddWithValue("@address", register.Address);
                    cmd.Parameters.AddWithValue("@About", register.About);
                    cmd.Parameters.AddWithValue("@Salary", register.Salary);
                    cmd.ExecuteNonQuery();
                }
                connection.Dispose();
            }

            return Json(new { data = register });
        }
        [HttpDelete]
        public void Delete(int id)
        {
            var dbconfig = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json").Build();
            string dbconnectionStr = dbconfig["ConnectionStrings:ADONET"];
            List<Register> Users = new List<Register>();
            SqlConnection connection = new SqlConnection(dbconnectionStr);
            {
                string sql = "SpCRUD";
                SqlCommand cmd = new SqlCommand(sql, connection);
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Event", "Delete");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}