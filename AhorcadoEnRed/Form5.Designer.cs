
namespace AhorcadoEnRed
{
    partial class Form5
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
            this.dibujoAhorcado1 = new Dibujo.DibujoAhorcado();
            this.SuspendLayout();
            // 
            // dibujoAhorcado1
            // 
            this.dibujoAhorcado1.Errores = 3;
            this.dibujoAhorcado1.Location = new System.Drawing.Point(224, 127);
            this.dibujoAhorcado1.Name = "dibujoAhorcado1";
            this.dibujoAhorcado1.Size = new System.Drawing.Size(185, 147);
            this.dibujoAhorcado1.TabIndex = 0;
            this.dibujoAhorcado1.Text = "dibujoAhorcado1";
            this.dibujoAhorcado1.Ahorcado += new System.EventHandler(this.dibujoAhorcado1_Ahorcado);
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dibujoAhorcado1);
            this.Name = "Form5";
            this.Text = "Form5";
            this.ResumeLayout(false);

        }

        #endregion

        private Dibujo.DibujoAhorcado dibujoAhorcado1;
    }
}