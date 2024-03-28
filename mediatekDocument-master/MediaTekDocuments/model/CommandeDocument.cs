using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class CommandeDocument : Commande
    {
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }
        public int IdSuivi { get; }
        public string Etat { get; }

        public CommandeDocument(string id, DateTime dateCommande, float montant, int nbExemplaire, 
            string idLivreDvd, int idSuivi, string etat) :
            base(id, dateCommande, montant)
        {
            this.NbExemplaire = nbExemplaire;
            this.IdLivreDvd = idLivreDvd;
            this.IdSuivi = idSuivi;
            this.Etat = etat;

        }
    }
}
