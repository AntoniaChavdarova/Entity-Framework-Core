using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class XmlDto
    {
        [XmlAttribute("username")]
        public string UserName { get; set; }

        [XmlArray("Purchases")]
        public PurchasesDto[] Purchases { get; set; }
    }

    [XmlType("Purchase")]
    public class PurchasesDto
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public GameDto Game { get; set; }
    }

    [XmlType("Game")]
    public class GameDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
//< Users >
//  < User username = "mgraveson" >
 
//     < Purchases >
 
//       < Purchase >
 
//         < Card > 7991 7779 5123 9211 </ Card >
    
//            < Cvc > 340 </ Cvc >
    
//            < Date > 2017 - 08 - 31 17:09 </ Date >
           
//                   < Game title = "Counter-Strike: Global Offensive" >
            
//                      < Genre > Action </ Genre >
            
//                      < Price > 12.49 </ Price >
            
//                    </ Game >
            
//                  </ Purchase >
            
//                  < Purchase >
            
//                    < Card > 7790 7962 4262 5606 </ Card >
               
//                       < Cvc > 966 </ Cvc >
               
//                       < Date > 2018 - 02 - 28 08:38 </ Date >
                      
//                              < Game title = "Tom Clancy's Ghost Recon Wildlands" >
                       
//                                 < Genre > Action </ Genre >
                       
//                                 < Price > 59.99 </ Price >
                       
//                               </ Game >
                       
//                             </ Purchase >
                       
//                           </ Purchases >
                       
//                           < TotalSpent > 72.48 </ TotalSpent >
                       
//                         </ User >
