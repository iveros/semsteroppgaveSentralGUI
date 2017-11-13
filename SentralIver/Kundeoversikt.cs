using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SOKlasseLib;

namespace SentralIver
{
    public partial class Kundeoversikt : Form
    {
        
        public Kundeoversikt()
        {
            InitializeComponent();
            // Burde lage ei utskriftsliste i SO_Klasser?
        }

        public void skrivUtKundeoversikt(Sentral o, int KID)
        {
            richKundeliste.AppendText(o.SkrivUtKundeliste(KID-1)); // Midlertidig funksjon i SO_Klasser
            
        }

        private void btnLukk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


