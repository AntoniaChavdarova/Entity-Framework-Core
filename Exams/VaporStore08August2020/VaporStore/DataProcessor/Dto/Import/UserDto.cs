using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    class UserDto
    {

        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^([A-Z]{1}[a-z]*\s[A-Z]{1}[a-z]*)$")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3 , 103)]
        public int Age { get; set; }

        public CardDto[] Cards { get; set; }
    }

    public class CardDto
    {
        [Required]
        [RegularExpression(@"^([0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4})$")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]{3})$")]
        public string Cvc { get; set; }

        [Required]
        [EnumDataType(typeof(CardType))]
        public string Type { get; set; }
    }
}

//{
//    "FullName": "",
//    "Username": "invalid",
//    "Email": "invalid@invalid.com",
//    "Age": 20,
//    "Cards": [
//      {
//        "Number": "1111 1111 1111 1111",
//        "CVC": "111",
//        "Type": "Debit"
//      }
   
