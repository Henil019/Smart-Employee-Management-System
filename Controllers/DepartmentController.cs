using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["EMS"].ConnectionString;

        private bool HasAccess()
        {
            return Session["Role"] as string == "Admin";
        }

        public ActionResult Index()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            List<Department> list = new List<Department>();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Department";
            SqlCommand cmd = new SqlCommand(qry, conn);
            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            while (drd.Read())
            {
                Department d = new Department();
                d.Id = (int)drd["Id"];
                d.Name = drd["Name"].ToString();
                list.Add(d);
            }
            conn.Close();
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Department d)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                string qry = "insert into Department(Name) values(@name)";
                SqlCommand cmd = new SqlCommand(qry, con);

                cmd.Parameters.AddWithValue("@name", d.Name ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                
                return RedirectToAction("Index");
            }
            return View(d);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            Department d = new Department();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Department where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            if (drd.Read())
            {
                d.Id = (int)drd["Id"];
                d.Name = drd["Name"].ToString();
            }
            conn.Close();
            return View(d);
        }

        [HttpPost]
        public ActionResult Edit(Department d)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            SqlConnection conn = new SqlConnection(cs);
            string qry = "update Department set Name=@name where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);

            cmd.Parameters.AddWithValue("@id", d.Id);
            cmd.Parameters.AddWithValue("@name", d.Name ?? (object)DBNull.Value);
            
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            SqlConnection conn = new SqlConnection(cs);
            string qry = "Delete from Department where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            conn.Open();

            cmd.ExecuteNonQuery();
            conn.Close();
            
            return RedirectToAction("Index");
        }
    }
}
