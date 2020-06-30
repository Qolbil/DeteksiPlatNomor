using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;


using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Data.OleDb;

namespace ProjectPlat_Otw
{
    public partial class Form2 : Form
    {
        Image<Bgr, byte> imgInput;
        Image<Gray, byte> imgGray;
        Image<Gray, byte> imgOtsu;
        Image<Gray, byte> img_DilasiBin;
        private Bitmap img_Pembalik, img_Plat, cropi, resizeBlob, resizeImg, bitmapOpen;
        private List<string> arrayBlobs;
        private System.Drawing.Point min, max;
        private Rectangle cropRectangle, destRect, destRectBlob;
        private Color c;
        private Graphics graphics, graphicsBlob;
        private System.Drawing.Image Imageee;
        private ImageAttributes imageAttributes, imageAttributesBlob;
        private List<int> sequenceCodeList;
        private OleDbConnection con = new OleDbConnection();
        private List<TextBox> txtBoxList;

        public Form2()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
