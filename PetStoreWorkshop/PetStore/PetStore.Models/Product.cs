using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using PetStore.Common;
using PetStore.Models.Enums;

namespace PetStore.Models
{
    public class Product
    {
        public Product()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ProductType ProductType { get; set; }

        [Range(GlobalConstants.SellableMinPrice , Double.MaxValue)]
        public decimal Price { get; set; }
    }
}
