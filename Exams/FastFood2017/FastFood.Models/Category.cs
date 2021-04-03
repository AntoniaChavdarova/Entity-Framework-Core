using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FastFood.Models
{
   public  class Category
    {
        public Category()
        {
            this.Items = new HashSet<Item>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
