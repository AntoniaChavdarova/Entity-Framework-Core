using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MiniORM.App.Data.Entities
{
    public class Department
    {
        //public Department()
        //{
        //    this.Employees = new HashSet<Employee>();
        //}

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
