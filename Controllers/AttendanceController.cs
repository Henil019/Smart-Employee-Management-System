using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class AttendanceController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["EMS"].ConnectionString;

        // GET: Attendance
        public ActionResult Index()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");

            string role = Session["Role"] as string;

            List<Attendance> list = new List<Attendance>();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select A.Id, E.FirstName, E.LastName, A.AttendanceDate, A.Status from Attendance2 A INNER JOIN Employee e ON A.EmployeeId=E.Id";
            
            if (role == "HR" && Session["DepartmentId"] != null)
            {
                qry += " WHERE e.DepartmentId = " + Session["DepartmentId"];
            }
            else if (role == "Employee" && Session["EmployeeId"] != null)
            {
                qry += " WHERE e.Id = " + Session["EmployeeId"];
            }

            SqlCommand cmd = new SqlCommand(qry, conn);
            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            while (drd.Read())
            {
                Attendance a = new Attendance();
                a.Id = (int)drd["Id"];
                a.AttendanceDate = (DateTime)drd["AttendanceDate"];
                a.Status = (string)drd["Status"];
                a.FirstName = drd["FirstName"].ToString();
                a.LastName = drd["LastName"].ToString();

                list.Add(a);
            }
            conn.Close();

            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Index"); // Basic employees shouldn't log own attendance, or maybe they do? We'll let HR/Admin do it for now to match prompt

            return View();
        }

        [HttpPost]
        public ActionResult Create(Attendance a)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                string qry = "insert into Attendance2(EmployeeId, AttendanceDate, Status) values(@empId, @date, @status)";
                SqlCommand cmd = new SqlCommand(qry, con);

                cmd.Parameters.AddWithValue("@empId", a.EmployeeId);
                cmd.Parameters.AddWithValue("@date", a.AttendanceDate);
                cmd.Parameters.AddWithValue("@status", a.Status ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                
                return RedirectToAction("Index");
            }
            
            return View(a);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Index");

            Attendance a = new Attendance();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Attendance2 where Id=@id";

            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            if (drd.Read())
            {
                a.Id = (int)drd["Id"];
                a.EmployeeId = (int)drd["EmployeeId"];
                a.AttendanceDate = (DateTime)drd["AttendanceDate"];
                a.Status = (string)drd["Status"];
            }
            conn.Close();
            return View(a);
        }

        [HttpPost]
        public ActionResult Edit(Attendance a)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Index");

            SqlConnection conn = new SqlConnection(cs);
            string qry = "update Attendance2 set EmployeeId=@empId, AttendanceDate=@date, Status=@status where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);

            cmd.Parameters.AddWithValue("@Id", a.Id);
            cmd.Parameters.AddWithValue("@empId", a.EmployeeId);
            cmd.Parameters.AddWithValue("@date", a.AttendanceDate);
            cmd.Parameters.AddWithValue("@status", a.Status ?? (object)DBNull.Value);
            
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Index");

            SqlConnection conn = new SqlConnection(cs);
            string qry = "Delete from Attendance2 where Id=@id";
            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");

            Attendance a = new Attendance();
            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Attendance2 where Id=@id";

            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            if (drd.Read())
            {
                a.Id = (int)drd["Id"];
                a.EmployeeId = (int)drd["EmployeeId"];
                a.AttendanceDate = (DateTime)drd["AttendanceDate"];
                a.Status = (string)drd["Status"];
            }
            conn.Close();
            return View(a);
        }
    }
}
