using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        #region Commun
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

       
        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// Getter sur les etats
        /// </summary>
        /// <returns></returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        #endregion

        #region Utilitaire

        /// <summary>
        /// Permets de gérer les demandes de requêtes HTML (post update delete) concernant
        /// un document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="titre"></param>
        /// <param name="image"></param>
        /// <param name="IdRayon"></param>
        /// <param name="IdPublic"></param>
        /// <param name="IdGenre"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitDocument(string id, string titre, string image, string IdRayon, string IdPublic, string IdGenre, string verbose)
        {
            Dictionary<string, string> dictDocument = new Dictionary<string, string>();
            dictDocument.Add("id", id);
            dictDocument.Add("titre", titre);
            dictDocument.Add("image", image);
            dictDocument.Add("idRayon", IdRayon);
            dictDocument.Add("idPublic", IdPublic);
            dictDocument.Add("idGenre", IdGenre);
            if (verbose == "post")
                return access.CreerEntite("document", JsonConvert.SerializeObject(dictDocument));
            if (verbose == "update")
                return access.ModifierEntite("document", id, JsonConvert.SerializeObject(dictDocument));
            if (verbose == "delete")
                return access.SupprimerEntite("document", JsonConvert.SerializeObject(dictDocument));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes HTML (post update delete) concernant
        /// un livre_dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitDvdLivre(string id, string verbose)
        {
            Dictionary<string, string> dictLivreDvd = new Dictionary<string, string>();
            dictLivreDvd.Add("id", id);
            if (verbose == "post")
                return access.CreerEntite("livres_dvd", JsonConvert.SerializeObject(dictLivreDvd));
            if (verbose == "delete")
                return access.SupprimerEntite("livres_dvd", JsonConvert.SerializeObject(dictLivreDvd));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes HTML (post update delete) concernant
        /// un livre
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isbn"></param>
        /// <param name="auteur"></param>
        /// <param name="collection"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitLivre(string id, string isbn, string auteur, string collection, string verbose)
        {
            Dictionary<string, string> unLivre = new Dictionary<string, string>();
            unLivre.Add("id", id);
            unLivre.Add("ISBN", isbn);
            unLivre.Add("auteur", auteur);
            unLivre.Add("collection", collection);
            if (verbose == "post")
                return access.CreerEntite("livre", JsonConvert.SerializeObject(unLivre));
            if (verbose == "update")
                return access.ModifierEntite("livre", id, JsonConvert.SerializeObject(unLivre));
            if (verbose == "delete")
                return access.SupprimerEntite("livre", JsonConvert.SerializeObject(unLivre));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// un Dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="synopsis"></param>
        /// <param name="realisateur"></param>
        /// <param name="duree"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitDvd(string id, string synopsis, string realisateur, int duree, string verbose)
        {
            Dictionary<string, object> unDvd = new Dictionary<string, object>();
            unDvd.Add("id", id);
            unDvd.Add("synopsis", synopsis);
            unDvd.Add("realisateur", realisateur);
            unDvd.Add("duree", duree);
            if (verbose == "post")
                return access.CreerEntite("dvd", JsonConvert.SerializeObject(unDvd));
            if (verbose == "update")
                return access.ModifierEntite("dvd", id, JsonConvert.SerializeObject(unDvd));
            if (verbose == "delete")
                return access.SupprimerEntite("dvd", JsonConvert.SerializeObject(unDvd));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// une revue
        /// </summary>
        /// <param name="id"></param>
        /// <param name="periodicite"></param>
        /// <param name="delaiMiseADispo"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitRevue(string id, string periodicite, int delaiMiseADispo, string verbose)
        {
            Dictionary<string, object> uneRevue = new Dictionary<string, object>();
            uneRevue.Add("id", id);
            uneRevue.Add("periodicite", periodicite);
            uneRevue.Add("delaiMiseADispo", delaiMiseADispo);
            if (verbose == "post")
                return access.CreerEntite("revue", JsonConvert.SerializeObject(uneRevue));
            if (verbose == "update")
                return access.ModifierEntite("revue", id, JsonConvert.SerializeObject(uneRevue));
            if (verbose == "delete")
                return access.SupprimerEntite("revue", JsonConvert.SerializeObject(uneRevue));
            return false;
        }

        #endregion

        #region Livre

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }


        /// <summary>
        /// creer un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerLivre(Livre livre)
        {
            bool valide = true;
            if (!utilitDocument(livre.Id, livre.Titre, livre.Image, livre.IdRayon, livre.IdPublic, livre.IdGenre, "post"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvdLivre(livre.Id, "post"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection, "post"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// modifie un livre dans la bddd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierLivre(Livre livre)
        {
            bool valide= true;
            if (!utilitDocument(livre.Id, livre.Titre, livre.Image, livre.IdRayon, livre.IdPublic, livre.IdGenre, "update"))
                valide = false;
            
            if (!utilitLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection, "update"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// supprime un livre dans la bdd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerLivre(Livre livre)
        {
            bool valide = true;
            if (!utilitLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection, "delete"))
                valide = false;
            
            if (!utilitDvdLivre(livre.Id, "delete"))
                valide = false;
            
            if (!utilitDocument(livre.Id, livre.Titre, livre.Image, livre.IdRayon, livre.IdPublic, livre.IdGenre, "delete"))
                valide = false;

            return valide;
        }
        #endregion

        #region Dvd

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// creer un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerDvd(Dvd dvd)
        {
            bool valide = true;
            if (!utilitDocument(dvd.Id, dvd.Titre, dvd.Image, dvd.IdRayon, dvd.IdPublic, dvd.IdGenre, "post"))
                valide = false;
            if (!utilitDvdLivre(dvd.Id, "post"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree, "post"))
                valide = false;

            return valide;
        }


        /// <summary>
        /// modifie un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            bool valide = true;
            if (!utilitDocument(dvd.Id, dvd.Titre, dvd.Image, dvd.IdRayon, dvd.IdPublic, dvd.IdGenre, "update"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree, "update"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// supprime un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            bool valide = true;
            if (!utilitDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree, "delete"))
                valide = false;
            if (!utilitDvdLivre(dvd.Id, "delete"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDocument(dvd.Id, dvd.Titre, dvd.Image, dvd.IdRayon, dvd.IdPublic, dvd.IdGenre, "delete"))
                valide = false;

            return valide;
        }

        #endregion

        #region Revue


        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// creer une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerRevue(Revue revue)
        {
            bool valide = true;
            if (!utilitDocument(revue.Id, revue.Titre, revue.Image, revue.IdRayon, revue.IdPublic, revue.IdGenre, "post"))
                valide = false;
            
            if (!utilitRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo, "post"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// modifie une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierRevue(Revue revue)
        {
            bool valide = true;
            if (!utilitDocument(revue.Id, revue.Titre, revue.Image, revue.IdRayon, revue.IdPublic, revue.IdGenre, "update"))
                valide = false;
            
            if (!utilitRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo, "update"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// supprime une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerRevue(Revue revue)
        {
            bool valide = true;
            if (!utilitRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo, "delete"))
                valide = false;
            if (!utilitDocument(revue.Id, revue.Titre, revue.Image, revue.IdRayon, revue.IdPublic, revue.IdGenre, "delete"))
                valide = false;

            return valide;
        }
        #endregion

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        #region Commandes de livres et Dvd
        /// <summary>
        /// Récupère les commandes d'une livre
        /// </summary>
        /// <param name="idLivre">id du livre concernée</param>
        /// <returns></returns>
        public List<CommandeDocument> GetCommandesLivres(string idLivre)
        {
            return access.GetCommandesLivres(idLivre);
        }

        public List<CommandeDocument> GetCommandesDvd(string idDvd)
        {
            return access.GetCommandesDvd(idDvd);
        }

        /// <summary>
        /// Retourne l'id max des commandes
        /// </summary>
        /// <returns></returns>
        public string getNbCommandeMax()
        {
            return access.getMaxIndex("maxcommande");
        }

        /// <summary>
        /// Retourne l'id max des livres
        /// </summary>
        /// <returns></returns>
        public string getNbLivreMax()
        {
            return access.getMaxIndex("maxlivre");
        }

        /// <summary>
        /// Retourne l'id max des Dvd
        /// </summary>
        /// <returns></returns>
        public string getNbDvdMax()
        {
            return access.getMaxIndex("maxdvd");
        }

        /// <summary>
        /// Retourne l'id max des revues
        /// </summary>
        /// <returns></returns>
        public string getNbRevueMax()
        {
            return access.getMaxIndex("maxrevue");
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// une commande de livre ou dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="idSuivi"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilCommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire,
            string idLivreDvd, int idSuivi, string etat, string verbose)
        {
            Dictionary<string, object> uneCommandeDocument = new Dictionary<string, object>();
            uneCommandeDocument.Add("Id", id);
            uneCommandeDocument.Add("DateCommande", dateCommande.ToString("yyyy-MM-dd"));
            uneCommandeDocument.Add("Montant", montant);
            uneCommandeDocument.Add("NbExemplaire", nbExemplaire);
            uneCommandeDocument.Add("IdLivreDvd", idLivreDvd);
            uneCommandeDocument.Add("IdSuivi", idSuivi);
            uneCommandeDocument.Add("Etat", etat);

            if (verbose == "post")
                return access.CreerEntite("commandedocument", JsonConvert.SerializeObject(uneCommandeDocument));
            if (verbose == "update")
                return access.ModifierEntite("commandedocument", id, JsonConvert.SerializeObject(uneCommandeDocument));
            if (verbose == "delete")
                return access.SupprimerEntite("commandedocument", JsonConvert.SerializeObject(uneCommandeDocument));
            return false;
        }

        /// <summary>
        /// Creer une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool CreerLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return utilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                    commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "post");
        }

        /// <summary>
        /// Modifie une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool ModifierLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return utilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                   commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "update");
        }

        /// <summary>
        /// Supprime une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool SupprimerLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return utilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                   commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "delete");
        }
        #endregion

        #region abonnements

        /// <summary>
        /// Retourne tous les abonnements d'une revue
        /// </summary>
        /// <param name="idRevue"></param>
        /// <returns></returns>
        public List<Abonnement> GetAbonnements(string idRevue)
        {
            return access.GetAbonnements(idRevue);
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// un abonnement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="idRevue"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilAbonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue, string verbose)
        {
            Dictionary<string, object> unAbonnement = new Dictionary<string, object>();
            unAbonnement.Add("Id", id);
            unAbonnement.Add("DateCommande", dateCommande.ToString("yyyy-MM-dd"));
            unAbonnement.Add("Montant", montant);
            unAbonnement.Add("DateFinAbonnement", dateFinAbonnement.ToString("yyyy-MM-dd"));
            unAbonnement.Add("IdRevue", idRevue);

            if (verbose == "post")
                return access.CreerEntite("abonnement", JsonConvert.SerializeObject(unAbonnement));
            if (verbose == "update")
                return access.ModifierEntite("abonnement", id, JsonConvert.SerializeObject(unAbonnement));
            if (verbose == "delete")
                return access.SupprimerEntite("abonnement", JsonConvert.SerializeObject(unAbonnement));
            return false;
        }

        /// <summary>
        /// Creer un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return utilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "post");
        }

        /// <summary>
        /// Modifie un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool UpdateAbonnement(Abonnement abonnement)
        {
            return utilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "update");
        }

        /// <summary>
        /// Supprime un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            return utilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "delete");
        }

        #endregion
    }
}
