using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private bool ajouterBool = false;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesSuivis">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Augemente un index de type string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string plusUnIdString(string id)
        {
            int taille = id.Length;
            int idnum = int.Parse(id) + 1;
            id = idnum.ToString();
            if (id.Length > taille)
                MessageBox.Show("Taille du registre arrivé a saturation");
            while (id.Length != taille)
            {
                id = "0" + id;
            }
            return id;
        }

        /// <summary>
        /// Ouvre une MessageBox au lancement de FrmMediatek.cs
        /// si des abonnements sont proches de se terminer
        /// </summary>
        private void afficherAlerteAbo()
        {
            bool interupteur = false;
            List<Revue> revues = controller.GetAllRevues();
            string alerteRevues = "Revues dont l'abonnement se termine dans moins de 30 jours : \n";
            foreach (Revue revue in revues)
            {
                List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                abonnements = abonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddMonths(1))
                            && (o.DateFinAbonnement >= DateTime.Now));
                if (abonnements.Count > 0)
                {
                    alerteRevues += "  -" + revue.Titre + "\n";
                    interupteur = true;
                }

            }

            if (interupteur)
                MessageBox.Show(alerteRevues);
        }
        #endregion



        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private readonly BindingSource bdgGenresInfo = new BindingSource();
        private readonly BindingSource bdgPublicInfo = new BindingSource();
        private readonly BindingSource bdgRayonInfo = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenresInfo, cbxLivresGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublicInfo, cbxLivresPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayonInfo, cbxLivresRayonInfos);
            LivreEnCoursDeModif(false);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche avec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            cbxLivresGenreInfos.SelectedIndex = cbxLivresGenreInfos.FindString(livre.Genre);
            cbxLivresPublicInfos.SelectedIndex = cbxLivresPublicInfos.FindString(livre.Public);
            cbxLivresRayonInfos.SelectedIndex = cbxLivresRayonInfos.FindString(livre.Rayon);
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            cbxLivresGenreInfos.SelectedIndex = -1;
            cbxLivresPublicInfos.SelectedIndex = -1;
            cbxLivresRayonInfos.SelectedIndex = -1;
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void LivreEnCoursDeModif(bool modif)
        {
            BtnAjouterLivres.Enabled = !modif;
            BtnSupprimerLivres.Enabled = !modif;
            BtnModifierLivres.Enabled = !modif;
            BtnAnnulerChoix.Enabled = modif;
            BtnValiderChoix.Enabled = modif;
            txbLivresTitre.ReadOnly = !modif;
            txbLivresAuteur.ReadOnly = !modif;
            cbxLivresPublicInfos.Enabled = modif;
            txbLivresIsbn.ReadOnly = !modif;
            txbLivresCollection.ReadOnly = !modif;
            cbxLivresGenreInfos.Enabled = modif;
            cbxLivresRayonInfos.Enabled = modif;
            txbLivresImage.ReadOnly = !modif;
            txbLivresNumero.ReadOnly = true;
            dgvLivresListe.Enabled = !modif;
        }

        /// <summary>
        /// enclanche la procédure d'ajout de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjouterLivres_Click(object sender, EventArgs e)
        {
            LivreEnCoursDeModif(true);
            txbLivresNumero.ReadOnly = false;
            VideLivresInfos();
        }

        /// <summary>
        /// enclanche la procédure de modification de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifierLivres_Click(object sender, EventArgs e)
        {
            LivreEnCoursDeModif(true);
        }

        /// <summary>
        /// enclanche la procédure de suppresion de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerLivres_Click(object sender, EventArgs e)
        {
            Livre leLivre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            if (MessageBox.Show($"Etes vous sur de vouloir supprimer {leLivre.Titre} de {leLivre.Auteur} ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // fonction a modifier pour prendre en charge le faite que l'on ne pourra pas supprimer un livre tant que des examplaire de se livre existe
                if (controller.SupprimerLivre(leLivre))
                {

                    lesLivres = controller.GetAllLivres();
                    RemplirLivresListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// annule les modification ou ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerChoixLivres_Click(object sender, EventArgs e)
        {
            LivreEnCoursDeModif(false);
        }


        /// <summary>
        /// valide dans la bdd les changements en cours ( ajout / modification)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderChoixLivres_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbLivresNumero.Text; ;
                int? a = null;
                try
                {
                    a = int.Parse(txbLivresNumero.Text);
                }
                catch
                {
                    MessageBox.Show("Le Numéro de document doit etre un entier");
                }
                Genre unGenre = (Genre)cbxLivresGenreInfos.SelectedItem;
                Public unPublic = (Public)cbxLivresPublicInfos.SelectedItem;
                Rayon unRayon = (Rayon)cbxLivresRayonInfos.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = txbLivresTitre.Text;
                string image = txbLivresImage.Text;
                string isbn = txbLivresIsbn.Text;
                string auteur = txbLivresAuteur.Text;
                string collection = txbLivresCollection.Text;
                string idGenre = (unGenre == null) ? null : unGenre.Id;
                string genre = (unGenre == null) ? null : unGenre.Libelle;
                string idPublic = (unPublic == null) ? null : unPublic.Id;
                string lePublic = (unPublic == null) ? null : unPublic.Libelle;
                string idRayon = (unRayon == null) ? null : unRayon.Id;
                string rayon = (unRayon == null) ? null : unRayon.Libelle;
                if (a != null && titre != "" && auteur != "" && genre != null && unPublic != null)
                {
                    Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (txbLivresNumero.ReadOnly)  // si on est en  modification
                        checkValid = controller.ModifierLivre(livre);
                    else      // si on est en creation
                        checkValid = controller.CreerLivre(livre);
                    if (checkValid)
                    {
                        LivreEnCoursDeModif(false);
                        lesLivres = controller.GetAllLivres();
                        RemplirLivresListeComplete();
                    }
                    else
                    {
                        if (txbLivresNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private readonly BindingSource bdgDvdGenreInfos = new BindingSource();
        private readonly BindingSource bdgDvdPublicInfo = new BindingSource();
        private readonly BindingSource bdgDvdRayonInfo = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgDvdGenreInfos, cbxDvdGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgDvdPublicInfo, cbxDvdPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgDvdRayonInfo, cbxDvdRayonInfos);
            DvdEnCoursDeModif(false);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            cbxDvdGenreInfos.SelectedIndex = cbxDvdGenreInfos.FindString(dvd.Genre);
            cbxDvdPublicInfos.SelectedIndex = cbxDvdPublicInfos.FindString(dvd.Public);
            cbxDvdRayonInfos.SelectedIndex = cbxDvdRayonInfos.FindString(dvd.Rayon);
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            cbxDvdGenreInfos.SelectedIndex = -1;
            cbxDvdPublicInfos.SelectedIndex = -1;
            cbxDvdRayonInfos.SelectedIndex = -1;
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void DvdEnCoursDeModif(bool modif)
        {
            BtnAjouterDvd.Enabled = !modif;
            BtnSupprimerDvd.Enabled = !modif;
            BtnModifierDvd.Enabled = !modif;
            BtnAnnulerChoixDvd.Enabled = modif;
            BtnValiderChoixDvd.Enabled = modif;
            txbDvdTitre.ReadOnly = !modif;
            txbDvdRealisateur.ReadOnly = !modif;
            txbDvdSynopsis.ReadOnly = !modif;
            cbxDvdPublicInfos.Enabled = modif;
            txbDvdDuree.ReadOnly = !modif;
            cbxDvdGenreInfos.Enabled = modif;
            cbxDvdRayonInfos.Enabled = modif;
            txbDvdImage.ReadOnly = !modif;
            dgvDvdListe.Enabled = !modif;
            txbDvdNumero.ReadOnly = true;
        }

        /// <summary>
        /// lance la procédure d'ajout d'un nouveau dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjouterDvd_Click(object sender, EventArgs e)
        {
            DvdEnCoursDeModif(true);
            txbDvdNumero.ReadOnly = false;
            VideDvdInfos();
        }

        /// <summary>
        /// lance la procédure de modification du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifierDvd_Click(object sender, EventArgs e)
        {
            DvdEnCoursDeModif(true);
        }

        /// <summary>
        /// lance la procédure de suppresion du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerDvd_Click(object sender, EventArgs e)
        {
            Dvd leDvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            if (MessageBox.Show($"Etes vous sur de vouloir supprimer {leDvd.Titre} de {leDvd.Realisateur} ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // fonction a modifier pour prendre en charge le faite que l'on ne pourra pas supprimer un livre tant que des examplaire de se livre existe
                if (controller.SupprimerDvd(leDvd))
                {
                    lesDvd = controller.GetAllDvd();
                    RemplirDvdListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// Annule les modification en cours (ajouter / supprimer )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerChoixDvd_Click(object sender, EventArgs e)
        {
            DvdEnCoursDeModif(false);
        }


        /// <summary>
        /// Valide les modification en cours dans la BDD ( ajouter / supprimer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderChoixDvd_Click(object sender, EventArgs e)
        {
            bool Valide = true;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbDvdNumero.Text; ;
                int? a = null;  // version simplifié de nullabe <int> a

                try
                {
                    a = int.Parse(txbDvdNumero.Text);
                }
                catch
                {
                    MessageBox.Show("Le Numéro de document doit etre un entier");
                }
                Genre unGenre = (Genre)cbxDvdGenreInfos.SelectedItem;
                Public unPublic = (Public)cbxDvdPublicInfos.SelectedItem;
                Rayon unRayon = (Rayon)cbxDvdRayonInfos.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = txbDvdTitre.Text;
                string image = txbDvdImage.Text;
                int duree = (txbDvdDuree.Text == "") ? 0 : int.Parse(txbDvdDuree.Text);
                string realisateur = txbDvdRealisateur.Text;
                string synopsis = txbDvdSynopsis.Text;
                string idGenre = (unGenre == null) ? null : unGenre.Id;
                string genre = (unGenre == null) ? null : unGenre.Libelle;
                string idPublic = (unPublic == null) ? null : unPublic.Id;
                string lePublic = (unPublic == null) ? null : unPublic.Libelle;
                string idRayon = (unRayon == null) ? null : unRayon.Id;
                string rayon = (unRayon == null) ? null : unRayon.Libelle;
                if (a != null && titre != "" && realisateur != "" && genre != null && unPublic != null)
                {
                    Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (txbDvdNumero.ReadOnly)  // si on est en  modification
                        Valide = controller.ModifierDvd(dvd);
                    else    // si on est en creation
                        Valide = controller.CreerDvd(dvd);
                    if (Valide)
                    {
                        DvdEnCoursDeModif(false);
                        lesDvd = controller.GetAllDvd();
                        RemplirDvdListeComplete();
                    }
                    else
                    {
                        if (txbDvdNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();
        private readonly BindingSource bdgRevuesGenreInfos = new BindingSource();
        private readonly BindingSource bdgRevuesPublicInfos = new BindingSource();
        private readonly BindingSource bdgRevuesRayonInfos = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgRevuesGenreInfos, cbxRevuesGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgRevuesPublicInfos, cbxRevuesPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRevuesRayonInfos, cbxRevuesRayonInfos);
            RevueEnCoursDeModif(false);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            cbxRevuesGenreInfos.SelectedIndex = cbxRevuesGenreInfos.FindString(revue.Genre);
            cbxRevuesPublicInfos.SelectedIndex = cbxRevuesPublicInfos.FindString(revue.Public);
            cbxRevuesRayonInfos.SelectedIndex = cbxRevuesRayonInfos.FindString(revue.Rayon);
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            cbxRevuesGenreInfos.SelectedIndex = -1;
            cbxRevuesPublicInfos.SelectedIndex = -1;
            cbxRevuesRayonInfos.SelectedIndex = -1;
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void RevueEnCoursDeModif(bool modif)
        {
            BtnAjouterRevue.Enabled = !modif;
            BtnSupprimerRevue.Enabled = !modif;
            BtnModifierRevue.Enabled = !modif;
            BtnAnnulerChoixRevue.Enabled = modif;
            BtnValiderChoixRevue.Enabled = modif;
            txbRevuesTitre.ReadOnly = !modif;
            txbRevuesPeriodicite.ReadOnly = !modif;
            cbxRevuesPublicInfos.Enabled = modif;
            txbRevuesDateMiseADispo.ReadOnly = !modif;
            txbLivresCollection.ReadOnly = !modif;
            cbxRevuesGenreInfos.Enabled = modif;
            cbxRevuesRayonInfos.Enabled = modif;
            txbRevuesImage.ReadOnly = !modif;
            txbRevuesNumero.ReadOnly = true;
            dgvRevuesListe.Enabled = !modif;
        }

        /// <summary>
        /// lance la procédure d'ajout d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjouterRevues_Click(object sender, EventArgs e)
        {
            RevueEnCoursDeModif(true);
            txbRevuesNumero.ReadOnly = false;
            VideRevuesInfos();

        }

        /// <summary>
        /// lance la procédure de modification d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierRevues_Click(object sender, EventArgs e)
        {
            RevueEnCoursDeModif(true);
        }

        /// <summary>
        /// lance la procédure de suppresion d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerRevues_Click(object sender, EventArgs e)
        {
            Revue laRevue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            if (MessageBox.Show($"Etes vous sur de vouloir supprimer {laRevue.Titre}?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (controller.GetExemplairesRevue(laRevue.Id).Count == 0)
                {

                    if (controller.SupprimerRevue(laRevue))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Des parutions sont rattachées à cette revue, vous ne pouvez pas la supprimer");
                }

            }
        }

        /// <summary>
        /// annule les modifications en cours (ajout / suppresion)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnulerChoixRevues_Click(object sender, EventArgs e)
        {
            RevueEnCoursDeModif(false);
        }

        /// <summary>
        /// valide les modifications en cours dans la bdd ( ajout / suppresion)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValiderChoixRevues_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbRevuesNumero.Text; ;
                int delaiMiseADispo = 0;
                int? a = null;
                int? b = null;
                try
                {
                    a = int.Parse(txbRevuesNumero.Text);
                    delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);
                    b = delaiMiseADispo;
                }
                catch
                {
                    MessageBox.Show("Le Numéro de document et le delai de mise a dispo doivent etre des entiers");
                }
                Genre unGenre = (Genre)cbxRevuesGenreInfos.SelectedItem;
                Public unPublic = (Public)cbxRevuesPublicInfos.SelectedItem;
                Rayon unRayon = (Rayon)cbxRevuesRayonInfos.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string idGenre = (unGenre == null) ? null : unGenre.Id;
                string genre = (unGenre == null) ? null : unGenre.Libelle;
                string idPublic = (unPublic == null) ? null : unPublic.Id;
                string lePublic = (unPublic == null) ? null : unPublic.Libelle;
                string idRayon = (unRayon == null) ? null : unRayon.Id;
                string rayon = (unRayon == null) ? null : unRayon.Libelle;
                string periodicite = txbRevuesPeriodicite.Text;
                if (a != null && b != null && titre != "" && genre != null && unPublic != null)
                {
                    Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);
                    if (txbRevuesNumero.ReadOnly)  // si on est en  modification
                        checkValid = controller.ModifierRevue(revue);
                    else      // si on est en creation
                        checkValid = controller.CreerRevue(revue);
                    if (checkValid)
                    {
                        RevueEnCoursDeModif(false);
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        if (txbRevuesNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region Onglet Commande Livres

        //Information du premier dgv et des infos du Livre  selectionné
        private readonly BindingSource bdgLesLivresListe = new BindingSource();
        private readonly BindingSource bdgComLivreGenresInfo = new BindingSource();
        private readonly BindingSource bdgComLivrePublicInfo = new BindingSource();
        private readonly BindingSource bdgComLivreRayonInfo = new BindingSource();
        private List<Livre> lesComLivres = new List<Livre>();
        // informations du second dgv et des infos des la commandes selectionné
        private readonly BindingSource bdgListeCommandeLivre = new BindingSource();
        private readonly BindingSource bdgComLivreEtat = new BindingSource();
        private List<CommandeDocument> lesCommandes = new List<CommandeDocument>();

        /// <summary>
        /// Ouverture de l'onglet Commande de Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeLivres_Enter(object sender, EventArgs e)
        {
            lesComLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComLivresRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgComLivreGenresInfo, cbxComLivresGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgComLivrePublicInfo, cbxComLivresPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgComLivreRayonInfo, cbxComLivresRayonInfos);
            RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
            CommandeLivreEnCoursDeModif(false);
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// affiche la liste des livres
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirComLivresListe(List<Livre> livres)
        {
            bdgLesLivresListe.DataSource = livres;
            DgvLesLivresListe.DataSource = bdgLesLivresListe;
            DgvLesLivresListe.Columns["isbn"].Visible = false;
            DgvLesLivresListe.Columns["idRayon"].Visible = false;
            DgvLesLivresListe.Columns["idGenre"].Visible = false;
            DgvLesLivresListe.Columns["idPublic"].Visible = false;
            DgvLesLivresListe.Columns["image"].Visible = false;
            DgvLesLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DgvLesLivresListe.Columns["id"].DisplayIndex = 0;
            DgvLesLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// affiche la liste des commandes associé a un livre
        /// </summary>
        /// <param name="lesCommandes">liste des commandes d'un livres</param>
        private void RemplirLivresComListeCommandes(List<CommandeDocument> lesCommandes)
        {
            if (lesCommandes.Count > 0)
            {
                bdgListeCommandeLivre.DataSource = lesCommandes;
                dgvLivresCommande.DataSource = bdgListeCommandeLivre;
                dgvLivresCommande.Columns["idLivreDvd"].Visible = false;
                dgvLivresCommande.Columns["idSuivi"].Visible = false;
                dgvLivresCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLivresCommande.Columns["id"].DisplayIndex = 0;
                dgvLivresCommande.Columns["dateCommande"].DisplayIndex = 1;
                dgvLivresCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvLivresCommande.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComLivresNumRecherche.Text.Equals(""))
            {
                txbComLivresTitreRecherche.Text = "";
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                Livre livre = lesComLivres.Find(x => x.Id.Equals(txbComLivresNumRecherche.Text));
                if (livre!= null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirComLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirComLivresListeComplete();
                }
            }
            else
            {
                RemplirComLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche avec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbComLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbComLivresTitreRecherche.Text.Equals(""))
            {
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                txbComLivresNumRecherche.Text = "";
                List<Livre> ComLivresParTitre;
                ComLivresParTitre = lesComLivres.FindAll(x => x.Titre.ToLower().Contains(txbComLivresTitreRecherche.Text.ToLower()));
                RemplirComLivresListe(ComLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxComLivresGenres.SelectedIndex < 0 && cbxComLivresPublics.SelectedIndex < 0 && cbxComLivresRayons.SelectedIndex < 0
                    && txbComLivresNumRecherche.Text.Equals(""))
                {
                    RemplirComLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheComLivresInfos(Livre livre)
        {
            txbComLivresAuteur.Text = livre.Auteur;
            txbComLivresCollection.Text = livre.Collection;
            txbComLivresImage.Text = livre.Image;
            txbComLivresIsbn.Text = livre.Isbn;
            txbComLivresNumero.Text = livre.Id;
            cbxComLivresGenreInfos.SelectedIndex = cbxComLivresGenreInfos.FindString(livre.Genre);
            cbxComLivresPublicInfos.SelectedIndex = cbxComLivresPublicInfos.FindString(livre.Public);
            cbxComLivresRayonInfos.SelectedIndex = cbxComLivresRayonInfos.FindString(livre.Rayon);
            txbComLivresTitre.Text = livre.Titre;
            string image = livre.Image;

        }



        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresGenres.SelectedIndex >= 0)
            {
                txbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                DgvLesLivresListe.ClearSelection();
                Genre genre = (Genre)cbxComLivresGenres.SelectedItem;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                List<Livre> livres = lesComLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComLivresListe(livres);
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresPublics.SelectedIndex >= 0)
            {
                txbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                DgvLesLivresListe.ClearSelection();
                Public lePublic = (Public)cbxComLivresPublics.SelectedItem;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresGenres.SelectedIndex = -1;
                List<Livre> livres = lesComLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirComLivresListe(livres);
                
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresRayons.SelectedIndex >= 0)
            {
                txbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                DgvLesLivresListe.ClearSelection();
                Rayon rayon = (Rayon)cbxComLivresRayons.SelectedItem;
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                List<Livre> livres = lesComLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComLivresListe(livres);
                
            }
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresCommandeInfos(Livre livre)
        {
            string idLivre = livre.Id;
            VideLivresComInfos();
            Console.WriteLine("test d'appel de methode");
            lesCommandes = controller.GetCommandesLivres(idLivre);
            GrpLivreCommande.Text = livre.Titre + " de " + livre.Auteur;
            Console.WriteLine("lesCommandes.count = " + lesCommandes.Count.ToString());

            if (lesCommandes.Count == 0)
                VideLivresComInfos();
            RemplirLivresComListeCommandes(lesCommandes);
        }





        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirComLivresListeComplete()
        {
            RemplirComLivresListe(lesComLivres);
            VideComLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideComLivresZones()
        {
            cbxComLivresGenres.SelectedIndex = -1;
            cbxComLivresRayons.SelectedIndex = -1;
            cbxComLivresPublics.SelectedIndex = -1;
            txbComLivresNumRecherche.Text = "";
            txbComLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresComZones()
        {
            cbxComLivresGenres.SelectedIndex = -1;
            cbxComLivresRayons.SelectedIndex = -1;
            cbxComLivresPublics.SelectedIndex = -1;
            txbComLivresNumRecherche.Text = "";
            txbComLivresTitreRecherche.Text = "";
            GrpLivreCommande.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideLivresComInfos()
        {
            txbCommandeNumeroCommande.Text = "";
            DtpComandeLivre.Value = DateTime.Now.Date;
            txbCommandeLivreMontant.Text = "";
            txbCommandeExemplaireLivre.Text = "";
            cbxCommandeLivreEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void CommandeLivreEnCoursDeModif(bool modif)
        {
            BtnLivreAjouterCom.Enabled = !modif;
            BtnLivreSupprimerCom.Enabled = !modif;
            BtnLivreModifierCom.Enabled = !modif;
            BtnAnnulerChoix.Enabled = modif;
            BtnValiderChoix.Enabled = modif;
            BtnComLivresNumRecherche.Enabled = !modif;
            BtnComLivresAnnulGenres.Enabled = !modif;
            BtnComLivresAnnulPublic.Enabled = !modif;
            BtnComLivresAnnulRayon.Enabled = !modif;
            txbLivresTitre.ReadOnly = !modif;
            txbLivresAuteur.ReadOnly = !modif;
            txbComLivresNumRecherche.Enabled = !modif;
            txbComLivresTitreRecherche.Enabled = !modif;
            cbxLivresPublicInfos.Enabled = modif;
            txbComLivresNumero.ReadOnly = !modif;
            txbLivresCollection.ReadOnly = !modif;
            cbxLivresGenreInfos.Enabled = modif;
            cbxLivresRayonInfos.Enabled = modif;
            txbLivresImage.ReadOnly = !modif;
            txbComLivresNumero.ReadOnly = true;
            DgvLesLivresListe.Enabled = !modif;
            dgvLivresCommande.Enabled = !modif;
            txbCommandeNumeroCommande.ReadOnly = true;
            DtpComandeLivre.Enabled = !modif;
            txbCommandeLivreMontant.ReadOnly = !modif;
            txbCommandeExemplaireLivre.ReadOnly = !modif;
            txbCommandeNumeroLivre.ReadOnly = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// affiche les détails d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheLivresComInfo(CommandeDocument laCommande)
        {
            txbCommandeNumeroCommande.Text = laCommande.Id;
            txbCommandeNumeroLivre.Text = laCommande.IdLivreDvd;
            DtpComandeLivre.Value = laCommande.DateCommande;
            txbCommandeLivreMontant.Text = laCommande.Montant.ToString();
            txbCommandeExemplaireLivre.Text = laCommande.NbExemplaire.ToString();
            cbxCommandeLivreEtat.SelectedIndex = cbxCommandeLivreEtat.FindString(laCommande.Etat);
        }


        /// <summary>
        /// démarre la procédure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAjouter_Click(object sender, EventArgs e)
        {
            CommandeLivreEnCoursDeModif(true);
            txbCommandeNumeroLivre.ReadOnly = true;
            ajouterBool = true;
            string id = plusUnIdString(controller.getNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideLivresComInfos();
            cbxCommandeLivreEtat.SelectedIndex = 0;
            txbCommandeNumeroCommande.Text = id;
            cbxCommandeLivreEtat.Enabled = false;
            
        }

        /// <summary>
        /// démarre la procédure de modification de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresCommande.CurrentCell != null && txbCommandeNumeroCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxCommandeLivreEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                CommandeLivreEnCoursDeModif(true);
                RemplirComboSuivi(lesSuivi, bdgComLivreEtat, cbxCommandeLivreEtat);
                cbxCommandeLivreEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionné");
            }
        }

        /// <summary>
        /// supprime une commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeLivre[bdgListeCommandeLivre.Position];
            if (dgvLivresCommande.CurrentCell != null && txbCommandeNumeroCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sur de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesComLivres.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {

                        try
                        {
                            Livre livre = (Livre)bdgLesLivresListe.List[bdgLesLivresListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbCommandeNumeroCommande.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une commande");
            }



        }


        /// <summary>
        /// annule la modifications ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
            CommandeLivreEnCoursDeModif(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeLivre[bdgListeCommandeLivre.Position];
                AfficheLivresComInfo(commandeDocument);
            }
            catch
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbCommandeNumeroCommande.Text;
                bool checkValid = false;
                DateTime dateCommande = DtpComandeLivre.Value;
                float montant = -1;
                int nbExemplaire = -1;
                try
                {
                    montant = float.Parse(txbCommandeLivreMontant.Text);
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un nombre a virgule");
                }
                try
                {
                    nbExemplaire = int.Parse(txbCommandeExemplaireLivre.Text);
                }
                catch
                {
                    MessageBox.Show("Le nombre d'exemplaire doit etre un nombre a entier");
                }
                string idLivreDvd = txbCommandeNumeroLivre.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxCommandeLivreEtat.SelectedItem;
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                else
                    MessageBox.Show("Veuillez selectionner un etat");
                if (montant != -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    if (!ajouterBool)
                        checkValid = controller.ModifierLivreDvdCom(commandeLivre);
                    else
                        checkValid = controller.CreerLivreDvdCom(commandeLivre);
                    if (checkValid)
                    {
                        if (!ajouterBool)
                            RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
                        CommandeLivreEnCoursDeModif(false);
                        try
                        {
                            Livre livre = (Livre)bdgLesLivresListe[bdgLesLivresListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbCommandeNumeroCommande.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }

                    }
                    else
                        MessageBox.Show("Erreur");
                }

            }

        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage les commandes relative a un livre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLesLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvLesLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLesLivresListe.List[bdgLesLivresListe.Position];
                    AfficheLivresCommandeInfos(livre);
                    txbComLivresNumero.Text = livre.Id;
                }
                catch
                {
                    VideComLivresZones();
                }
            }
            else
            {
                txbCommandeNumeroLivre.Text = "";
                VideLivresComInfos();

            }
            if (DgvLesLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLesLivresListe.List[bdgLesLivresListe.Position];
                    AfficheComLivresInfos(livre);
                }
                catch
                {
                    VideComLivresZones();
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLesLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComLivresZones();
            string titreColonne = DgvLesLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesComLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesComLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesComLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirComLivresListe(sortedList);
        }

        

       
        ///<summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage les information d'une commande .
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        private void dgvLivresCommande_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresCommande.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeLivre[bdgListeCommandeLivre.Position];
                    AfficheLivresComInfo(commandeDocument);
                }
                catch
                {
                    VideLivresComInfos();
                }

            }
            else
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresCommande_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandes.Count > 0 && dgvLivresCommande != null)
            {
                VideLivresComInfos();
                string titreColonne = dgvLivresCommande.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "IdLivre":
                        sortedList = lesCommandes.OrderBy(o => o.Id).ToList();
                        
                        break;
                    case "DateCommande":
                        sortedList = lesCommandes.OrderBy(o => o.DateCommande).ToList();
                        
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandes.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandes.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandes.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirLivresComListeCommandes(sortedList);
            }
        }

#endregion

#region Onglet Commande Dvd

        private readonly BindingSource bdgComDvdListe = new BindingSource();
        private List<Dvd> lesComDvd = new List<Dvd>();
        private readonly BindingSource bdgComDvdGenreInfos = new BindingSource();
        private readonly BindingSource bdgComDvdPublicInfos = new BindingSource();
        private readonly BindingSource bdgComDvdRayonInfos = new BindingSource();
        private readonly BindingSource bdgCommandeListeDvd = new BindingSource();
        private readonly BindingSource bdgCommandeEtatDvd = new BindingSource();
        private List<CommandeDocument> lesCommandesDvd = new List<CommandeDocument>();


        ///<summary>
        ///Ouverture de l'onglet de commande de dvd:
        ///appel des methodes pour remplir les datagrid et les combos 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeDvd_Enter(object sender, EventArgs e)
        {
            lesComDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComDvdRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgComDvdGenreInfos, cbxComDvdGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgComDvdPublicInfos, cbxComDvdPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgComDvdRayonInfos, cbxComDvdRayonInfos);
            RemplirComboSuivi(controller.GetAllSuivis(), bdgCommandeEtatDvd, cbxCommandeDvdEtat);
            CommandeDvdEnCoursDeModif(false);
            RemplirComDvdListeComplete();
        }

        ///<summary>
        ///Rempli le datagrid de la liste des dvd
        /// </summary>
        /// <param name="lesComDvd">Liste des dvd</param>
        private void RemplirComDvdListe(List<Dvd> lesComDvd)
        {
            bdgComDvdListe.DataSource = lesComDvd;
            dgvComDvdListe.DataSource = bdgComDvdListe;
            dgvComDvdListe.Columns["idGenre"].Visible = false;
            dgvComDvdListe.Columns["idPublic"].Visible = false;
            dgvComDvdListe.Columns["idRayon"].Visible = false;
            dgvComDvdListe.Columns["image"].Visible = false;
            dgvComDvdListe.Columns["synopsis"].Visible = false;
            dgvComDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComDvdListe.Columns["id"].DisplayIndex = 0;
            dgvComDvdListe.Columns["titre"].DisplayIndex = 1;

        }

        ///<summary>
        ///Rempli le datagrid concernant les commandes des dvd
        /// </summary>
        /// <param name="LesCommandes"></param>
        private void RemplirCommandeDvdListe(List<CommandeDocument> lesCommandes)
        {
            if(lesCommandes.Count > 0)
            {
                bdgCommandeListeDvd.DataSource = lesCommandes;
                dgvCommandeDvd.DataSource = bdgCommandeListeDvd;
                dgvCommandeDvd.Columns["idLivreDvd"].Visible = false;
                dgvCommandeDvd.Columns["idSuivi"].Visible = false;
                dgvCommandeDvd.Columns["id"].Visible = false;
                dgvCommandeDvd.Columns["dateCommande"].Visible = false;
                dgvCommandeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvCommandeDvd.ClearSelection();
            }
            
        }

        ///<summary>
        ///Recherche et affiche le dvd dont on a saisi le numero
        ///Si non, affichage d'un message box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComDvdNumRecherche.Text.Equals(""))
            {
                txbComDvdTitreRecherche.Text = "";
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                cbxComDvdRayons.SelectedIndex = -1;
                Dvd ComDvd = lesComDvd.Find(x => x.Id.Equals(txbComDvdNumRecherche.Text));
                if (ComDvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { ComDvd };
                    RemplirComDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("Numero introuvable");
                    RemplirComDvdListeComplete();
                }
            }
            else
            {
                RemplirComDvdListeComplete();
            }
        }

        ///<summary>
        ///Recherche et affiche les dvd dont le titre match avec la saisie.
        ///Cette procedure est exécute a chauqe ou suppression de caractere
        ///dans le textbox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbComDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbComDvdTitreRecherche.Text.Equals(""))
            {
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                txbComDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesComDvd.FindAll(x => x.Titre.ToLower().Contains(txbComDvdTitreRecherche.Text.ToLower()));
                RemplirComDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxComDvdGenres.SelectedIndex < 0 && cbxComDvdPublics.SelectedIndex < 0 && cbxComDvdRayons.SelectedIndex < 0
                    && txbComDvdNumRecherche.Text.Equals(""))
                {
                    RemplirComDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheComDvdInfos(Dvd dvd)
        {
            txbComDvdRealisateur.Text = dvd.Realisateur;
            txbComDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbComDvdDuree.Text = dvd.Duree.ToString();
            txbComDvdNumero.Text = dvd.Id;
            cbxComDvdGenreInfos.SelectedIndex = cbxComDvdGenreInfos.FindString(dvd.Genre);
            cbxComDvdPublicInfos.SelectedIndex = cbxComDvdPublicInfos.FindString(dvd.Public);
            cbxComDvdRayonInfos.SelectedIndex = cbxComDvdRayonInfos.FindString(dvd.Rayon);
            txbComDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            
        }

        ///<summary>
        ///Affiche les informations detaillé d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficherInfoCommandeDvd(CommandeDocument laCommande)
        {
            txbComDvdNumero.Text = laCommande.Id;
            txbCommandeDvdId.Text = laCommande.IdLivreDvd;
            dtpCommandeDvdDate.Value = laCommande.DateCommande;
            txbCommandeDvdMontant.Text = laCommande.Montant.ToString();
            txbCommandeDvdNbExemplaire.Text = laCommande.NbExemplaire.ToString();
            txbCommandeDvdId.Text = laCommande.IdLivreDvd;
            cbxCommandeDvdEtat.SelectedIndex = cbxCommandeDvdEtat.FindString(laCommande.IdSuivi.ToString());
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideComDvdInfos()
        {
            txbComDvdRealisateur.Text = "";
            txbComDvdSynopsis.Text = "";
            txbComDvdImage.Text = "";
            txbComDvdDuree.Text = "";
            txbComDvdNumero.Text = "";
            cbxComDvdGenreInfos.SelectedIndex = -1;
            cbxComDvdPublicInfos.SelectedIndex = -1;
            cbxComDvdRayonInfos.SelectedIndex = -1;
            txbComDvdTitre.Text = "";

        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxComDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdGenres.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxComDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComDvdListe(Dvd);
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxComDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdPublics.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxComDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxComDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdRayons.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxComDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesComDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComDvdListe(Dvd);
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdComAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirComDvdListeComplete()
        {
            RemplirComDvdListe(lesComDvd);
            VideComDvdZones();
        }

        ///<summary>
        ///Recupere et affiche les commandes d'un dvd
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficherCommandeDvdInfos(Dvd dvd)
        {
            string idDvd = dvd.Id;
            VideComDvdInfos();
            Console.WriteLine("test appel de methode");
            lesCommandes = controller.GetCommandesDvd(idDvd);
            GrpDvdCommande.Text = dvd.Titre + " de " + dvd.Realisateur;
            Console.WriteLine("lesCommandes.count = " + lesCommandes.Count.ToString());
            if (lesCommandes.Count == 0)
                VideComDvdInfos();
            RemplirCommandeDvdListe(lesCommandes);

        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideComDvdZones()
        {
            cbxComDvdGenres.SelectedIndex = -1;
            cbxComDvdRayons.SelectedIndex = -1;
            cbxComDvdPublics.SelectedIndex = -1;
            txbComDvdNumRecherche.Text = "";
            txbComDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void CommandeDvdEnCoursDeModif(bool modif)
        {
            BtnAjouterDvdCom.Enabled = !modif;
            BtnSupprimerDvdCom.Enabled = !modif;
            BtnModifierDvdCom.Enabled = !modif;
            BtnAnnulerDvdCom.Enabled = modif;
            BtnValiderDvdCom.Enabled = modif;
            txbComDvdTitre.ReadOnly = !modif;
            txbComDvdRealisateur.ReadOnly = !modif;
            txbComDvdSynopsis.ReadOnly = !modif;
            cbxComDvdPublicInfos.Enabled = modif;
            txbComDvdDuree.ReadOnly = !modif;
            cbxComDvdGenreInfos.Enabled = modif;
            cbxComDvdRayonInfos.Enabled = modif;
            txbComDvdImage.ReadOnly = !modif;
            dgvComDvdListe.Enabled = !modif;
            txbComDvdNumero.ReadOnly = true;
            txbComDvdNumero.ReadOnly = true;
            dgvComDvdListe.Enabled = !modif;
            dgvCommandeDvd.Enabled = !modif;
            txbCommandeDvdNumero.ReadOnly = true;
            dtpCommandeDvdDate.Enabled = !modif;
            txbCommandeDvdMontant.Enabled = !modif;
            txbCommandeDvdNbExemplaire.Enabled = !modif;
            txbCommandeDvdId.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// lance la procédure d'ajout d'une commande de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComAjouterDvd_Click(object sender, EventArgs e)
        {
            CommandeDvdEnCoursDeModif(true);
            txbComDvdNumero.ReadOnly = false;
            VideComDvdInfos();
        }

        /// <summary>
        /// lance la procédure de modification de la commande du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComModifierDvd_Click(object sender, EventArgs e)
        {
            CommandeDvdEnCoursDeModif(true);
        }

        ///<summary>
        ///Supprime une commande de Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerDvdCom_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgComDvdListe[bdgComDvdListe.Position];
            if (dgvCommandeDvd.CurrentCell != null && txbCommandeDvdNumero.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                {
                    MessageBox.Show("Une commande livrée ou reglé ne peut pas etre supprimée");

                }
                else if (MessageBox.Show($"Êtes vous sur de vouloir supprimer la commande n° {commandeDocument.Id} concernant" +
                    $" {lesComDvd.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre} ?", "validation suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {
                        try
                        {
                            Dvd dvd = (Dvd)bdgComDvdListe.List[bdgComDvdListe.Position];
                            AfficherCommandeDvdInfos(dvd);
                            txbCommandeDvdNumero.Text = dvd.Id;
                        }
                        catch
                        {
                            VideComDvdZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Veuillez saisir une commande");
            }
        }

        ///<summary>
        ///Annule la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnulerDvdCom_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgCommandeEtatDvd, cbxCommandeDvdEtat);
            CommandeDvdEnCoursDeModif(false);

            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandeListeDvd[bdgCommandeListeDvd.Position];
                AfficherInfoCommandeDvd(commandeDocument);
            }
            catch
            {
                VideComDvdInfos();
            }
        }

        ///<summary>
        ///Valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValiderDvdCom_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sur ?","oui ?",  MessageBoxButtons.YesNo) == DialogResult.OK)
            {
                string id = txbComDvdNumero.Text;
                bool valide = false;
                DateTime dateCommande = dtpCommandeDvdDate.Value;
                float montant = -1;
                int nbExemplaire = -1;
                try
                {
                    montant = float.Parse(txbCommandeDvdMontant.Text);
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un chiffre a virgule.");

                }
                try
                {
                    nbExemplaire = int.Parse(txbCommandeDvdNbExemplaire.Text);
                }
                catch
                {
                    MessageBox.Show("Le nombre d'exemplaire doit etre un entier.");

                }
                string idLivreDvd = txbCommandeDvdId.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxCommandeDvdEtat.SelectedItem;
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                else
                {
                    MessageBox.Show("Veuillez selectionner un etat");
                    if (montant != -1 && nbExemplaire != -1 && etat != "")
                    {
                        CommandeDocument commandeDvd = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                        if (!ajouterBool)
                            valide = controller.ModifierLivreDvdCom(commandeDvd);
                        else
                            valide = controller.CreerLivreDvdCom(commandeDvd);
                        if (valide)
                        {
                            if (!ajouterBool)
                                RemplirComboSuivi(controller.GetAllSuivis(), bdgCommandeEtatDvd, cbxCommandeDvdEtat);
                            CommandeDvdEnCoursDeModif(false);
                            try
                            {
                                Dvd dvd = (Dvd)bdgCommandeListeDvd.List[bdgCommandeListeDvd.Position];
                                AfficherCommandeDvdInfos(dvd);
                                txbCommandeDvdId.Text = dvd.Id;
                            }
                            catch
                            {
                                VideDvdZones();
                            }
                        }
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        
        }
        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvComDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComDvdZones();
            string titreColonne = dgvComDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesComDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Genre":
                    sortedList = lesComDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du detaile de la commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvComDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandeDvd.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgCommandeListeDvd[bdgCommandeListeDvd.Position];
                    AfficherInfoCommandeDvd(commandeDocument);
                     
                }
                catch
                {
                    
                    VideComDvdInfos();
                }
            }
            else
            {
                VideComDvdInfos();
            }
            if (dgvComDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgComDvdListe.List[bdgComDvdListe.Position];
                    AfficherCommandeDvdInfos(dvd);

                }
                catch
                {
                    VideComDvdZones();
                }
            }
            else
            {
                VideComDvdInfos();
            }
        }


        private void dgvCommandeDvd_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvCommandeDvd.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgCommandeListeDvd[bdgCommandeListeDvd.Position];
                    AfficherInfoCommandeDvd(commandeDocument);
                }
                catch
                {
                    VideComDvdInfos();
                }
            }
            else
            {
                VideComDvdInfos();
            }
        }
        /// <summary>
        /// Permet le tri sur le datagrid des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandesDvd.Count > 0 && dgvCommandeDvd != null)
            {
                VideComDvdInfos();
                string titreColonne = dgvCommandeDvd.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Id).ToList();
                        
                        break;
                    case "DateCommande":
                        sortedList = lesCommandesDvd.OrderBy(o => o.DateCommande).ToList();
                        
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandesDvd.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandesDvd.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirCommandeDvdListe(sortedList);
            }
        }
#endregion

    }
}
