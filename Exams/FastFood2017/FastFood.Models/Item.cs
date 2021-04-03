using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Models
{
    public class Item
    {
        public Item()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public decimal Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}