using PetStore.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class ClientProduct
    {
        public int ClientId { get; set; }

        public Client Client { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Range(GlobalConstants.ClientProductMinQuantity , Int32.MaxValue)]
        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}