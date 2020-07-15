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
using System.Text.RegularExpressions;
using Emgu.CV.CvEnum;

namespace ProjectPlat_Otw
{
    public partial class Form1 : Form
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
        private System.Drawing.Image Imageee;
        private ImageAttributes imageAttributes, imageAttributesBlob;
        private List<int> sequenceCodeList;
        private OleDbConnection con = new OleDbConnection();
        private List<TextBox> txtBoxList;
        private double[] bobot, data;
        private double alpha;
        private int r, g, b, w, h, size;
        private Rectangle newrect;
        private string str, imgChain;

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
            img_Pembalik = switchColor(new Bitmap(img_DilasiBin.ToBitmap()));
            //Histogram Equalization
            //pbOtsu.Image = img_DilasiBin.ToBitmap();
            //pictOtsu.Image = imgOtsu.ToBitmap();
            //pictOtsu.SizeMode = PictureBoxSizeMode.Zoom;
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
            imgChain = ChainCode(cropi);
            //Console.WriteLine(imgChain);
            pbOtsu.Image = cropi;
            arrayBlobs.Clear();
            txtBoxList.Clear();
            arrayBlobs.Add(string.Join("", imgChain));
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
                    insert.CommandText = "INSERT INTO PlatTrainChaincode (Kelas, array1D) values ('" + txtBoxList[i].Text + "','" + arrayBlobs[i] + "' ) ";
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
                    insert.CommandText = "INSERT INTO PlatTrainChaincode (Kelas, array1D) values ('" + txtBoxList[i].Text + "','" + arrayBlobs[i] + "' ) ";
                    //insert.CommandText = "Update PlatTrainChaincode SET array1D = '" + arrayBlobs[i] + "' where kelas = '"+ txtBoxList[i].Text +"' ";
                    //bbtInput.CommandText = "Update PlatBBChaincode SET bobotBaru = '" + string.Join(" ",bbtString) + "' where kelasBB = '" + arrayKelasBobot[indeks] +"'";
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
            //string sql = "SELECT * FROM PlatTrainChaincode";
            OleDbCommand get = new OleDbCommand(sql, con);
            OleDbDataReader row = get.ExecuteReader();

            List<string> arrayKelas = new List<string>();
            double[][] arrayEkstraksi = new double[1000][];

            // Memasukkan data ke dalam array
            int i = 0;
            while (row.Read())
            {
                arrayKelas.Add(row[1].ToString());
                arrayEkstraksi[i] = new double[row[2].ToString().Length];

                for (int j = 0; j < row[2].ToString().Length; j++)
                {
                    arrayEkstraksi[i][j] = double.Parse(row[2].ToString().Substring(j, 1));
                }   
                i++;
            }

            // Ambil data bobot
            //string sqli = "SELECT * FROM PlatBBChaincode";
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
                arrayEkstraksiBobot[q] = new double[rows[2].ToString().Length];

