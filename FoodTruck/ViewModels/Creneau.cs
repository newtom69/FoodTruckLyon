using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.ViewModels
{
    public class Creneau
    {
        public DateTime DateRetrait { get; set; }
        public int CommandesPossiblesRestantes { get; set; }
        
    }
}