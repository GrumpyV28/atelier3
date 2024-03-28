using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Commande
    {
        public string Id { get;  }
        public DateTime DateCommande { get; }

        public float Montant { get; }


        public Commande(string id, DateTime dateCommande, float montant)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }


}
