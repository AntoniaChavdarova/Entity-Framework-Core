using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MiniORM.App.Data.Entities
{
    public class Project
    {
        public Project()
        {
            this.EmployeeProjects = new HashSet<EmployeeProject>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<EmployeeProject> EmployeeProjects { get; }
    }
}
