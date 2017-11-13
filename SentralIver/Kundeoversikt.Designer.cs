namespace SentralIver
{
    partial class Kundeoversikt
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.richKundeliste = new System.Windows.Forms.RichTextBox();
            this.btnLukk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richKundeliste
            // 
            this.richKundeliste.Location = new System.Drawing.Point(13, 13);
            this.richKundeliste.Name = "richKundeliste";
            this.richKundeliste.Size = new System.Drawing.Size(922, 467);
            this.richKundeliste.TabIndex = 0;
            this.richKundeliste.Text = "";
            // 
            // btnLukk
            // 
            this.btnLukk.Location = new System.Drawing.Point(793, 504);
            this.btnLukk.Name = "btnLukk";
            this.btnLukk.Size = new System.Drawing.Size(142, 32);
            this.btnLukk.TabIndex = 1;
            this.btnLukk.Text = "Lukk";
            this.btnLukk.UseVisualStyleBackColor = true;
            this.btnLukk.Click += new System.EventHandler(this.btnLukk_Click);
            // 
            // Kundeoversikt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 548);
            this.Controls.Add(this.btnLukk);
            this.Controls.Add(this.richKundeliste);
            this.Name = "Kundeoversikt";
            this.Text = "Kundeoversikt";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richKundeliste;
        private System.Windows.Forms.Button btnLukk;
    }
}