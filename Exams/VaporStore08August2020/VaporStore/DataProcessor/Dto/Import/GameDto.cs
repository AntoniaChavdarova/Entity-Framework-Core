using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.ImportResults
{
    public class GameDto
    {
        [Required]
        public string Name { get; set; }

        [Range(typeof(decimal) , "0" , "79228162514264337593543950335")]
        public decimal Price { get; set; }

        public string ReleaseDate { get; set; }

        [Required]
        public string Developer { get; set; }

        [Required]
        public string Genre { get; set; }

        public string[] Tags { get; set; }
    }
}

//    "Name": "Dota 2",
//    "Price": 0,
//    "ReleaseDate": "2013-07-09",
//    "Developer": "Valve",
//    "Genre": "Action",
//    "Tags": [
//      "Multi-player",
//      "Co-op",
//      "Steam Trading Cards",
//      "Steam Workshop",
//      "SteamVR Collectibles",
//      "In-App Purchases",
//      "Valve Anti-Cheat enabled"
//    ]

