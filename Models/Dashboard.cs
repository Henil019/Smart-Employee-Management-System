using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeManagementSystem.Models
{
    public class Dashboard
    {
        public int TotalEmployee {  get; set; }
        public decimal HighestSalary {  get; set; }
        public decimal AvarageSalary {  get; set; }
        public int TotalDepartment {  get; set; }

    }
}