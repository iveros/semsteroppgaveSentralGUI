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
    public partial class Form1 : Form
    {
        int kid;
        Sentral nySentral = new Sentral();
        public Form1()
        {
            InitializeComponent();
        }

       

        public void leggTilKunde(string n, string a, int k, string p, string i)
        {
            nySentral.RegistrerKunde(n, a, k, p, i);
            this.kid = k;
        }
        
        
        private void btnAvslutt_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnRegistrerNyKunde_Click(object sender, EventArgs e)
        {
            RegistrerNyKunde formRegistrerNyKunde = new RegistrerNyKunde(this);
            formRegistrerNyKunde.Show();
            
        }

        private void btnKundeoversikt_Click(object sender, EventArgs e)
        {
            Kundeoversikt formKundeoversikt = new Kundeoversikt();
            formKundeoversikt.skrivUtKundeoversikt(nySentral, kid);
            formKundeoversikt.Show();
        }








       
    }
}
