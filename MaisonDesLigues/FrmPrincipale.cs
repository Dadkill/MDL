﻿using System;
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
            foreach (GroupBox ungroupbox in tabPage1.Controls) //Miss1 Je rattache tous les boutons a un evenement
            {  
                foreach (Control unCtrl in ungroupbox.Controls)
                {
                    if (unCtrl is RadioButton)
                    {
                        RadioButton unbtnradio = (RadioButton)unCtrl;
                        if (unbtnradio.Text != "Magie")
                            unbtnradio.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
                    }
                    foreach(Control leCont in unCtrl.Controls)
                    {
                        if (leCont is RadioButton)
                        {
                            RadioButton unbtnradio2 = (RadioButton)leCont;
                            if (unbtnradio2.Text != "Magie")
                                unbtnradio2.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
                        }
                    }
                }
            }
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e) //Action a chaque fois qu'un radiobutton est coché
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Checked == true)
            {
                if (radioButton.Name == "radioButton1")
                {
                    Organiser(GrpAtelier);
                    ChangerTailleForm(606, 635);
                }
                else if (radioButton.Name == "radioButton2")
                {
                    Utilitaire.RemplirComboBox(UneConnexion, comboBox2, "VATELIER01");
                    Organiser(GrpTheme);
                    ChangerTailleForm(606, 410);
                    ChangerPositionGrpBox(32, 136, GrpTheme);
                }
               else if (radioButton.Name == "radioButton3")
                {
                    Utilitaire.RemplirComboBox(UneConnexion, comboBox3, "VATELIER01");
                    Organiser(GrpVacation);
                    ChangerTailleForm(606, 425);
                    ChangerPositionGrpBox(32, 136, GrpVacation);
                }
               else if (radioButton.Name == "radioButton4")
                {
                    MagieOptionAjouter.Checked = true;
                    Organiser(GrpAjouter);
                    ChangerTailleForm(606, 322);
                }
               else if (radioButton.Name == "radioButton5")
                {
                    Utilitaire.RemplirComboBox(UneConnexion, comboBox4, "VATELIER01");
                    Organiser(GrpModifier);
                    ChangerTailleForm(606, 370);
                    ChangerPositionGrpBox(15, 121, GrpModifier);
                }
            }

        }

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
                    //this.GererInscriptionLicencie();
                    break;
                case "RadIntervenant":
                    this.GererInscriptionIntervenant();
                    break;
                default:
                    throw new Exception("Erreur interne à l'application");
            }
        }

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie du complément d'inscription d'un intervenant.
        /// </summary>
       
        private void GererInscriptionIntervenant()
        {

            GrpBenevole.Visible = false;
            GrpIntervenant.Visible = true;
            PanFonctionIntervenant.Visible = true;
            GrpIntervenant.Left = 23;
            GrpIntervenant.Top = 264;
            Utilitaire.CreerDesControles(this, UneConnexion, "STATUT", "Rad_", PanFonctionIntervenant, "RadioButton", this.rdbStatutIntervenant_StateChanged);
            Utilitaire.RemplirComboBox(UneConnexion, CmbAtelierIntervenant, "VATELIER01");

            CmbAtelierIntervenant.Text = "Choisir";

        }

        /// <summary>     
        /// procédure permettant d'afficher l'interface de saisie des disponibilités des bénévoles.
        /// </summary>
        private void GererInscriptionBenevole()
        {

            GrpBenevole.Visible = true;
            GrpBenevole.Left = 23;
            GrpBenevole.Top = 264;
            GrpIntervenant.Visible = false;

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
        private void ChangerTailleForm(int x,int y) //Miss1 Methode pour changer la taille de la form avec x en abscisse et y en ordonnée
        {
           
            this.Size = new System.Drawing.Size(x, y);
            this.CenterToScreen();
        }
        private void ChangerPositionGrpBox(int x, int y,GroupBox magrpbox)  //Miss1 Methode pour changer la position de la grpbox passé en paramétre a x en abscisse et y en ordonnée pixel.
        {
            magrpbox.Location = new System.Drawing.Point(x, y);
        }
        /// <summary>
        /// permet d'appeler la méthode VerifBtnEnregistreIntervenant qui déterminera le statu du bouton BtnEnregistrerIntervenant
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
        /// Permet d'intercepter le click sur le bouton d'enregistrement d'un bénévole.
        /// Cette méthode va appeler la méthode InscrireBenevole de la Bdd, après avoir mis en forme certains paramètres à envoyer.
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
                    }
                }
                else
                { // inscription sans les nuitées
                      UneConnexion.InscrireIntervenant(TxtNom.Text, TxtPrenom.Text, TxtAdr1.Text, TxtAdr2.Text != "" ? TxtAdr2.Text : "", TxtCp.Text, TxtVille.Text, txtTel.MaskCompleted ? txtTel.Text :"", TxtMail.Text != "" ? TxtMail.Text : "", System.Convert.ToInt16(CmbAtelierIntervenant.SelectedValue), this.IdStatutSelectionne);
                      MessageBox.Show("Inscription intervenant sans nuitée effectuée");
                    
                }
                Utilitaire.resetTextbox(GrpIdentite);
                Utilitaire.resetTextbox(GrpIntervenant);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        /// <summary>
        /// Méthode privée testant le contrôle combo et la variable IdStatutSelectionne qui contient une valeur
        /// Cette méthode permetra ensuite de définir l'état du bouton BtnEnregistrerIntervenant
        /// </summary>
        /// <returns></returns>
        private Boolean VerifBtnEnregistreIntervenant()
        {
            return CmbAtelierIntervenant.Text !="Choisir" && this.IdStatutSelectionne.Length > 0;
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

        private void TxtMail_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtCp_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(TxtCp.Text, "[^0-9]"))
            {
                MessageBox.Show("Veuillez entrer uniquement des nombres pour le code postal.");
                TxtCp.Text = TxtCp.Text.Remove(TxtCp.Text.Length - 1);
                TxtCp.SelectionStart = TxtCp.Text.Length; // add some logic if length is 0
                TxtCp.SelectionLength = 0;
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)//Miss1
        {

            ChangerTailleForm(606, 180);
            MagieOption.Checked = true;
            MagieOptionAjouter.Checked = true;
        }

        private void TabInscription_Enter(object sender, EventArgs e)//Miss 1
        {
            ChangerTailleForm(666,632);
        }

        private void Organiser(GroupBox magroupbox) //Miss1 Cache tous les GroupBox autre que celle passé en parametre d'un meme parent
        {
            magroupbox.Show();

                foreach (GroupBox unegrb in magroupbox.Parent.Controls)
                {
                    if (unegrb.Text != magroupbox.Text && unegrb.Text !="Options")
                    {
                        unegrb.Hide();
                    }
                }          
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            UneConnexion.ModifierVacation(int.Parse(comboBox4.SelectedValue.ToString()), int.Parse(comboBox5.SelectedValue.ToString()),DateTime.Parse(textBox9.Text), DateTime.Parse(textBox8.Text));
        }

        private void BtnAjouterAtelier_Click(object sender, EventArgs e)
        {
            
        }
    }
}
