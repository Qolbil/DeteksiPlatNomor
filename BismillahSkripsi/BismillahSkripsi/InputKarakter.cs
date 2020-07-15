using AForge.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BismillahSkripsi
{
    public partial class InputKarakter : Form
    {
        Image<Bgr, byte> imgInput;
        Image<Gray, byte> imgGray;
        Image<Gray, byte> imgOtsu;
        Image<Gray, byte> img_DilasiBin;
        private Bitmap img_Pembalik, img_Plat, cropi, resizeBlob, resizeImg, bitmapOpen, copy, newimg;
        private List<string> arrayBlobs;
        private System.Drawing.Point min, max;
        private Rectangle cropRectangle, destRect, destRectBlob;
        private Color c;
        private Graphics graphics, graphicsBlob;
        private ImageAttributes imageAttributes, imageAttributesBlob;
        private List<int> sequenceCodeList;
        private OleDbConnection con = new OleDbConnection();
        private List<TextBox> txtBoxList;
        private double[] bobot, data;
        private double alpha;
        private int r, g, b, w, h, size;
        public InputKarakter()
        {
            InitializeComponent();

            alpha = 0.05;

            resizeImg = new Bitmap(480, 360);
            resizeBlob = new Bitmap(20, 20);
            destRect = new Rectangle(0, 0, 480, 360);
            destRectBlob = new Rectangle(0, 0, 20, 20);

            string Path = Environment.CurrentDirectory;
            string[] appPath = Path.Split(new string[] { "bin" }, StringSplitOptions.None);
            AppDomain.CurrentDomain.SetData("DataDirectory", appPath[0]);
            con.ConnectionString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = E:\Kuliah\ScriptSong\Data\dataPlat.accdb; Persist Security Info = False; ";

            resizeImg.SetResolution(resizeImg.HorizontalResolution, resizeImg.VerticalResolution);

            arrayBlobs = new List<string>();
            sequenceCodeList = new List<int>(400);
            txtBoxList = new List<TextBox>(9);
            graphics = Graphics.FromImage(resizeImg);

            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog bukaFile = new OpenFileDialog();
            if (bukaFile.ShowDialog() == DialogResult.OK)
            {
                pbInput.Image = pbGrey.Image = pbOtsu.Image = null;
                bitmapOpen = new Bitmap(bukaFile.FileName);
                imgInput = resizing(bitmapOpen).ToImage<Bgr, byte>();
                pbInput.Image = imgInput.ToBitmap();
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            imgGray = imgInput.Convert<Gray, byte>();
            //histogram Equalization
            //Mat histeq = new Mat();
            //CvInvoke.CLAHE(imgGray, 50, new Size(8,8), histeq);
            Bitmap imgGreyBit = imgGray.ToBitmap();

            HistogramEqualization filter = new HistogramEqualization();
            filter.ApplyInPlace(imgGreyBit);
            pbGrey.Image = imgGreyBit;/*<Bgr, byte>().ToBitmap<Bgr, byte>();*/
        }

        private void btnOtsu_Click(object sender, EventArgs e)
        {
            imgOtsu = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
            //Otsu Threshold var: 480. 255
            CvInvoke.Threshold(imgGray, imgOtsu, 300, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            //dilasi
            img_DilasiBin = imgOtsu.Convert<Gray, byte>().Dilate(1);
            cropi = resizingBlob(img_DilasiBin.AsBitmap());
            //imgChain = ChainCode(cropi);
            //Console.WriteLine(imgChain);
            pbOtsu.Image = cropi;
            arrayBlobs.Clear();
            txtBoxList.Clear();
            arrayBlobs.Add(string.Join("", array1D(cropi)));
            txtBoxList.Add(txtDataset);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < txtBoxList.Count; i++)
            {
                try
                {
                    con.Open();
                    OleDbCommand insert = new OleDbCommand();
                    insert.Connection = con;
                    insert.CommandText = "INSERT INTO platPelatihan (Kelas, array1D) values ('" + txtBoxList[i].Text + "','" + arrayBlobs[i] + "' ) ";
                    int a = insert.ExecuteNonQuery();
                    con.Close();
                    if (a == 0) { MessageBox.Show("Gagal Menyimpan Data");}
                    //Not updated.
                    else { MessageBox.Show("Berhasil Menyimpan Data"); }
                }
                //Updated.
                catch (Exception ex)
                {
                    // Not updated
                }
            }
        }

        private int[] array1D(Bitmap bitmap)
        {
            sequenceCodeList.Clear();

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    if ((bitmap.GetPixel(i, j).R + bitmap.GetPixel(i, j).G + bitmap.GetPixel(i, j).B) / 3 < 128)
                    {
                        sequenceCodeList.Add(0);
                    }
                    else
                    {
                        sequenceCodeList.Add(1);
                    }
                }
            }

            return sequenceCodeList.ToArray();
        }

        private Bitmap resizing(Bitmap bitmap)
        {
            graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);

            return resizeImg;
        }

        private Bitmap resizingBlob(Bitmap bitmap)
        {
            resizeBlob.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using (graphicsBlob = Graphics.FromImage(resizeBlob))
            {
                graphicsBlob.CompositingMode = CompositingMode.SourceCopy;
                graphicsBlob.CompositingQuality = CompositingQuality.HighQuality;
                graphicsBlob.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsBlob.SmoothingMode = SmoothingMode.HighQuality;
                graphicsBlob.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (imageAttributesBlob = new ImageAttributes())
                {
                    imageAttributesBlob.SetWrapMode(WrapMode.TileFlipXY);
                    graphicsBlob.DrawImage(bitmap, destRectBlob, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributesBlob);
                }
            }

            return resizeBlob;
        }

    }
}
