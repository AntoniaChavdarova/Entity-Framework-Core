﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using PetStore.Common;

namespace PetStore.Models
{
    public class Order
    {
        public Order()
        {
            this.Id = Guid.NewGuid().ToString();
            this.ClientProducts = new HashSet<ClientProduct>();
        }

        public string Id { get; set; }

        [Required]
        [MinLength(GlobalConstants.TownNameMinLength)]
        ]
        public string Town { get; set; }

        [Required]
        [MinLength(GlobalConstants.AddressTextMinLength)]
        public string Address { get; set; }

        public string Notes { get; set; }

        public decimal TotalPrice => this.ClientProducts.Sum(cp => cp.Product.Price * cp.Quantity);

        public ICollection<ClientProduct> ClientProducts { get; set; }
    }
}
