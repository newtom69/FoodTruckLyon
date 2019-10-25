namespace FoodTruck.Models
{
    public enum TypeRepas
    {
        //anciennes déf
        Dejeuner = 1,
        Diner = 2,

        //nouvelles déf doit correspondre à la table en bdd Repas // TODO faire DAL pour récuperer ces valeurs
        After = 5,
        PetitDéjeuner = 10,
        Déjeuner = 20,
        Gouter = 30,
        Dîner = 40,
        Nuit = 50,
    }
}