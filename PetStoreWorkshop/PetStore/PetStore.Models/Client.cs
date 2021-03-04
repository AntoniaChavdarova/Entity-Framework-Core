using PetStore.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetStore.Models
{
    public class Client
    {
        public Client()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PetsBuyed = new HashSet<Pet>();
        }

        public string Id { get; set; }

        [Required]
        [MinLength(GlobalConstants.UsernameMinLength)]
        [MaxLength(GlobalConstants.UsernameMaxLength)]
        public string UserName { get; set; }

        [Required]
        [MinLength(GlobalConstants.ClientPassMinLength )]
        [MaxLength (GlobalConstants.ClientPassMaxLength)]
        public string Password { get; set; }

        [Required]
        [MinLength(GlobalConstants.EmailMinLength )]
        [MaxLength(GlobalConstants.EmailMaxLength)]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public virtual ICollection<Pet> PetsBuyed { get; set; }


    }
}
