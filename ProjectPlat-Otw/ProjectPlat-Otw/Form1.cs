using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

using Emgu.CV;
using Emgu.CV.Structure;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace ProjectPlat_Otw
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> imgInput;
        Image<Gray, byte> imgGray;
        Image<Gray, byte> imgOtsu;
        Image<Gray, byte> img_DilasiBin;
        Image<Gray, byte> img_histogramEqualization;
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
        private double[] bobot, data;
        private int temTarget, statusTarget;
        private double alpha;

        public Form1()
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
            //con.ConnectionString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = |DataDirectory|\dataPlat.accdb; Persist Security Info = False; ";
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
            toolStripProgressBar1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog bukaFile = new OpenFileDialog();
            if (bukaFile.ShowDialog() == DialogResult.OK)
            {
                //bitmapOpen = null; imgInput = null; imgGray = null;
                pbInput.InitialImage = null ;
                pbInput.Image = pbChara.Image = pbGrey.Image = pbOtsu.Image = pbPlat.Image = null;
                pbInput.Image = Imageee;
                bitmapOpen = new Bitmap(bukaFile.FileName);
                imgInput = resizing(bitmapOpen).ToImage<Bgr, byte>();
                pbInput.Image = imgInput.ToBitmap();
                toolStripStatusLabelDirectory.Text = Path.GetFullPath(bukaFile.FileName);
                toolStripStatusLabelSize.Text = "Lebar: " + pbInput.Image.Width + " px. Tinggi: " + pbInput.Image.Height + " px.";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imgGray = imgInput.Convert<Gray, byte>();
            //histogram Equalization
            //Mat histeq = new Mat();
            //CvInvoke.CLAHE(imgGray, 50, new Size(8,8), histeq);
            Bitmap imgGreyBit = imgGray.ToBitmap();

            HistogramEqualization filter = new HistogramEqualization();
            filter.ApplyInPlace(imgGreyBit);
            pbGrey.Image = imgGreyBit;/*<Bgr, byte>().ToBitmap<Bgr, byte>();*/
            LogAction("Berhasil Menerapkan Greyscale");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            imgOtsu = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
            //Otsu Threshold var: 480. 255
            CvInvoke.Threshold (imgGray, imgOtsu, 300, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            //dilasi
            img_DilasiBin = imgOtsu.Convert<Gray, byte>().Dilate(1);
            //Histogram Equalization
            //pbOtsu.Image = img_DilasiBin.ToBitmap();
            //pictOtsu.Image = imgOtsu.ToBitmap();
            //pictOtsu.SizeMode = PictureBoxSizeMode.Zoom;
            img_Pembalik = switchColor(new Bitmap(img_DilasiBin.ToBitmap()));
            LogAction("Berhasil Menerapkan Otsu");
            BlobSquare();
            
        }
        
        private void btnProses_Click(object sender, EventArgs e)
        {
            
            pbPlat.Image = colorSwitch(new Bitmap(img_Plat));

            arrayBlobs.Clear();
            txtBoxList.Clear();
            WaitCallback del = delegate
            {
                this.Invoke(new Action(() => BlobDetection((Bitmap)pbChara.Image)));
            };
            ThreadPool.QueueUserWorkItem(del);

            pbChara.SizeMode = PictureBoxSizeMode.Zoom;
            
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void otsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imgOtsu = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
            //Otsu Threshold var: 480. 255
            CvInvoke.Threshold(imgGray, imgOtsu, 300, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            //dilasi
            img_DilasiBin = imgOtsu.Convert<Gray, byte>().Dilate(1);
            cropi = resizingBlob(img_DilasiBin.AsBitmap()); 
            pbOtsu.Image = cropi;
            arrayBlobs.Clear();
            txtBoxList.Clear();
            arrayBlobs.Add(string.Join("", array1D(cropi)));
            txtBoxList.Add(txtDataset);

            //pictOtsu.Image = imgOtsu.ToBitmap();
            //pictOtsu.SizeMode = PictureBoxSizeMode.Zoom;

        }

        private void sAveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < txtBoxList.Count; i++)
            {
                try
                {
                    con.Open();
                    OleDbCommand insert = new OleDbCommand();
                    insert.Connection = con;
                    insert.CommandText = "INSERT INTO PlatPelatihan (Kelas, array1D) values ('" + txtBoxList[i].Text + "','" + arrayBlobs[i] + "' ) ";
                    int a = insert.ExecuteNonQuery();
                    con.Close();
                    if (a == 0) { LogAction("Gagal Menyimpan Data"); }
                    //Not updated.
                    else { LogAction("Berhasil Menyimpan Data"); }
                }
                //Updated.
                catch (Exception ex)
                {
                    // Not updated
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveTraining_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < txtBoxList.Count; i++)
            {
                try
                {
                    con.Open();
                    OleDbCommand insert = new OleDbCommand();
                    insert.Connection = con;
                    insert.CommandText = "INSERT INTO PlatPelatihan (Kelas, array1D) values ('" + txtBoxList[i].Text + "','" + arrayBlobs[i] + "' ) ";
                    int a = insert.ExecuteNonQuery();
                    con.Close();
                    if (a == 0) { LogAction("Gagal Menyimpan Data"); }
                    //Not updated.
                    else { LogAction("Berhasil Menyimpan Data"); }
                }
                //Updated.
                catch (Exception ex)
                {
                    // Not updated
                } 
            }
        }

        private void btnTraining_Click(object sender, EventArgs e)
        {
            con.Open();

            // Ambil data training
            string sql = "SELECT * FROM PlatPelatihan";
            OleDbCommand get = new OleDbCommand(sql, con);
            OleDbDataReader row = get.ExecuteReader();

            List<string> arrayKelas = new List<string>();
            int[][] arrayEkstraksi = new int[1000][];

            // Memasukkan data ke dalam array
            int i = 0;
            while (row.Read())
            {
                arrayKelas.Add(row[1].ToString());

                for (int j = 0; j < row[2].ToString().Length; j++)
                {
                    arrayEkstraksi[i] = new int[row[2].ToString().Length];
                    arrayEkstraksi[i][j] = int.Parse(row[2].ToString().Substring(j, 1));
                    //Console.WriteLine("Sub string: " + arrayEkstraksi[i][j].ToString());
                }
                i++;
            }

            // Ambil data bobot
            string sqli = "SELECT * FROM PlatW";
            OleDbCommand get1 = new OleDbCommand(sqli, con);
            OleDbDataReader rows = get1.ExecuteReader();

            List<string> arrayKelasBobot = new List<string>();
            double[][] arrayEkstraksiBobot = new double[1000][];

            // Memasukkan data bobot ke dalam array bobot
            int q = 0;
            while (rows.Read())
            {
                arrayKelasBobot.Add(rows[1].ToString());
                for (int j = 0; j < rows[2].ToString().Length; j++)
                {
                    arrayEkstraksiBobot[q] = new double[rows[2].ToString().Length];
                    arrayEkstraksiBobot[q][j] = double.Parse(rows[2].ToString().Substring(j, 1));
                    //Console.WriteLine("Sub string: " + arrayEkstraksiBobot[q][j].ToString());
                }
                q++;
            }

            // Training sebanyak 10 iterasi
            for (int p = 0; p < 10; p++)
            {
                // Ulangi sebanyak data ekstraksi
                for (int k = 0; k < arrayEkstraksi.GetLength(0); k++)
                {
                    // Jika data ekstraksi habis, keluar dari loop
                    if (arrayEkstraksi[k] == null) { break; }

                    // Mencari karakter di arrayKelasBobot berdasarkan karakter arrayKelas[k], kemudian ambil indeks/posisi/lokasi nya
                    int indeks = arrayKelasBobot.IndexOf(arrayKelas[k]);

                    //Mencari ekstraksi berdasarkan indeks/ posisi / lokasi
                    bobot = arrayEkstraksiBobot[indeks];

                    // Ulangi sebanyak data ekstraksi bobot
                    for (int m = 0; m < arrayEkstraksiBobot.GetLength(0); m++)
                    {
                        // Jika data ekstraksi bobot habis, keluar dari loop
                        if (arrayEkstraksiBobot[m] == null) { break; }

                        // Proses LVQ
                        bobot = elveki(bobot, arrayEkstraksiBobot[m], arrayEkstraksi[k], alpha);
                    }

                    //Update table(bobot) values(string.Join("-", arrayEkstraksiBobot[indeks])) where kelas = arrayKelasBobot[indeks]
                    OleDbCommand bbtInput = new OleDbCommand();
                    bbtInput.Connection = con;
                    bbtInput.CommandText = "Update PlatW SET bobotBaru = '" + string.Join(" ", bobot) + "' where kelasBB = '" + arrayKelasBobot[indeks] +"'";
                    int a = bbtInput.ExecuteNonQuery();
                    if (a == 0) { LogAction("Gagal Menyimpan Data"); }
                    else { LogAction("Berhasil Menyimpan Data"); Console.ReadLine(); }
                    

                }

                alpha = alpha - (0.1 * alpha);
            }
            LogAction("Data Telah Ditraining");
            con.Close();
        }

        private void BlobSquare()
        {
            Bitmap image = (Bitmap)img_Pembalik;
            image = AForge.Imaging.Image.Clone(image, PixelFormat.Format24bppRgb);
            Bitmap originalImage =(Bitmap) pbInput.Image;//resizing(bitmapOpen);
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

        private void btnKlasifikasi_Click(object sender, EventArgs e)
        {
            // Klasifikasi // Testing
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

                    if (color.R<127&& color.B < 127&& color.B < 127)
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
                int i = 0, x=5;
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
                    arrayBlobs.Add(string.Join("",array1D(cropi)));

                    var txt = new TextBox
                    {
                        Name = "txtKelas" + i,
                        Size = new Size(20, 20),
                        Location = new System.Drawing.Point(x, 205),
                        Text = " ",
                    };
                    this.Controls.Add(txt);
                    txtBoxList.Add(txt);
                                                
                    //TextBox txt = new TextBox();
                    //txt.Location = new System.Drawing.Point(x, 170);
                    //txt.Text = string.Join("", array1D(cropi));
                    //txt.Name = "textBox" + i.ToString();
                    //Form1.Controls.Add(txt);

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
                    if ((bitmap.GetPixel(i, j).R+ bitmap.GetPixel(i, j).G+ bitmap.GetPixel(i, j).B) /3 <128)
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

        private double[] elveki(double[] bobot1, double[] bobot2, int[] data, double alpha)
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
                { outputBobot[i] = bobot1[i] + (alpha * (data[i] - bobot1[i])); }
            }
            else
            {
                for (int i = 0; i < bobot1.Length; i++)
                { outputBobot[i] = bobot1[i] - (alpha * (data[i] - bobot1[i])); }
            }

            return outputBobot;
        }

        //private Int16 lvq(double[] bobot1, double[] bobot2, double[] data, int temTarget)
        //{
        //    double totalBobot1 = 0;
        //    double totalBobot2 = 0;
        //    for (int i = 0; i < bobot1.Length; i++)
        //    {
        //        totalBobot1 = totalBobot1 + Math.Pow((data[i] - bobot1[i]),2);
        //        totalBobot2 = totalBobot2 + Math.Pow((data[i] - bobot2[i]), 2);
        //    }
        //    if (Math.Pow(totalBobot1, 0.5) <= Math.Pow(totalBobot2, 0.5))
        //    {
        //        if (temTarget == 1)
        //        { return 1; }
        //        else //target sama dengan 2
        //        { return 3; }
        //    }
        //    else
        //    {
        //        if (temTarget == 2)
        //        { return 2; }
        //        else { return 4; }
        //    }
        //}

        //private double[] updateBobot(double[] bobot, double[] data, double alpha, int statusTarget)
        //{
        //    double[] temp = new double[bobot.Length];
        //    if (statusTarget == 1)
        //    {
        //        for (int i = 0; i < bobot.Length; i++)
        //        { temp[i] = bobot[i] + (alpha * (data[i] - bobot[i])); }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < bobot.Length; i++)
        //        {
        //            temp[i] = bobot[i] - (alpha * (data[i] - bobot[i]));
        //        }
        //    }
        //    return temp;
        //}


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

        private void label1_Click(object sender, EventArgs e)
        {

        }

       
    }
}
