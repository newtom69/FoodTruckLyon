using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.Models
{
    public enum ValiditeCodePromo
    {
        Inconnu,
        Valide,
        DateDepasse,
        DateFuture,
        MontantInsuffisant,
    }
}