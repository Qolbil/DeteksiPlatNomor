namespace BismillahSkripsi
{
    partial class Pengujian
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
            this.btnKlasifikasi = new System.Windows.Forms.Button();
            this.btn = new System.Windows.Forms.Button();
            this.btnOtsu = new System.Windows.Forms.Button();
            this.btnGrey = new System.Windows.Forms.Button();
            this.btnInput = new System.Windows.Forms.Button();
            this.rTBlog = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.pbChara = new System.Windows.Forms.PictureBox();
            this.pbPlat = new System.Windows.Forms.PictureBox();
            this.pbOtsu = new System.Windows.Forms.PictureBox();
            this.pbGrey = new System.Windows.Forms.PictureBox();
            this.pbInput = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDirectory = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtHasil = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChara)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOtsu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInput)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnKlasifikasi
            // 
            this.btnKlasifikasi.Location = new System.Drawing.Point(330, 378);
            this.btnKlasifikasi.Name = "btnKlasifikasi";
            this.btnKlasifikasi.Size = new System.Drawing.Size(75, 23);
            this.btnKlasifikasi.TabIndex = 50;
            this.btnKlasifikasi.Text = "Klasifikasi";
            this.btnKlasifikasi.UseVisualStyleBackColor = true;
            this.btnKlasifikasi.Click += new System.EventHandler(this.btnKlasifikasi_Click);
            // 
            // btn
            // 
            this.btn.Location = new System.Drawing.Point(249, 377);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(75, 23);
            this.btn.TabIndex = 49;
            this.btn.Text = "Deteksi Blob";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnOtsu
            // 
            this.btnOtsu.Location = new System.Drawing.Point(249, 348);
            this.btnOtsu.Name = "btnOtsu";
            this.btnOtsu.Size = new System.Drawing.Size(75, 23);
            this.btnOtsu.TabIndex = 48;
            this.btnOtsu.Text = "Otsu";
            this.btnOtsu.UseVisualStyleBackColor = true;
            this.btnOtsu.Click += new System.EventHandler(this.btnOtsu_Click);
            // 
            // btnGrey
            // 
            this.btnGrey.Location = new System.Drawing.Point(249, 319);
            this.btnGrey.Name = "btnGrey";
            this.btnGrey.Size = new System.Drawing.Size(75, 23);
            this.btnGrey.TabIndex = 47;
            this.btnGrey.Text = "Greyscale";
            this.btnGrey.UseVisualStyleBackColor = true;
            this.btnGrey.Click += new System.EventHandler(this.btnGrey_Click);
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(249, 290);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(75, 23);
            this.btnInput.TabIndex = 46;
            this.btnInput.Text = "Input";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // rTBlog
            // 
            this.rTBlog.BackColor = System.Drawing.SystemColors.Info;
            this.rTBlog.Location = new System.Drawing.Point(527, 291);
            this.rTBlog.Name = "rTBlog";
            this.rTBlog.Size = new System.Drawing.Size(261, 121);
            this.rTBlog.TabIndex = 45;
            this.rTBlog.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(14, 275);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 137);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ParameterBlob";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(129, 105);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 20);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = "70";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(129, 79);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = "80";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(129, 53);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "40";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(129, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "10";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Blob Terbesar Height:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Blob Terbesar Width:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Blob Terkecil Height:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Blob Terkecil Width:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(662, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Deteksi Karakter";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(529, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Plat";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(376, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 41;
            this.label3.Text = "Otsu";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 40;
            this.label2.Text = "Grayscale";
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(60, 18);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(55, 13);
            this.label.TabIndex = 39;
            this.label.Text = "Citra Input";
            // 
            // pbChara
            // 
            this.pbChara.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbChara.Location = new System.Drawing.Point(637, 34);
            this.pbChara.Name = "pbChara";
            this.pbChara.Size = new System.Drawing.Size(134, 121);
            this.pbChara.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbChara.TabIndex = 38;
            this.pbChara.TabStop = false;
            // 
            // pbPlat
            // 
            this.pbPlat.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbPlat.Location = new System.Drawing.Point(480, 34);
            this.pbPlat.Name = "pbPlat";
            this.pbPlat.Size = new System.Drawing.Size(134, 121);
            this.pbPlat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPlat.TabIndex = 37;
            this.pbPlat.TabStop = false;
            // 
            // pbOtsu
            // 
            this.pbOtsu.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbOtsu.Location = new System.Drawing.Point(326, 34);
            this.pbOtsu.Name = "pbOtsu";
            this.pbOtsu.Size = new System.Drawing.Size(134, 121);
            this.pbOtsu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbOtsu.TabIndex = 36;
            this.pbOtsu.TabStop = false;
            // 
            // pbGrey
            // 
            this.pbGrey.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbGrey.Location = new System.Drawing.Point(169, 34);
            this.pbGrey.Name = "pbGrey";
            this.pbGrey.Size = new System.Drawing.Size(134, 121);
            this.pbGrey.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbGrey.TabIndex = 35;
            this.pbGrey.TabStop = false;
            // 
            // pbInput
            // 
            this.pbInput.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pbInput.Location = new System.Drawing.Point(14, 34);
            this.pbInput.Name = "pbInput";
            this.pbInput.Size = new System.Drawing.Size(134, 121);
            this.pbInput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbInput.TabIndex = 34;
            this.pbInput.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDirectory,
            this.toolStripStatusLabelSize});
            this.statusStrip1.Location = new System.Drawing.Point(0, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 24);
            this.statusStrip1.TabIndex = 52;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelDirectory
            // 
            this.toolStripStatusLabelDirectory.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabelDirectory.Name = "toolStripStatusLabelDirectory";
            this.toolStripStatusLabelDirectory.Size = new System.Drawing.Size(59, 19);
            this.toolStripStatusLabelDirectory.Text = "Directory";
            // 
            // toolStripStatusLabelSize
            // 
            this.toolStripStatusLabelSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabelSize.Name = "toolStripStatusLabelSize";
            this.toolStripStatusLabelSize.Size = new System.Drawing.Size(31, 19);
            this.toolStripStatusLabelSize.Text = "Size";
            // 
            // txtHasil
            // 
            this.txtHasil.Location = new System.Drawing.Point(411, 381);
            this.txtHasil.Name = "txtHasil";
            this.txtHasil.Size = new System.Drawing.Size(110, 20);
            this.txtHasil.TabIndex = 53;
            // 
            // Pengujian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtHasil);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnKlasifikasi);
            this.Controls.Add(this.btn);
            this.Controls.Add(this.btnOtsu);
            this.Controls.Add(this.btnGrey);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.rTBlog);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.pbChara);
            this.Controls.Add(this.pbPlat);
            this.Controls.Add(this.pbOtsu);
            this.Controls.Add(this.pbGrey);
            this.Controls.Add(this.pbInput);
            this.Name = "Pengujian";
            this.Text = "Pengujian";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbChara)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOtsu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbGrey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInput)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnKlasifikasi;
        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.Button btnOtsu;
        private System.Windows.Forms.Button btnGrey;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.RichTextBox rTBlog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.PictureBox pbChara;
        private System.Windows.Forms.PictureBox pbPlat;
        private System.Windows.Forms.PictureBox pbOtsu;
        private System.Windows.Forms.PictureBox pbGrey;
        private System.Windows.Forms.PictureBox pbInput;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDirectory;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSize;
        private System.Windows.Forms.TextBox txtHasil;
    }
}