                for (int j = 0; j < rows[2].ToString().Length; j++)
                {
                    arrayEkstraksiBobot[q][j] = double.Parse(rows[2].ToString().Substring(j, 1));
                    //Console.WriteLine("Sub string: " + arrayEkstraksiBobot[q][j].ToString());
                }
                q++;
            }

            // Training sebanyak 10 iterasi/epoch
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
                    String[] bbtString = bobot.Select(x => x.ToString()).ToArray();
                    //Console.WriteLine(bbtString);
                    OleDbCommand bbtInput = new OleDbCommand();
                    bbtInput.Connection = con;
                    bbtInput.CommandText = "Update PlatW SET bobotBaru = '" + string.Join(" ", bbtString) + "' where kelasBB = '" + arrayKelasBobot[indeks] + "'";
                    //bbtInput.CommandText = "Update PlatBBChaincode SET bobotBaru = '" + string.Join(" ",bbtString) + "' where kelasBB = '" + arrayKelasBobot[indeks] +"'";
                    int a = bbtInput.ExecuteNonQuery();
                    if (a == 0) { LogAction("Gagal Menyimpan Data"); }
                    else { LogAction("Berhasil Menyimpan Data"); Console.ReadLine(); }
                    

                }

                alpha = alpha - (0.1 * alpha);
            }
            LogAction("Data Telah Ditraining");
            con.Close();
        }

        private void btnKlasifikasi_Click_1(object sender, EventArgs e)
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
                    if (arrayEkstraksiBobot[j]==null) { break; }

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
        private void FindLicensePlate(
         VectorOfVectorOfPoint contours, int[,] hierachy, int idx, IInputArray gray, IInputArray canny,
         List<IInputOutputArray> licensePlateImagesList, List<IInputOutputArray> filteredLicensePlateImagesList, List<RotatedRect> detectedLicensePlateRegionList,
         List<String> licenses)
        {
            for (; idx >= 0; idx = hierachy[idx, 0])
            {
                int numberOfChildren = GetNumberOfChildren(hierachy, idx);
                //if it does not contains any children (charactor), it is not a license plate region
                if (numberOfChildren == 0) continue;

                using (VectorOfPoint contour = contours[idx])
                {
                    if (CvInvoke.ContourArea(contour) > 400)
                    {
                        if (numberOfChildren < 3)
                        {
                            //If the contour has less than 3 children, it is not a license plate (assuming license plate has at least 3 charactor)
                            //However we should search the children of this contour to see if any of them is a license plate
                            FindLicensePlate(contours, hierachy, hierachy[idx, 2], gray, canny, licensePlateImagesList,
                               filteredLicensePlateImagesList, detectedLicensePlateRegionList, licenses);
                            continue;
                        }

                        RotatedRect box = CvInvoke.MinAreaRect(contour);
                        if (box.Angle < -45.0)
                        {
                            float tmp = box.Size.Width;
                            box.Size.Width = box.Size.Height;
                            box.Size.Height = tmp;
                            box.Angle += 90.0f;
                        }
                        else if (box.Angle > 45.0)
                        {
                            float tmp = box.Size.Width;
                            box.Size.Width = box.Size.Height;
                            box.Size.Height = tmp;
                            box.Angle -= 90.0f;
                        }

                        double whRatio = (double)box.Size.Width / box.Size.Height;
                        if (!(3.0 < whRatio && whRatio < 10.0))
                        //if (!(1.0 < whRatio && whRatio < 2.0))
                        {
                            //if the width height ratio is not in the specific range,it is not a license plate 
                            //However we should search the children of this contour to see if any of them is a license plate
                            //Contour<Point> child = contours.VNext;
                            if (hierachy[idx, 2] > 0)
                                FindLicensePlate(contours, hierachy, hierachy[idx, 2], gray, canny, licensePlateImagesList,
                                   filteredLicensePlateImagesList, detectedLicensePlateRegionList, licenses);
                            continue;
                        }

                        using (UMat tmp1 = new UMat())
                        using (UMat tmp2 = new UMat())
                        {
                            PointF[] srcCorners = box.GetVertices();

                            PointF[] destCorners = new PointF[] {
                        new PointF(0, box.Size.Height - 1),
                        new PointF(0, 0),
                        new PointF(box.Size.Width - 1, 0),
                        new PointF(box.Size.Width - 1, box.Size.Height - 1)};

                            using (Mat rot = CameraCalibration.GetAffineTransform(srcCorners, destCorners))
                            {
                                CvInvoke.WarpAffine(gray, tmp1, rot, Size.Round(box.Size));
                            }

                            //resize the license plate such that the front is ~ 10-12. This size of front results in better accuracy from tesseract
                            Size approxSize = new Size(240, 180);
                            double scale = Math.Min(approxSize.Width / box.Size.Width, approxSize.Height / box.Size.Height);
                            Size newSize = new Size((int)Math.Round(box.Size.Width * scale), (int)Math.Round(box.Size.Height * scale));
                            CvInvoke.Resize(tmp1, tmp2, newSize, 0, 0, Inter.Cubic);

                            //removes some pixels from the edge
                            int edgePixelSize = 2;
                            Rectangle newRoi = new Rectangle(new Point(edgePixelSize, edgePixelSize),
                               tmp2.Size - new Size(2 * edgePixelSize, 2 * edgePixelSize));
                            UMat plate = new UMat(tmp2, newRoi);

                            UMat filteredPlate = FilterPlate(plate);

                            Tesseract.Character[] words;
                            StringBuilder strBuilder = new StringBuilder();
                            using (UMat tmp = filteredPlate.Clone())
                            {
                                _ocr.Recognize(tmp);
                                words = _ocr.GetCharacters();

                                if (words.Length == 0) continue;

                                for (int i = 0; i < words.Length; i++)
                                {
                                    strBuilder.Append(words[i].Text);
                                }
                            }

                            licenses.Add(strBuilder.ToString());
                            licensePlateImagesList.Add(plate);
                            filteredLicensePlateImagesList.Add(filteredPlate);
                            detectedLicensePlateRegionList.Add(box);

                        }
                    }
                }
            }
        }
        private static UMat FilterPlate(UMat plate)
        {
            UMat thresh = new UMat();
            CvInvoke.Threshold(plate, thresh, 120, 255, ThresholdType.BinaryInv);
            //Image<Gray, Byte> thresh = plate.ThresholdBinaryInv(new Gray(120), new Gray(255));

            Size plateSize = plate.Size;
            using (Mat plateMask = new Mat(plateSize.Height, plateSize.Width, DepthType.Cv8U, 1))
            using (Mat plateCanny = new Mat())
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                plateMask.SetTo(new MCvScalar(255.0));
                CvInvoke.Canny(plate, plateCanny, 100, 50);
                CvInvoke.FindContours(plateCanny, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                int count = contours.Size;
                for (int i = 1; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {

                        Rectangle rect = CvInvoke.BoundingRectangle(contour);
                        if (rect.Height > (plateSize.Height >> 1))
                        {
                            rect.X -= 1; rect.Y -= 1; rect.Width += 2; rect.Height += 2;
                            Rectangle roi = new Rectangle(Point.Empty, plate.Size);
                            rect.Intersect(roi);
                            CvInvoke.Rectangle(plateMask, rect, new MCvScalar(), -1);
                            //plateMask.Draw(rect, new Gray(0.0), -1);
                        }
                    }

                }

                thresh.SetTo(new MCvScalar(), plateMask);
            }

            CvInvoke.Erode(thresh, thresh, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            CvInvoke.Dilate(thresh, thresh, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

            return thresh;
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

        private string ChainCode(Bitmap bit)
        {
            str = "";
            //size = 10; // set size	

            //w = size;
            //h = size;

            //copy = new Bitmap(bit);
            //newrect = new Rectangle(0, 0, w, h);
            //newimg = new Bitmap(w, h);

            //newimg.SetResolution(copy.HorizontalResolution, copy.VerticalResolution);

            //graphics = Graphics.FromImage(newimg);
            //graphics.CompositingMode = CompositingMode.SourceCopy;
            //graphics.CompositingQuality = CompositingQuality.HighQuality;
            //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //graphics.SmoothingMode = SmoothingMode.HighQuality;
            //graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            //ImageAttributes wrapMode = new ImageAttributes();
            //wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            //graphics.DrawImage(copy, newrect, 0, 0, copy.Width, copy.Height, GraphicsUnit.Pixel, wrapMode);

            Color c0, c1, c2, c3, c4, c5, c6, c7;
            int jr, r0, r1, r2, r3, r4, r5, r6, r7;

            for (int x = 0; x < bit.Width; x++)
            {
                if (x == 0 || (x > 1 && x % 2 == 0))
                {
                    for (int y = bit.Height - 1; y >= 0; y--)
                    {
                        Color clr = bit.GetPixel(x, y);
                        if (x < bit.Width - 1)
                        {
                            c0 = bit.GetPixel(x + 1, y);
                            r0 = (c0.R + c0.G + c0.B) / 3;
                            if (r0 > 127) { r0 = 0; }
                            else { r0 = 1; }
                        }
                        else { r0 = 0; }

                        if (x < bit.Width - 1 && y > 0)
                        {
                            c1 = bit.GetPixel(x + 1, y - 1);
                            r1 = (c1.R + c1.G + c1.B) / 3;
                            if (r1 > 127) { r1 = 0; }
                            else { r1 = 1; }
                        }
                        else { r1 = 0; }

                        if (y > 0)
                        {
                            c2 = bit.GetPixel(x, y - 1);
                            r2 = (c2.R + c2.G + c2.B) / 3;
                            if (r2 > 127) { r2 = 0; }
                            else { r2 = 1; }
                        }
                        else { r2 = 0; }

                        if (x > 0 && y > 0)
                        {
                            c3 = bit.GetPixel(x - 1, y - 1);
                            r3 = (c3.R + c3.G + c3.B) / 3;
                            if (r3 > 127) { r3 = 0; }
                            else { r3 = 1; }
                        }
                        else { r3 = 0; }

                        if (x > 0)
                        {
                            c4 = bit.GetPixel(x - 1, y);
                            r4 = (c4.R + c4.G + c4.B) / 3;
                            if (r4 > 127) { r4 = 0; }
                            else { r4 = 1; }
                        }
                        else { r4 = 0; }

                        if (x > 0 && y < bit.Height - 1)
                        {
                            c5 = bit.GetPixel(x - 1, y + 1);
                            r5 = (c5.R + c5.G + c5.B) / 3;
                            if (r5 > 127) { r5 = 0; }
                            else { r5 = 1; }
                        }
                        else { r5 = 0; }

                        if (y < bit.Height - 1)
                        {
                            c6 = bit.GetPixel(x, y + 1);
                            r6 = (c6.R + c6.G + c6.B) / 3;
                            if (r6 > 127) { r6 = 0; }
                            else { r6 = 1; }
                        }
                        else { r6 = 0; }

                        if (x < bit.Width - 1 && y < bit.Height - 1)
                        {
                            c7 = bit.GetPixel(x + 1, y + 1);
                            r7 = (c7.R + c7.G + c7.B) / 3;
                            if (r7 > 127) { r7 = 0; }
                            else { r7 = 1; }
                        }
                        else { r7 = 0; }

                        jr = r0 + r1 + r2 + r3 + r4 + r5 + r6 + r7;
                        str = str + jr.ToString();
                    }
                }
                else if (x == 1 || (x > 1 && x % 2 == 1))
                {
                    for (int y = 0; y < bit.Height; y++)
                    {
                        Color clr = bit.GetPixel(x, y);
                        if (x < bit.Width - 1)
                        {
                            c0 = bit.GetPixel(x + 1, y);
                            r0 = (c0.R + c0.G + c0.B) / 3;
                            if (r0 > 127) { r0 = 0; }
                            else { r0 = 1; }
                        }
                        else { r0 = 0; }

                        if (x < bit.Width - 1 && y > 0)
                        {
                            c1 = bit.GetPixel(x + 1, y - 1);
                            r1 = (c1.R + c1.G + c1.B) / 3;
                            if (r1 > 127) { r1 = 0; }
                            else { r1 = 1; }
                        }
                        else { r1 = 0; }

                        if (y > 0)
                        {
                            c2 = bit.GetPixel(x, y - 1);
                            r2 = (c2.R + c2.G + c2.B) / 3;
                            if (r2 > 127) { r2 = 0; }
                            else { r2 = 1; }
                        }
                        else { r2 = 0; }

                        if (x > 0 && y > 0)
                        {
                            c3 = bit.GetPixel(x - 1, y - 1);
                            r3 = (c3.R + c3.G + c3.B) / 3;
                            if (r3 > 127) { r3 = 0; }
                            else { r3 = 1; }
                        }
                        else { r3 = 0; }

                        if (x > 0)
                        {
                            c4 = bit.GetPixel(x - 1, y);
                            r4 = (c4.R + c4.G + c4.B) / 3;
                            if (r4 > 127) { r4 = 0; }
                            else { r4 = 1; }
                        }
                        else { r4 = 0; }

                        if (x > 0 && y < bit.Height - 1)
                        {
                            c5 = bit.GetPixel(x - 1, y + 1);
                            r5 = (c5.R + c5.G + c5.B) / 3;
                            if (r5 > 127) { r5 = 0; }
                            else { r5 = 1; }
                        }
                        else { r5 = 0; }

                        if (y < bit.Height - 1)
                        {
                            c6 = bit.GetPixel(x, y + 1);
                            r6 = (c6.R + c6.G + c6.B) / 3;
                            if (r6 > 127) { r6 = 0; }
                            else { r6 = 1; }
                        }
                        else { r6 = 0; }

                        if (x < bit.Width - 1 && y < bit.Height - 1)
                        {
                            c7 = bit.GetPixel(x + 1, y + 1);
                            r7 = (c7.R + c7.G + c7.B) / 3;
                            if (r7 > 127) { r7 = 0; }
                            else { r7 = 1; }
                        }
                        else { r7 = 0; }

                        jr = r0 + r1 + r2 + r3 + r4 + r5 + r6 + r7;
                        str = str + jr.ToString();
                    }
                }
            }

            return str;
        }

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
