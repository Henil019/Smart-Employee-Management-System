using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class AIController : Controller
    {
        // GET: AI
        public ActionResult Index()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login", "Admin");
            return View();
        }
    }
}