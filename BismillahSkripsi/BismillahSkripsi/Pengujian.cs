using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Data.OleDb;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;

using Emgu.CV;
using Emgu.CV.Structure;

using AForge.Imaging;
using AForge.Imaging.Filters;


namespace BismillahSkripsi
{
    public partial class Pengujian : Form
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
        public Pengujian()
        {
            InitializeComponent();

            alpha = 0.05;
            resizeImg = new Bitmap(480, 360);
            resizeBlob = new Bitmap(20, 20);
            destRect = new Rectangle(0, 0, 480, 360);
            destRectBlob = new Rectangle(0, 0, 20, 20);
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

            toolStripStatusLabelDirectory.Text = "";
            toolStripStatusLabelSize.Text = "";
        }
        private void btnInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog bukaFile = new OpenFileDialog();
            if (bukaFile.ShowDialog() == DialogResult.OK)
            {
                pbInput.Image = pbChara.Image = pbGrey.Image = pbOtsu.Image = pbPlat.Image = null;
                bitmapOpen = new Bitmap(bukaFile.FileName);
                imgInput = resizing(bitmapOpen).ToImage<Bgr, byte>();
                pbInput.Image = imgInput.ToBitmap();
                toolStripStatusLabelDirectory.Text = Path.GetFullPath(bukaFile.FileName);
                toolStripStatusLabelSize.Text = "Lebar: " + pbInput.Image.Width + " px. Tinggi: " + pbInput.Image.Height + " px.";
            }
        }
        private void btnGrey_Click(object sender, EventArgs e)
        {
            imgGray = imgInput.Convert<Gray, byte>();
            Bitmap imgGreyBit = imgGray.ToBitmap();
            HistogramEqualization filter = new HistogramEqualization();
            filter.ApplyInPlace(imgGreyBit);
            pbGrey.Image = imgGreyBit;
            LogAction("Berhasil Menerapkan Greyscale");
        }

        private void btnOtsu_Click(object sender, EventArgs e)
        {
            imgOtsu = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
            CvInvoke.Threshold(imgGray, imgOtsu, 300, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            img_DilasiBin = imgOtsu.Convert<Gray, byte>().Dilate(1);
            img_Pembalik = switchColor(new Bitmap(img_DilasiBin.ToBitmap()));
            LogAction("Berhasil Menerapkan Otsu");
            BlobSquare();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            pbPlat.Image = colorSwitch(new Bitmap(img_Plat));
            arrayBlobs.Clear();
            txtBoxList.Clear();
            WaitCallback del = delegate
            {
                this.Invoke(new Action(() => BlobDetection((Bitmap)pbChara.Image)));
            };
            ThreadPool.QueueUserWorkItem(del);
        }

        private void btnKlasifikasi_Click(object sender, EventArgs e)
        {
            con.Open();
            double ED;
            List<string> cls = new List<string>();
            List<double> sort = new List<double>();

            // Ambil data bobot
            string sqli = "SELECT * FROM PlatW";
            OleDbCommand get1 = new OleDbCommand(sqli, con);
            OleDbDataReader rows = get1.ExecuteReader();

            List<string> arrayKelasBobot = new List<string>();
            double[][] arrayEkstraksiBobot = new double[1000][];

            int q = 0;
            while (rows.Read())
            {
                arrayKelasBobot.Add(rows[1].ToString());

                arrayEkstraksiBobot[q] = new double[rows[3].ToString().Length];
                arrayEkstraksiBobot[q] = rows[3].ToString().Split(' ').Select(Double.Parse).ToArray(); ;

                q++;
            }



            for (int i = 0; i < arrayBlobs.Count; i++)
            {
                string[] str = arrayBlobs[i].Select(x => new string(x, 1)).ToArray();

                for (int j = 0; j < arrayEkstraksiBobot.Length; j++)
                {
                    if (arrayEkstraksiBobot[j] == null) { break; }

                    double totalBobot1 = 0;

                    for (int k = 0; k < str.Length; k++)
                    {
                        totalBobot1 = totalBobot1 + Math.Pow((double.Parse(str[k]) - arrayEkstraksiBobot[j][k]), 2);
                    }
                    ED = Math.Sqrt(totalBobot1);
                    sort.Add(ED);
                }

                List<double> res = sort.OrderBy(o => o).ToList();

                txtHasil.AppendText(arrayKelasBobot.ElementAt(sort.IndexOf(res[0])));
                sort.Clear();
            }
        }

        private void BlobSquare()
        {
            Bitmap image = (Bitmap)img_Pembalik;
            image = AForge.Imaging.Image.Clone(image, PixelFormat.Format24bppRgb);
            Bitmap originalImage = (Bitmap)pbInput.Image;//resizing(bitmapOpen);
            originalImage = AForge.Imaging.Image.Clone(originalImage, PixelFormat.Format24bppRgb);
            BlobCounter bc = new BlobCounter();
            // set bentuk blob
            //bc.CoupledSizeFiltering = true;
            bc.FilterBlobs = true;
            bc.MinWidth = 250;
            bc.MinHeight = 50;
            bc.MaxWidth = 300;
            bc.MaxHeight = 200;

            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;

            // process binary image
            bc.ProcessImage(image);
            Rectangle[] blobs = bc.GetObjectsRectangles();

            // extract the biggest blob
            Graphics g = Graphics.FromImage(image);
            Pen highLighter = new Pen(Color.Blue, 5);
            if (blobs.Length > 0)
            {
                foreach (Rectangle blob in blobs)
                {
                    g.DrawRectangle(highLighter, blob.X, blob.Y, blob.Width, blob.Height);
                    cropRectangle = new Rectangle(blob.X, blob.Y, blob.Width, blob.Height);
                }

                pbOtsu.Image = image;

                bc.ProcessImage(image);

                //Blob[] belobes = bc.GetObjectsInformation();
                //bc.ExtractBlobsImage(image, belobes[0], false);
                //pbPlat.Image = belobes[0].Image.ToManagedImage(); 
                cropi = new Bitmap(cropRectangle.Width, cropRectangle.Height);
                graphics = Graphics.FromImage(cropi);
                //graphicsAsli = Graphics.FromImage(originalImage);
                //graphics.DrawImage(image, 0, 0, cropRectangle, GraphicsUnit.Pixel);
                graphics.DrawImage(originalImage, 0, 0, cropRectangle, GraphicsUnit.Pixel);
                img_Plat = cropi;
                //pbPlat.Image = new ExtractBiggestBlob().Apply(image);
                LogAction("Detected " + blobs.Length + " blobs present.");
            }
            else
            {
                LogAction("Detected " + blobs.Length + " blobs present.");
                //pbOtsu.Image = image;
            }
        }

        //membalik warna
        private Bitmap switchColor(Bitmap bitmap)
        {
            int r, g, b;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);

                    if (color.R < 127 && color.B < 127 && color.B < 127)
                    {
                        r = g = b = 255;
                    }
                    else
                    {
                        r = g = b = 0;
                    }

                    bitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return bitmap;
        }
        //membalik warna
        private Bitmap colorSwitch(Bitmap bitmap)
        {
            int r, g, b;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);

                    if (color.R < 127 && color.B < 127 && color.B < 127)
                    {
                        r = g = b = 0;
                    }
                    else
                    {
                        r = g = b = 255;
                    }

                    bitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return bitmap;
        }
        //Deteksi Huruf dan Angka
        private void BlobDetection(Bitmap image)
        {
            image = (Bitmap)pbPlat.Image;
            image = AForge.Imaging.Image.Clone(image, PixelFormat.Format24bppRgb);
            Bitmap originalImage = img_Plat;
            originalImage = AForge.Imaging.Image.Clone(originalImage, PixelFormat.Format24bppRgb);
            BlobCounter bc = new BlobCounter();
            // set bentuk blob
            bc.FilterBlobs = true;
            bc.MinWidth = int.Parse(textBox1.Text);
            bc.MinHeight = int.Parse(textBox2.Text);
            bc.MaxWidth = int.Parse(textBox3.Text);
            bc.MaxHeight = int.Parse(textBox4.Text);

            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;

            // process binary image
            bc.ProcessImage(image);
            Rectangle[] blobs = bc.GetObjectsRectangles();

            // extract the biggest blob
            Graphics g = Graphics.FromImage(originalImage);
            Pen highLighter = new Pen(Color.Red, 5);
            if (blobs.Length > 0)
            {
                int i = 0, x = 5;
                foreach (Rectangle blob in blobs)
                {
                    cropRectangle = new Rectangle(blob.X, blob.Y, blob.Width, blob.Height);
                    cropi = null;
                    cropi = new Bitmap(cropRectangle.Width, cropRectangle.Height);
                    graphics = Graphics.FromImage(cropi);
                    graphics.DrawImage(originalImage, 0, 0, cropRectangle, GraphicsUnit.Pixel);
                    //img_Plat = cropi;

                    cropi = new Bitmap(resizingBlob(cropi));

                    var picture = new PictureBox
                    {
                        Name = "pictureBoxs" + i,
                        Size = new Size(20, 20),
                        Location = new System.Drawing.Point(x, 180),
                        Image = cropi,
                    };
                    this.Controls.Add(picture);
                    arrayBlobs.Add(string.Join("", array1D(cropi)));
                    //Console.WriteLine(imgChain);

                    var txt = new TextBox
                    {
                        Name = "txtKelas" + i,
                        Size = new Size(20, 20),
                        Location = new System.Drawing.Point(x, 205),
                        Text = " ",
                    };
                    this.Controls.Add(txt);
                    txtBoxList.Add(txt);
                    
                    x += 25;
                    i++;
                    //Console.WriteLine(string.Join("", array1D(cropi)));

                    g.DrawRectangle(highLighter, blob.X, blob.Y, blob.Width, blob.Height);
                }


                LogAction("Detected " + blobs.Length + " blobs present.");
                //LogStatus(true);
                pbChara.Image = originalImage;

                //PictureBox[] picture = new PictureBox[arrayBlobs.Length];
                //int j = 0, x = 12;
                //foreach (var p in arrayBlobs)
                //{

                //}
            }
            else
            {
                LogAction("Detected " + blobs.Length + " blobs present.");
                //LogStatus(false);
                pbChara.Image = originalImage;
            }
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

        //private string ChainCode(Bitmap bit)
        //{
        //    str = "";

        //    Color c0, c1, c2, c3, c4, c5, c6, c7;
        //    int jr, r0, r1, r2, r3, r4, r5, r6, r7;

        //    for (int x = 0; x < bit.Width; x++)
        //    {
        //        if (x == 0 || (x > 1 && x % 2 == 0))
        //        {
        //            for (int y = bit.Height - 1; y >= 0; y--)
        //            {
        //                Color clr = bit.GetPixel(x, y);
        //                if (x < bit.Width - 1)
        //                {
        //                    c0 = bit.GetPixel(x + 1, y);
        //                    r0 = (c0.R + c0.G + c0.B) / 3;
        //                    if (r0 > 127) { r0 = 0; }
        //                    else { r0 = 1; }
        //                }
        //                else { r0 = 0; }

        //                if (x < bit.Width - 1 && y > 0)
        //                {
        //                    c1 = bit.GetPixel(x + 1, y - 1);
        //                    r1 = (c1.R + c1.G + c1.B) / 3;
        //                    if (r1 > 127) { r1 = 0; }
        //                    else { r1 = 1; }
        //                }
        //                else { r1 = 0; }

        //                if (y > 0)
        //                {
        //                    c2 = bit.GetPixel(x, y - 1);
        //                    r2 = (c2.R + c2.G + c2.B) / 3;
        //                    if (r2 > 127) { r2 = 0; }
        //                    else { r2 = 1; }
        //                }
        //                else { r2 = 0; }

        //                if (x > 0 && y > 0)
        //                {
        //                    c3 = bit.GetPixel(x - 1, y - 1);
        //                    r3 = (c3.R + c3.G + c3.B) / 3;
        //                    if (r3 > 127) { r3 = 0; }
        //                    else { r3 = 1; }
        //                }
        //                else { r3 = 0; }

        //                if (x > 0)
        //                {
        //                    c4 = bit.GetPixel(x - 1, y);
        //                    r4 = (c4.R + c4.G + c4.B) / 3;
        //                    if (r4 > 127) { r4 = 0; }
        //                    else { r4 = 1; }
        //                }
        //                else { r4 = 0; }

        //                if (x > 0 && y < bit.Height - 1)
        //                {
        //                    c5 = bit.GetPixel(x - 1, y + 1);
        //                    r5 = (c5.R + c5.G + c5.B) / 3;
        //                    if (r5 > 127) { r5 = 0; }
        //                    else { r5 = 1; }
        //                }
        //                else { r5 = 0; }

        //                if (y < bit.Height - 1)
        //                {
        //                    c6 = bit.GetPixel(x, y + 1);
        //                    r6 = (c6.R + c6.G + c6.B) / 3;
        //                    if (r6 > 127) { r6 = 0; }
        //                    else { r6 = 1; }
        //                }
        //                else { r6 = 0; }

        //                if (x < bit.Width - 1 && y < bit.Height - 1)
        //                {
        //                    c7 = bit.GetPixel(x + 1, y + 1);
        //                    r7 = (c7.R + c7.G + c7.B) / 3;
        //                    if (r7 > 127) { r7 = 0; }
        //                    else { r7 = 1; }
        //                }
        //                else { r7 = 0; }

        //                jr = r0 + r1 + r2 + r3 + r4 + r5 + r6 + r7;
        //                str = str + jr.ToString();
        //            }
        //        }
        //        else if (x == 1 || (x > 1 && x % 2 == 1))
        //        {
        //            for (int y = 0; y < bit.Height; y++)
        //            {
        //                Color clr = bit.GetPixel(x, y);
        //                if (x < bit.Width - 1)
        //                {
        //                    c0 = bit.GetPixel(x + 1, y);
        //                    r0 = (c0.R + c0.G + c0.B) / 3;
        //                    if (r0 > 127) { r0 = 0; }
        //                    else { r0 = 1; }
        //                }
        //                else { r0 = 0; }

        //                if (x < bit.Width - 1 && y > 0)
        //                {
        //                    c1 = bit.GetPixel(x + 1, y - 1);
        //                    r1 = (c1.R + c1.G + c1.B) / 3;
        //                    if (r1 > 127) { r1 = 0; }
        //                    else { r1 = 1; }
        //                }
        //                else { r1 = 0; }

        //                if (y > 0)
        //                {
        //                    c2 = bit.GetPixel(x, y - 1);
        //                    r2 = (c2.R + c2.G + c2.B) / 3;
        //                    if (r2 > 127) { r2 = 0; }
        //                    else { r2 = 1; }
        //                }
        //                else { r2 = 0; }

        //                if (x > 0 && y > 0)
        //                {
        //                    c3 = bit.GetPixel(x - 1, y - 1);
        //                    r3 = (c3.R + c3.G + c3.B) / 3;
        //                    if (r3 > 127) { r3 = 0; }
        //                    else { r3 = 1; }
        //                }
        //                else { r3 = 0; }

        //                if (x > 0)
        //                {
        //                    c4 = bit.GetPixel(x - 1, y);
        //                    r4 = (c4.R + c4.G + c4.B) / 3;
        //                    if (r4 > 127) { r4 = 0; }
        //                    else { r4 = 1; }
        //                }
        //                else { r4 = 0; }

        //                if (x > 0 && y < bit.Height - 1)
        //                {
        //                    c5 = bit.GetPixel(x - 1, y + 1);
        //                    r5 = (c5.R + c5.G + c5.B) / 3;
        //                    if (r5 > 127) { r5 = 0; }
        //                    else { r5 = 1; }
        //                }
        //                else { r5 = 0; }

        //                if (y < bit.Height - 1)
        //                {
        //                    c6 = bit.GetPixel(x, y + 1);
        //                    r6 = (c6.R + c6.G + c6.B) / 3;
        //                    if (r6 > 127) { r6 = 0; }
        //                    else { r6 = 1; }
        //                }
        //                else { r6 = 0; }

        //                if (x < bit.Width - 1 && y < bit.Height - 1)
        //                {
        //                    c7 = bit.GetPixel(x + 1, y + 1);
        //                    r7 = (c7.R + c7.G + c7.B) / 3;
        //                    if (r7 > 127) { r7 = 0; }
        //                    else { r7 = 1; }
        //                }
        //                else { r7 = 0; }

        //                jr = r0 + r1 + r2 + r3 + r4 + r5 + r6 + r7;
        //                str = str + jr.ToString();
        //            }
        //        }
        //    }

        //    return str;
        ////}

        private double[] elveki(double[] bobot1, double[] bobot2, double[] data, double alpha)
        {
            double totalBobot1 = 0;
            double totalBobot2 = 0;

            for (int i = 0; i < bobot1.Length; i++)
            {
                totalBobot1 = totalBobot1 + Math.Pow((data[i] - bobot1[i]), 2);
                totalBobot2 = totalBobot2 + Math.Pow((data[i] - bobot2[i]), 2);
            }

            double[] outputBobot = new double[bobot1.Length];

            if (Math.Min(totalBobot1, totalBobot2) == totalBobot1)
            {
                for (int i = 0; i < bobot1.Length; i++)
                {
                    outputBobot[i] = bobot1[i] + (alpha * (data[i] - bobot1[i]));
                    //Console.WriteLine(outputBobot[i] + " " + bobot1[i] + " " + alpha + " " + data[i] + " " + bobot1[i]);
                }
            }
            else
            {
                for (int i = 0; i < bobot1.Length; i++)
                { outputBobot[i] = bobot1[i] - (alpha * (data[i] - bobot1[i])); }
            }

            return outputBobot;
        }
        private void LogAction(string Log)
        {
            try
            {
                rTBlog.AppendText(DateTime.Now.ToString("MM/dd/yyyy - hh:mm:ss") + " " + Log);
                rTBlog.AppendText(Environment.NewLine);
                rTBlog.SelectionStart = rTBlog.Text.Length;
                rTBlog.ScrollToCaret();
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
