using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaisonDesLigues
{
    class Cheque
    {
        private string numeroCheque;
        private float montantCheque;
        private string typePaiement;

        public Cheque(string pNumeroCheque, float pMontantCheque, string pTypePaiement)
        {
            numeroCheque = pNumeroCheque;
            montantCheque = pMontantCheque;
            typePaiement = pTypePaiement;
        }

        public string getNumeroCheque()
        {
            return numeroCheque;
        }

        public void setNumeroCheque(string nouvNumeroCheque)
        {
            numeroCheque = nouvNumeroCheque;
        }

        public float getMontantCheque()
        {
            return montantCheque;
        }

        public void setMontantCheque(float nouvMontantCheque)
        {
            montantCheque = nouvMontantCheque;
        }

        public string getTypePaiement()
        {
            return typePaiement;
        }

        public void setTypePaiement(string nouvTypePaiement)
        {
            typePaiement = nouvTypePaiement;
        }

    }
}
