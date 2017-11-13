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
    public partial class RegistrerNyKunde : Form
    {
        static int Kid = 1;
        Form1 sentral;

        
        //Sentral nySentral = new Sentral();

        public RegistrerNyKunde(Form1 o)
        {
            InitializeComponent();
            txtKid.Text = (Kid).ToString();
            this.sentral = o;
        }

        private void btn_Lagre_Click(object sender, EventArgs e)
        {
            try
            {
                sentral.leggTilKunde(txtNavn.Text, txtAdresse.Text, Kid, txtPassord.Text, txtIP.Text);
                ++Kid;

                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Avbryt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
