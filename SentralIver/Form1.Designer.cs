using System;

namespace SentralIver
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        internal void RegistrerKunde(string n, string a, int k, string p, string i)
        {
            throw new NotImplementedException();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnKundeoversikt = new System.Windows.Forms.Button();
            this.btnRegistrerNyKunde = new System.Windows.Forms.Button();
            this.btnSlettenkunde = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.btnAvslutt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 52);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(948, 408);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Alarmoversikt";
            // 
            // btnKundeoversikt
            // 
            this.btnKundeoversikt.Location = new System.Drawing.Point(41, 480);
            this.btnKundeoversikt.Name = "btnKundeoversikt";
            this.btnKundeoversikt.Size = new System.Drawing.Size(148, 33);
            this.btnKundeoversikt.TabIndex = 2;
            this.btnKundeoversikt.Text = "Kundeoversikt";
            this.btnKundeoversikt.UseVisualStyleBackColor = true;
            this.btnKundeoversikt.Click += new System.EventHandler(this.btnKundeoversikt_Click);
            // 
            // btnRegistrerNyKunde
            // 
            this.btnRegistrerNyKunde.Location = new System.Drawing.Point(41, 519);
            this.btnRegistrerNyKunde.Name = "btnRegistrerNyKunde";
            this.btnRegistrerNyKunde.Size = new System.Drawing.Size(148, 33);
            this.btnRegistrerNyKunde.TabIndex = 3;
            this.btnRegistrerNyKunde.Text = "Registrer ny kunde";
            this.btnRegistrerNyKunde.UseVisualStyleBackColor = true;
            this.btnRegistrerNyKunde.Click += new System.EventHandler(this.btnRegistrerNyKunde_Click);
            // 
            // btnSlettenkunde
            // 
            this.btnSlettenkunde.Location = new System.Drawing.Point(41, 558);
            this.btnSlettenkunde.Name = "btnSlettenkunde";
            this.btnSlettenkunde.Size = new System.Drawing.Size(148, 33);
            this.btnSlettenkunde.TabIndex = 4;
            this.btnSlettenkunde.Text = "Slett en kunde";
            this.btnSlettenkunde.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(241, 480);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(148, 33);
            this.button4.TabIndex = 5;
            this.button4.Text = "Forbruksforespørsel";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(472, 480);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(222, 33);
            this.button5.TabIndex = 6;
            this.button5.Text = "Liste alle kunder";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(472, 519);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(222, 33);
            this.button6.TabIndex = 7;
            this.button6.Text = "Liste forbruk alle kunder";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(472, 558);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(222, 33);
            this.button7.TabIndex = 8;
            this.button7.Text = "Liste alle alarmer siden forrige sjekk";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(714, 480);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(222, 33);
            this.button8.TabIndex = 9;
            this.button8.Text = "Liste forbruk for en kunde";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(714, 519);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(222, 33);
            this.button9.TabIndex = 10;
            this.button9.Text = "Liste alle alarmer for en kunde";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // btnAvslutt
            // 
            this.btnAvslutt.Location = new System.Drawing.Point(798, 621);
            this.btnAvslutt.Name = "btnAvslutt";
            this.btnAvslutt.Size = new System.Drawing.Size(138, 44);
            this.btnAvslutt.TabIndex = 11;
            this.btnAvslutt.Text = "Avslutt";
            this.btnAvslutt.UseVisualStyleBackColor = true;
            this.btnAvslutt.Click += new System.EventHandler(this.btnAvslutt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 677);
            this.Controls.Add(this.btnAvslutt);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.btnSlettenkunde);
            this.Controls.Add(this.btnRegistrerNyKunde);
            this.Controls.Add(this.btnKundeoversikt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnKundeoversikt;
        private System.Windows.Forms.Button btnRegistrerNyKunde;
        private System.Windows.Forms.Button btnSlettenkunde;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button btnAvslutt;
    }
}

