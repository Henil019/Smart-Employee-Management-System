using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["EMS"].ConnectionString;

        private void PopulateDropdowns()
        {
            List<Department> depts = new List<Department>();
            List<Role> roles = new List<Role>();
            
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            
            SqlCommand cmd1 = new SqlCommand("SELECT * FROM Department", conn);
            SqlDataReader drd1 = cmd1.ExecuteReader();
            while (drd1.Read())
            {
                Department d = new Department();
                d.Id = (int)drd1["Id"];
                d.Name = drd1["Name"].ToString();
                depts.Add(d);
            }
            drd1.Close();
            
            SqlCommand cmd2 = new SqlCommand("SELECT * FROM Role", conn);
            SqlDataReader drd2 = cmd2.ExecuteReader();
            while (drd2.Read())
            {
                Role r = new Role();
                r.Id = (int)drd2["Id"];
                r.Title = drd2["Title"].ToString();
                roles.Add(r);
            }
            drd2.Close();
            conn.Close();
            
            ViewBag.Departments = new SelectList(depts, "Id", "Name");
            ViewBag.Roles = new SelectList(roles, "Id", "Title");
        }

        public ActionResult Index()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");

            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            List<Employee> list = new List<Employee>();
            SqlConnection conn = new SqlConnection(cs);
            string qry = @"
                SELECT e.*, d.Name as DepartmentName, r.Title as RoleTitle 
                FROM Employee e 
                LEFT JOIN Department d ON e.DepartmentId = d.Id 
                LEFT JOIN Role r ON e.RoleId = r.Id";

            if (role == "HR" && Session["DepartmentId"] != null)
            {
                qry += " WHERE e.DepartmentId = @deptId";
            }

            SqlCommand cmd = new SqlCommand(qry, conn);
            
            if (role == "HR" && Session["DepartmentId"] != null)
            {
                cmd.Parameters.AddWithValue("@deptId", Session["DepartmentId"]);
            }

            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            while (drd.Read())
            {
                Employee e = new Employee();
                e.Id = (int)drd["Id"];
                e.FirstName = (string)drd["FirstName"];
                e.LastName = (string)drd["LastName"];
                e.Email = (string)drd["Email"];
                e.DepartmentId = drd["DepartmentId"] != DBNull.Value ? (int)drd["DepartmentId"] : 0;
                e.RoleId = drd["RoleId"] != DBNull.Value ? (int)drd["RoleId"] : 0;
                e.DepartmentName = drd["DepartmentName"].ToString();
                e.RoleTitle = drd["RoleTitle"].ToString();
                e.Salary = (decimal)drd["Salary"];
                e.JoinDate = (DateTime)drd["JoinDate"];
                e.IsHR = drd["IsHR"] != DBNull.Value && Convert.ToBoolean(drd["IsHR"]);

                list.Add(e);
            }
            conn.Close();

            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee e)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            if (ModelState.IsValid)
            {
                SqlConnection con = new SqlConnection(cs);
                string qry = "insert into Employee(FirstName, LastName, Email, DepartmentId, RoleId, Salary, JoinDate, Password, IsHR) values(@fnm, @lnm, @em, @dp, @rl, @sl, @jn, @pwd, @ishr)";
                SqlCommand cmd = new SqlCommand(qry, con);

                cmd.Parameters.AddWithValue("@fnm", e.FirstName);
                cmd.Parameters.AddWithValue("@lnm", e.LastName);
                cmd.Parameters.AddWithValue("@em", e.Email);
                cmd.Parameters.AddWithValue("@dp", e.DepartmentId);
                cmd.Parameters.AddWithValue("@rl", e.RoleId);
                cmd.Parameters.AddWithValue("@sl", e.Salary);
                cmd.Parameters.AddWithValue("@jn", e.JoinDate);
                cmd.Parameters.AddWithValue("@pwd", e.Password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ishr", e.IsHR);
                
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index");
            }
            PopulateDropdowns();
            return View(e);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            Employee e = new Employee();

            SqlConnection conn = new SqlConnection(cs);
            string qry = "Select * from Employee where Id=@id";

            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);
            
            conn.Open();

            SqlDataReader drd = cmd.ExecuteReader();

            if (drd.Read())
            {
                e.Id = (int)drd["Id"];
                e.FirstName = (string)drd["FirstName"];
                e.LastName = (string)drd["LastName"];
                e.Email = (string)drd["Email"];
                e.DepartmentId = drd["DepartmentId"] != DBNull.Value ? (int)drd["DepartmentId"] : 0;
                e.RoleId = drd["RoleId"] != DBNull.Value ? (int)drd["RoleId"] : 0;
                e.Salary = (decimal)drd["Salary"];
                e.JoinDate = (DateTime)drd["JoinDate"];
                e.Password = drd["Password"].ToString();
                e.IsHR = drd["IsHR"] != DBNull.Value && Convert.ToBoolean(drd["IsHR"]);
            }
            conn.Close();
            PopulateDropdowns();
            return View(e);
        }

        [HttpPost]
        public ActionResult Edit(Employee e)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(cs);
                string qry = "update Employee set FirstName=@fnm, LastName=@lnm, Email=@em, DepartmentId=@dp, RoleId=@rl, Salary=@sl, JoinDate=@jn, Password=@pwd, IsHR=@ishr where Id=@id";
                SqlCommand cmd = new SqlCommand(qry, conn);

                cmd.Parameters.AddWithValue("@Id", e.Id);
                cmd.Parameters.AddWithValue("@fnm", e.FirstName);
                cmd.Parameters.AddWithValue("@lnm", e.LastName);
                cmd.Parameters.AddWithValue("@em", e.Email);
                cmd.Parameters.AddWithValue("@dp", e.DepartmentId);
                cmd.Parameters.AddWithValue("@rl", e.RoleId);
                cmd.Parameters.AddWithValue("@sl", e.Salary);
                cmd.Parameters.AddWithValue("@jn", e.JoinDate);
                cmd.Parameters.AddWithValue("@pwd", e.Password ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ishr", e.IsHR);
                
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                return RedirectToAction("Index");
            }
            PopulateDropdowns();
            return View(e);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            string qry1 = "Delete from Attendance2 where EmployeeId=@id";
            SqlCommand cmd1 = new SqlCommand(qry1, conn);
            cmd1.Parameters.AddWithValue("@id", id);
            cmd1.ExecuteNonQuery();

            string qry2 = "Delete from Employee where Id=@id";
            SqlCommand cmd2 = new SqlCommand(qry2, conn);
            cmd2.Parameters.AddWithValue("@id", id);
            cmd2.ExecuteNonQuery();

            conn.Close();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee" && Session["EmployeeId"] != null && (int)Session["EmployeeId"] != id)
            {
                return RedirectToAction("Details", new { id = Session["EmployeeId"] });
            }

            Employee e = new Employee();

            SqlConnection conn = new SqlConnection(cs);
            string qry = @"
                SELECT e.*, d.Name as DepartmentName, r.Title as RoleTitle 
                FROM Employee e 
                LEFT JOIN Department d ON e.DepartmentId = d.Id 
                LEFT JOIN Role r ON e.RoleId = r.Id
                WHERE e.Id=@id";

            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.AddWithValue("@id", id);

            conn.Open();
            SqlDataReader drd = cmd.ExecuteReader();

            if (drd.Read())
            {
                e.Id = (int)drd["Id"];
                e.FirstName = (string)drd["FirstName"];
                e.LastName = (string)drd["LastName"];
                e.Email = (string)drd["Email"];
                e.DepartmentId = drd["DepartmentId"] != DBNull.Value ? (int)drd["DepartmentId"] : 0;
                e.RoleId = drd["RoleId"] != DBNull.Value ? (int)drd["RoleId"] : 0;
                e.DepartmentName = drd["DepartmentName"].ToString();
                e.RoleTitle = drd["RoleTitle"].ToString();
                e.Salary = (decimal)drd["Salary"];
                e.JoinDate = (DateTime)drd["JoinDate"];
            }
            conn.Close();
            return View(e);
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            string role = Session["Role"] as string;
            if (role == "Employee") return RedirectToAction("Details", new { id = Session["EmployeeId"] });

            Dashboard d = new Dashboard();
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();

            string qry = "select count(*) from Employee";
            if (role == "HR" && Session["DepartmentId"] != null)
            {
                qry += " WHERE DepartmentId = " + Session["DepartmentId"];
            }
            SqlCommand cmd = new SqlCommand(qry, conn);
            d.TotalEmployee = Convert.ToInt32(cmd.ExecuteScalar());

            string qry1 = "select max(salary) from Employee";
            SqlCommand cmd1= new SqlCommand(qry1, conn);
            d.HighestSalary = Convert.ToInt32(cmd1.ExecuteScalar());

            conn.Close();
            return View(d);
        }
    }
}