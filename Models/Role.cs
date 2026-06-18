using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role Title is required")]
        [Display(Name = "Role Title")]
        public string Title { get; set; }
    }
}
