using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class RoleController : Controller
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

            List<Role> list = new List<Role>();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Role";
            SqlCommand cmd = new SqlCommand(qry, conn);
            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            while (drd.Read())
            {
                Role r = new Role();
                r.Id = (int)drd["Id"];
                r.Title = drd["Title"].ToString();
                list.Add(r);
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
        public ActionResult Create(Role r)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                string qry = "insert into Role(Title) values(@title)";
                SqlCommand cmd = new SqlCommand(qry, con);

                cmd.Parameters.AddWithValue("@title", r.Title);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                
                return RedirectToAction("Index");
            }
            return View(r);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            Role r = new Role();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Role where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            if (drd.Read())
            {
                r.Id = (int)drd["Id"];
                r.Title = drd["Title"].ToString();
            }
            conn.Close();
            return View(r);
        }

        [HttpPost]
        public ActionResult Edit(Role r)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            if (!HasAccess()) return RedirectToAction("Dashboard", "Employee");

            SqlConnection conn = new SqlConnection(cs);
            string qry = "update Role set Title=@title where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);

            cmd.Parameters.AddWithValue("@id", r.Id);
            cmd.Parameters.AddWithValue("@title", r.Title ?? (object)DBNull.Value);
            
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
            string qry = "Delete from Role where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            
            return RedirectToAction("Index");
        }
    }
}
