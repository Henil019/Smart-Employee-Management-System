using EmployeeManagementSystem.Models;  
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["EMS"].ConnectionString;

        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin a)
        {
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Admin where Username=@un and Password=@pwd";
            SqlCommand cmd= new SqlCommand(qry, conn);

            cmd.Parameters.AddWithValue("@un", a.Username);
            cmd.Parameters.AddWithValue("@pwd", a.Password);

            conn.Open();

            SqlDataReader drd=cmd.ExecuteReader();
            if(drd.Read()) 
            {
                Session["Admin"] = a.Username;
                Session["Role"] = "Admin";
                return RedirectToAction("Dashboard", "Employee");
            }
            drd.Close();

            // 2. If not an Admin, check if it's an Employee/HR
            string empQry = "Select * from Employee where Email=@un and Password=@pwd";
            SqlCommand empCmd = new SqlCommand(empQry, conn);
            empCmd.Parameters.AddWithValue("@un", a.Username);
            empCmd.Parameters.AddWithValue("@pwd", a.Password);

            SqlDataReader empDrd = empCmd.ExecuteReader();
            if (empDrd.Read())
            {
                bool isHR = empDrd["IsHR"] != DBNull.Value && Convert.ToBoolean(empDrd["IsHR"]);
                
                Session["Admin"] = a.Username; // Keep this as the generic "is logged in" token
                Session["Role"] = isHR ? "HR" : "Employee";
                Session["EmployeeId"] = empDrd["Id"];
                Session["DepartmentId"] = empDrd["DepartmentId"];

                return RedirectToAction("Dashboard", "Employee");
            }

            ViewBag.Message = "Invalid Username or Password";
            conn.Close();
            return View();

        }
        public ActionResult logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
}}
