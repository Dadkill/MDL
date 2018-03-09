using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaisonDesLigues;

namespace MaisonDesLigues
{
    public class ComboBoxItem
    {
        string NomComplet;
        string nom;
        string prenom;
        string id;
        string email;

        public ComboBoxItem(string nomc, string mid,string mail,string mnon,string mprenom)
        {
            NomComplet = nomc;
            id = mid;
            email = mail;
            nom = mnon;
            prenom = mprenom;
        }

        public string getId()
        {
          return id;
        }
        public string getmail()
        {
            return email;
        }
        public string getnom()
        {
            return nom;
        }
        public string getprenom()
        {
            return prenom;
        }
        public override string ToString()
        {
            return NomComplet;
        }
    }
}
