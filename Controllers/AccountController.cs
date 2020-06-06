using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyLoginApplication.Controllers
{
    public class AccountController : Controller
    {
        readonly SqlConnection con = new SqlConnection();
        readonly SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        void ConnectionString()
        {
            con.ConnectionString = "Data source=DESKTOP-GB8Q486; database=Office; integrated security = SSPI;";
        }

        [HttpPost]
        public ActionResult Login(Models.Membership model)
        {
            ConnectionString();
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "Select * from dbo.Users WHERE Username='"+model.Username +"' AND Password='"+model.Password +"'"; // UNSAFE AGAINST SQL INJECTION

            // cmd.CommandText = "Select * from dbo.Users WHERE Username= @Username AND Password= @Password"; // SAFE 
            // cmd.Parameters.AddWithValue("@Username", model.Username); // SAFE 
            // cmd.Parameters.AddWithValue("@Password", model.Password); // SAFE
            
            dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                FormsAuthentication.SetAuthCookie(model.Username, false);
                return RedirectToAction("index", "Employees");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(Users model)
        {
            using (var context = new OfficeEntities())
            {
                if (model.Username == null && model.Password == null)
                {
                    return View();
                }
                else
                {
                    context.Users.Add(model);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        // [HttpPost]
        // public ActionResult Login(Models.Membership model)
        // {
        //     using (var context = new OfficeEntities())
        //     {
        //         bool isValid = context.Users.Any(x => x.Username == model.Username && x.Password == model.Password);
        //         if (isValid)
        //         {
        //             FormsAuthentication.SetAuthCookie(model.Username, false);
        //             return RedirectToAction("Index", "Employees");
        //         }
        //
        //         ModelState.AddModelError("", "Invalid username and password");
        //         return View();
        //     }
        // }

        public static void ShowSqlException(string connectionString)
        {
            string queryString = "EXECUTE NonExistantStoredProcedure";
            StringBuilder errorMessages = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                                             "Message: " + ex.Errors[i].Message + "\n" +
                                             "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                             "Source: " + ex.Errors[i].Source + "\n" +
                                             "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                }
            }
        }
    }
}