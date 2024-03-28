using System;


namespace MediaTekDocuments.model
{
    public class Abonnement : Commande
    {
        public DateTime DateFinAbonnement { get; }

        public string IdRevue { get; }

        public Abonnement(string id, DateTime dateCommande, float montant, DateTime dateFinAbonnement, string idRevue)
            : base(id, dateCommande, montant)
        {
            DateFinAbonnement = dateFinAbonnement;
            IdRevue = idRevue;
        }

    }
}