namespace BismillahSkripsi
{
    partial class InputKarakter
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
            this.pbInput = new System.Windows.Forms.PictureBox();
            this.pbGrey = new System.Windows.Forms.PictureBox();
            this.pbOtsu = new System.Windows.Forms.PictureBox();
            this.btnInput = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtDataset = new System.Windows.Forms.TextBox();
            this.btnOtsu = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOtsu)).BeginInit();
            this.SuspendLayout();
            // 
            // pbInput
            // 
            this.pbInput.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pbInput.Location = new System.Drawing.Point(12, 12);
            this.pbInput.Name = "pbInput";
            this.pbInput.Size = new System.Drawing.Size(114, 104);
            this.pbInput.TabIndex = 0;
            this.pbInput.TabStop = false;
            // 
            // pbGrey
            // 
            this.pbGrey.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pbGrey.Location = new System.Drawing.Point(150, 12);
            this.pbGrey.Name = "pbGrey";
            this.pbGrey.Size = new System.Drawing.Size(114, 104);
            this.pbGrey.TabIndex = 1;
            this.pbGrey.TabStop = false;
            // 
            // pbOtsu
            // 
            this.pbOtsu.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pbOtsu.Location = new System.Drawing.Point(288, 12);
            this.pbOtsu.Name = "pbOtsu";
            this.pbOtsu.Size = new System.Drawing.Size(114, 104);
            this.pbOtsu.TabIndex = 2;
            this.pbOtsu.TabStop = false;
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(12, 135);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(75, 23);
            this.btnInput.TabIndex = 3;
            this.btnInput.Text = "Input";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(93, 135);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 4;
            this.btnProcess.Text = "Proses";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(255, 135);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Simpan";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtDataset
            // 
            this.txtDataset.Location = new System.Drawing.Point(334, 135);
            this.txtDataset.Name = "txtDataset";
            this.txtDataset.Size = new System.Drawing.Size(25, 20);
            this.txtDataset.TabIndex = 6;
            // 
            // btnOtsu
            // 
            this.btnOtsu.Location = new System.Drawing.Point(174, 135);
            this.btnOtsu.Name = "btnOtsu";
            this.btnOtsu.Size = new System.Drawing.Size(75, 23);
            this.btnOtsu.TabIndex = 7;
            this.btnOtsu.Text = "Otsu";
            this.btnOtsu.UseVisualStyleBackColor = true;
            this.btnOtsu.Click += new System.EventHandler(this.btnOtsu_Click);
            // 
            // InputKarakter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(428, 204);
            this.Controls.Add(this.btnOtsu);
            this.Controls.Add(this.txtDataset);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.pbOtsu);
            this.Controls.Add(this.pbGrey);
            this.Controls.Add(this.pbInput);
            this.Name = "InputKarakter";
            this.Text = "Input Karakter";
            ((System.ComponentModel.ISupportInitialize)(this.pbInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOtsu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbInput;
        private System.Windows.Forms.PictureBox pbGrey;
        private System.Windows.Forms.PictureBox pbOtsu;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtDataset;
        private System.Windows.Forms.Button btnOtsu;
    }
}