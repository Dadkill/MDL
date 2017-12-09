using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.ObjectModel;
using ComposantNuite;
using BaseDeDonnees;

namespace MaisonDesLigues
{
    public partial class FrmPrincipale : Form
    {

        /// <summary>
        /// constructeur du formulaire
        /// </summary>
        public FrmPrincipale()
        {
            InitializeComponent();
        }
        private Bdd UneConnexion;
        private String TitreApplication;
        private String IdStatutSelectionne = "";
        /// <summary>
        /// création et ouverture d'une connexion vers la base de données sur le chargement du formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPrincipale_Load(object sender, EventArgs e)
        {
            UneConnexion = ((FrmLogin)Owner).UneConnexion;
            TitreApplication = ((FrmLogin)Owner).TitreApplication;
            this.Text = TitreApplication;
        }

        //======================================================
        //------------------------------------------------------
        //-----------------PARTIE INSCRIPTION-------------------
        //------------------------------------------------------
        //======================================================
        /// <summary>
        /// gestion de l'événement click du bouton quitter.
        /// Demande de confirmation avant de quitetr l'application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdQuitter_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez-vous quitter l'application ?", ConfigurationManager.AppSettings["TitreApplication"], MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                UneConnexion.FermerConnexion();
                Application.Exit();
            }
        }

        private void RadTypeParticipant_Changed(object sender, EventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "RadBenevole":
                    this.GererInscriptionBenevole();
                    break;
                case "RadLicencie":
                    this.GererInscriptionLicencie();
                    break;
                case "RadIntervenant":
                    this.GererInscriptionIntervenant();
                    break;
                default:
                    throw new Exception("Erreur interne à l'application");
            }
        }

        //------------------------------------------------------
        //-----------------Choix Intervenant--------------------
        //------------------------------------------------------

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie du complément d'inscription d'un intervenant.
        /// </summary>
        private void GererInscriptionIntervenant()
        {

            GrpBenevole.Visible = false;
            GrpIntervenant.Visible = true;
            GrpLicencie.Visible = false;
            PanFonctionIntervenant.Visible = true;
            GrpIntervenant.Left = 23;
            GrpIntervenant.Top = 264;
            Utilitaire.CreerDesControles(this, UneConnexion, "STATUT", "Rad_", PanFonctionIntervenant, "RadioButton", this.rdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(UneConnexion, CmbAtelierIntervenant, "VATELIER01");

            CmbAtelierIntervenant.Text = "Choisir";

        }

        /// <summary>
        /// permet d'appeler la méthode VerifBtnEnregistreIntervenant qui déterminera le statut du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbStatutIntervenant_StateChanged(object sender, EventArgs e)
        {
            // stocke dans un membre de niveau form l'identifiant du statut sélectionné (voir règle de nommage des noms des controles : prefixe_Id)
            this.IdStatutSelectionne = ((RadioButton)sender).Name.Split('_')[1];
            BtnEnregistrerIntervenant.Enabled = VerifBtnEnregistreIntervenant();
        }

        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle panel permettant la saisie des nuités d'un intervenant.
        /// S'il faut rendre visible le panel, on teste si les nuités possibles ont été chargés dans ce panel. Si non, on les charges 
        /// On charge ici autant de contrôles ResaNuit qu'il y a de nuits possibles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdbNuiteIntervenant_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "RdbNuiteIntervenantOui")
            {
                PanNuiteIntervenant.Visible = true;
                if (PanNuiteIntervenant.Controls.Count == 0) // on charge les nuites possibles possibles et on les affiche
                {
                    DataTable LesDATENUITEEs = UneConnexion.ObtenirDonnees("VDATENUITEE02");
                    //foreach(Dat
                    Dictionary<Int16, String> LesNuites = UneConnexion.ObtenirDatesNuitees();
                    int i = 0;
                    foreach (KeyValuePair<Int16, String> UneNuite in LesNuites)
                    {
                        ComposantNuite.ResaNuite unResaNuit = new ResaNuite(UneConnexion.ObtenirDonnees("VHOTEL01"), (UneConnexion.ObtenirDonnees("VCATEGORIECHAMBRE01")), UneNuite.Value, UneNuite.Key);
                        unResaNuit.Left = 5;
                        unResaNuit.Top = 5 + (24 * i++);
                        unResaNuit.Visible = true;
                        //unResaNuit.click += new System.EventHandler(ComposantNuite_StateChanged);
                        PanNuiteIntervenant.Controls.Add(unResaNuit);
                    }

                }

            }
            else
            {
                PanNuiteIntervenant.Visible = false;
            }
            BtnEnregistrerIntervenant.Enabled = VerifBtnEnregistreIntervenant();

        }

        /// <summary>
        /// Cette procédure va appeler la procédure .... qui aura pour but d'enregistrer les éléments 
        /// de l'inscription d'un intervenant, avec éventuellement les nuités à prendre en compte        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnregistrerIntervenant_Click(object sender, EventArgs e)
        {
            try
            {
                if (RdbNuiteIntervenantOui.Checked)
                {
                    // inscription avec les nuitées
                    Collection<Int16> NuitsSelectionnes = new Collection<Int16>();
                    Collection<String> HotelsSelectionnes = new Collection<String>();
                    Collection<String> CategoriesSelectionnees = new Collection<string>();
                    foreach (Control UnControle in PanNuiteIntervenant.Controls)
                    {
                        if (UnControle.GetType().Name == "ResaNuite" && ((ResaNuite)UnControle).GetNuitSelectionnee())
                        {
                            // la nuité a été cochée, il faut donc envoyer l'hotel et le type de chambre à la procédure de la base qui va enregistrer le contenu hébergement 
                            //ContenuUnHebergement UnContenuUnHebergement= new ContenuUnHebergement();
                            CategoriesSelectionnees.Add(((ResaNuite)UnControle).GetTypeChambreSelectionnee());
                            HotelsSelectionnes.Add(((ResaNuite)UnControle).GetHotelSelectionne());
                            NuitsSelectionnes.Add(((ResaNuite)UnControle).IdNuite);
                        }

                    }
                    if (NuitsSelectionnes.Count == 0)
                    {
                        MessageBox.Show("Si vous avez sélectionné que l'intervenant avait des nuités\n in faut qu'au moins une nuit soit sélectionnée");
                    }
                    else
                    {
                        UneConnexion.InscrireIntervenant(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : "", TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text : "", TxtMail.Text != "" ? TxtMail.Text : "", System.Convert.ToInt16(CmbAtelierIntervenant.SelectedValue), this.IdStatutSelectionne, CategoriesSelectionnees, HotelsSelectionnes, NuitsSelectionnes);
                        MessageBox.Show("Inscription intervenant avec nuitées effectuée");
                        if (TxtMail.Text != null)
                        {
                            Utilitaire.envoyerEmail(TxtMail.Text.ToString(), "Votre enregistrement en tant qu'intervenant a été effectué", "Bonjour, ce mail vous est envoyé pour vous confirmer votre inscription en tant qu'intervenant durant l'événement d'escrime.");
                        }
                        Utilitaire.resetTextbox(GrpIdentite);
                        Utilitaire.resetTextbox(GrpIntervenant);
                    }
                }
                else
                { // inscription sans les nuitées
                    UneConnexion.InscrireIntervenant(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : "", TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text : "", TxtMail.Text != "" ? TxtMail.Text : "", System.Convert.ToInt16(CmbAtelierIntervenant.SelectedValue), this.IdStatutSelectionne);
                    MessageBox.Show("Inscription intervenant sans nuitée effectuée");
                    if (TxtMail.Text != null)
                    {
                        Utilitaire.envoyerEmail(TxtMail.Text.ToString(), "Votre enregistrement en tant qu'intervenant a été effectué", "Bonjour, ce mail vous est envoyé pour vous confirmer votre inscription en tant qu'intervenant durant l'événement d'escrime.");
                    }
                    Utilitaire.resetTextbox(GrpIdentite);
                    Utilitaire.resetTextbox(GrpIntervenant);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// Méthode permettant de définir le statut activé/désactivé du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbAtelierIntervenant_TextChanged(object sender, EventArgs e)
        {
            BtnEnregistrerIntervenant.Enabled = VerifBtnEnregistreIntervenant();
        }

        /// <summary>
        /// Méthode privée testant le contrôle combo et la variable IdStatutSelectionne qui contient une valeur
        /// Cette méthode permetra ensuite de définir l'état du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <returns></returns>
        private Boolean VerifBtnEnregistreIntervenant()
        {
            return CmbAtelierIntervenant.Text != "Choisir" && this.IdStatutSelectionne.Length > 0;
        }

        //------------------------------------------------------
        //-----------------Choix Licencié-----------------------
        //------------------------------------------------------

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie du complément d'inscription d'un licencié.
        /// </summary>
        private void GererInscriptionLicencie()
        {
            GrpBenevole.Visible = false;
            GrpIntervenant.Visible = false;
            GrpLicencie.Visible = true;
            GrpLicencie.Left = 23;
            GrpLicencie.Top = 264;
            Utilitaire.RemplirComboBox(UneConnexion, CmbQualiteLicencie, "VQUALITE01");
            Utilitaire.RemplirListBox(UneConnexion, LsbLicencieChoixAteliers, "VATELIER01");
            LsbLicencieChoixAteliers.SelectedIndex = -1;

            CmbQualiteLicencie.SelectedIndex = -1;
            CmbQualiteLicencie.Text = "Choisir";

        }

        /// <summary>
        /// Méthode qui permet d'afficher ou masquer le controle panel permettant la saisie des nuités d'un licencié.
        /// S'il faut rendre visible le panel, on teste si les nuités possibles ont été chargés dans ce panel. Si non, on les charges 
        /// On charge ici autant de contrôles ResaNuit qu'il y a de nuits possibles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdbNuiteLicencie_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "RdbNuiteLicencieOui")
            {
                PanNuiteLicencie.Visible = true;
                if (PanNuiteLicencie.Controls.Count == 0) // on charge les nuites possibles possibles et on les affiche
                {
                    DataTable LesDATENUITEEs = UneConnexion.ObtenirDonnees("VDATENUITEE02");
                    //foreach(Dat
                    Dictionary<Int16, String> LesNuites = UneConnexion.ObtenirDatesNuitees();
                    int i = 0;
                    foreach (KeyValuePair<Int16, String> UneNuite in LesNuites)
                    {
                        ComposantNuite.ResaNuite unResaNuit = new ResaNuite(UneConnexion.ObtenirDonnees("VHOTEL01"), (UneConnexion.ObtenirDonnees("VCATEGORIECHAMBRE01")), UneNuite.Value, UneNuite.Key);
                        unResaNuit.Left = 5;
                        unResaNuit.Top = 5 + (24 * i++);
                        unResaNuit.Visible = true;
                        //unResaNuit.click += new System.EventHandler(ComposantNuite_StateChanged);
                        PanNuiteLicencie.Controls.Add(unResaNuit);
                    }
                }
            }
            else
            {
                PanNuiteLicencie.Visible = false;
            }
        }

        private void VerifBtnEnregistreLicencie()
        {
            BtnEnregistrerLicencie.Enabled = (CmbQualiteLicencie.SelectedIndex != -1 && TxtLicenceLicencie.MaskCompleted && TxtNumeroChequeComplet.MaskCompleted && TxtMontantChequeComplet.Value != 0 && !string.IsNullOrEmpty(TxtMontantChequeComplet.Text) && LsbLicencieChoixAteliers.SelectedIndices.Count != 0 || CmbQualiteLicencie.SelectedIndex != -1 && TxtLicenceLicencie.MaskCompleted && TxtNumeroChequeInscription.MaskCompleted && TxtNumeroChequeAccompagnant.MaskCompleted && TxtMontantChequeInscription.Value != 0 && TxtMontantChequeAccompagnant.Value != 0 && !string.IsNullOrEmpty(TxtMontantChequeInscription.Text) && !string.IsNullOrEmpty(TxtMontantChequeAccompagnant.Text) && LsbLicencieChoixAteliers.SelectedIndices.Count != 0);
        }

        private void RdbPaiementLicencie_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Name == "RdbPaiementLicencieComplet")
            {
                TxtNumeroChequeComplet.Enabled = true;
                TxtMontantChequeComplet.Enabled = true;
                TxtNumeroChequeInscription.Enabled = false;
                TxtNumeroChequeInscription.Text = "";
                TxtNumeroChequeAccompagnant.Enabled = false;
                TxtNumeroChequeAccompagnant.Text = "";
                TxtMontantChequeInscription.Enabled = false;
                TxtMontantChequeInscription.Text = "0";
                TxtMontantChequeAccompagnant.Enabled = false;
                TxtMontantChequeAccompagnant.Text = "0";
            }
            else
            {
                TxtNumeroChequeComplet.Enabled = false;
                TxtMontantChequeComplet.Enabled = false;
                TxtNumeroChequeComplet.Text = "";
                TxtMontantChequeComplet.Text = "0";
                TxtNumeroChequeInscription.Enabled = true;
                TxtMontantChequeInscription.Enabled = true;
                TxtNumeroChequeAccompagnant.Enabled = true;
                TxtMontantChequeAccompagnant.Enabled = true;
            }
        }

        private void BtnEnregistrerLicencie_Click(object sender, EventArgs e)
        {

        }

        //------------------------------------------------------
        //-----------------Choix Bénévole-----------------------
        //------------------------------------------------------

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie des disponibilités des bénévoles.
        /// </summary>
        private void GererInscriptionBenevole()
        {
            GrpBenevole.Visible = true;
            GrpIntervenant.Visible = false;
            GrpLicencie.Visible = false;
            GrpBenevole.Left = 23;
            GrpBenevole.Top = 264;
            

            Utilitaire.CreerDesControles(this, UneConnexion, "VDATEBENEVOLAT01", "ChkDateB_", PanelDispoBenevole, "CheckBox", this.rdbStatutIntervenant_StateChanged);
            // on va tester si le controle à placer est de type CheckBox afin de lui placer un événement checked_changed
            // Ceci afin de désactiver les boutons si aucune case à cocher du container n'est cochée
            foreach (Control UnControle in PanelDispoBenevole.Controls)
            {
                if (UnControle.GetType().Name == "CheckBox")
                {
                    CheckBox UneCheckBox = (CheckBox)UnControle;
                    UneCheckBox.CheckedChanged += new System.EventHandler(this.ChkDateBenevole_CheckedChanged);
                }
            }


        }
        
        /// <summary>
        /// Permet d'intercepter le click sur le bouton d'enregistrement d'un bénévole.
        /// Cetteméthode va appeler la méthode InscrireBenevole de la Bdd, après avoir mis en forme certains paramètres à envoyer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnregistreBenevole_Click(object sender, EventArgs e)
        {
            Collection<Int16> IdDatesSelectionnees = new Collection<Int16>();
            Int64? NumeroLicence;
            if (TxtLicenceBenevole.MaskCompleted)
            {
                NumeroLicence = System.Convert.ToInt64(TxtLicenceBenevole.Text);
            }
            else
            {
                NumeroLicence = null;
            }


            foreach (Control UnControle in PanelDispoBenevole.Controls)
            {
                if (UnControle.GetType().Name == "CheckBox" && ((CheckBox)UnControle).Checked)
                {
                    /* Un name de controle est toujours formé come ceci : xxx_Id où id représente l'id dans la table
                     * Donc on splite la chaine et on récupére le deuxième élément qui correspond à l'id de l'élément sélectionné.
                     * on rajoute cet id dans la collection des id des dates sélectionnées
                        
                    */
                    IdDatesSelectionnees.Add(System.Convert.ToInt16((UnControle.Name.Split('_'))[1]));
                }
            }
            UneConnexion.InscrireBenevole(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : null, TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text : null, TxtMail.Text != "" ? TxtMail.Text : null, System.Convert.ToDateTime(TxtDateNaissance.Text), NumeroLicence, IdDatesSelectionnees);
            Utilitaire.resetTextbox(GrpIdentite);
            Utilitaire.resetTextbox(GrpBenevole);
        }
        /// <summary>
        /// Cetet méthode teste les données saisies afin d'activer ou désactiver le bouton d'enregistrement d'un bénévole
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkDateBenevole_CheckedChanged(object sender, EventArgs e)
        {
            BtnEnregistreBenevole.Enabled = (TxtLicenceBenevole.Text == "" || TxtLicenceBenevole.MaskCompleted) && TxtDateNaissance.MaskCompleted && Utilitaire.CompteChecked(PanelDispoBenevole) > 0;
        }
       
        private void TxtMail_TextChanged(object sender, EventArgs e)
        {

        }

        private void CmbQualiteLicencie_SelectedIndexChanged(object sender, EventArgs e)
        {
            VerifBtnEnregistreLicencie();
        }

        private void TxtLicenceLicencie_KeyUp(object sender, KeyEventArgs e)
        {
            VerifBtnEnregistreLicencie();
        }
    }
}
