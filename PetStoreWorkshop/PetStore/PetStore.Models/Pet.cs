using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using PetStore.Common;
using PetStore.Models.Enums;

namespace PetStore.Models
{
   public class Pet
    {

        public Pet()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }

        [Required]
        [MinLength(GlobalConstants.PetNameMinLength)]
        public string Name { get; set; }

        public Gender Gender { get; set; }

        [Range(GlobalConstants.PetMinAge , GlobalConstants.PetMaxAge)]
        public int Age { get; set; }

        public bool IsSold { get; set; }

        [MinLength(GlobalConstants.SellableMinPrice)]
        public decimal Price { get; set; }

        [Required]
        public int BreedId { get; set; }

        public Breed Breed { get; set; }

        public int ClienId { get; set; }

        public Client Client { get; set; }




    }
}
