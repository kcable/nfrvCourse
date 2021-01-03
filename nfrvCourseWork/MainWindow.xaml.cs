using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace nfrvCourseWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool loadedImg = false;
        private BitmapImage refImg;

        private int ImageBoxWidth;
        private int ImageBoxHeight;
        private int factor = 4; // if fact  = 1 ->8 possible colours  if fact = 4 -> 125 possilbe colours
        public MainWindow()
        {
            InitializeComponent();
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {


            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }



        private Bitmap ConvToGray(Bitmap inBitmap)
        {
            for (int x = 0; x < ImageBoxWidth; x++)
            {
                for (int y = 0; y < ImageBoxHeight; y++)

                {

                    int color = inBitmap.GetPixel(x, y).R;

                    System.Drawing.Color redVal = System.Drawing.Color.FromArgb(color, color, color);

                    inBitmap.SetPixel(x, y, redVal);

                }
            }

            return inBitmap;
        }


        private void BurkAlgorithm(Bitmap originalBitmap)
        {
            for (int y = 0; y < ImageBoxHeight - 1; y++)
            {

                for (int x = 2; x < ImageBoxWidth - 2; x++)
                {
                    int oldR = originalBitmap.GetPixel(x, y).R;
                    int oldG = originalBitmap.GetPixel(x, y).G;
                    int oldB = originalBitmap.GetPixel(x, y).B;
                    //quantizing
                    int newR = (oldR * factor / 255) * (255 / factor);
                    int newG = (oldG * factor / 255) * (255 / factor);
                    int newB = (oldB * factor / 255) * (255 / factor);
                    originalBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(newR, newG, newB));
                    int errR = oldR - newR;
                    int errG = oldG - newG;
                    int errB = oldB - newB;

                    funnelErrors(originalBitmap, x + 1, y, errR, errG, errB, 8, 32.0);
                    funnelErrors(originalBitmap, x + 2, y, errR, errG, errB, 4, 32.0);
                    funnelErrors(originalBitmap, x - 2, y + 1, errR, errG, errB, 2, 32.0);
                    funnelErrors(originalBitmap, x - 1, y + 1, errR, errG, errB, 4, 32.0);
                    funnelErrors(originalBitmap, x, y + 1, errR, errG, errB, 8, 32.0);
                    funnelErrors(originalBitmap, x + 1, y + 1, errR, errG, errB, 4, 32.0);
                    funnelErrors(originalBitmap, x + 2, y + 1, errR, errG, errB, 2, 32.0);



                    //int redColorValue = originalBitmap.GetPixel(i, j).R;
                    // int approxColor = Approximate(redColorValue);
                    //  resultBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(approxColor, approxColor, approxColor));
                }
            }


        }


        private int Approximate(int c)

        {

            if (c >= 0 && c < 63)

                return 0;

            if (c >= 63 && c < 127)

                return 63;

            if (c >= 127 && c < 191)

                return 127;

            if (c >= 191 && c < 255)

                return 191;

            else

                return 0;

        }

        private bool isLoaded()
        {
            if (loadedImg == true)
            {
                return true;
            }
            else
            {
                MessageBox.Show("You must load an image first !");
                return false;

            }
        }


        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp); // zapazvame bitmapa vuv streama
                memory.Position = 0; // podsigurqvame se che zapochvame ot nachaloto na streama
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Load_Image(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDiag = new OpenFileDialog();
            fileDiag.Filter = "png|*.png|img|*.img|jpeg|*.jpeg";
            fileDiag.DefaultExt = ".png";
            Nullable<bool> isOpen = fileDiag.ShowDialog();
            if (isOpen == true)
            {
                Uri filePath = new Uri(fileDiag.FileName);
                refImg = new BitmapImage(filePath);
                ImageBoxHeight = refImg.PixelHeight;
                ImageBoxWidth = refImg.PixelWidth;
                refBox.Source = refImg;
                refBox.Height = ImageBoxHeight;
                refBox.Width = ImageBoxWidth;
                loadedImg = true;
            }
        }





        private void funnelErrors(Bitmap originalBitmap, int x, int y, int errR, int errG, int errB, int part, double whole)
        {
            int r, g, b;

            r = originalBitmap.GetPixel(x, y).R;
            g = originalBitmap.GetPixel(x, y).G;
            b = originalBitmap.GetPixel(x, y).B;
            r = Convert.ToInt32(r + errR * part / whole);
            g = Convert.ToInt32(g + errG * part / whole);
            b = Convert.ToInt32(b + errB * part / whole);

            r = r >= 255 ? r = 255 : r;
            g = g >= 255 ? g = 255 : g;
            b = b >= 255 ? b = 255 : b;

            originalBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
        }




        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (isLoaded())
            {
                Bitmap originalBitmap = BitmapImage2Bitmap(refImg);
                BurkAlgorithm(originalBitmap);
                resultBox.Width = ImageBoxWidth;
                resultBox.Height = ImageBoxHeight;
                resultBox.Source = Bitmap2BitmapImage(originalBitmap);

            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (isLoaded())
            {
                Bitmap resultBitmap = new Bitmap(ImageBoxWidth, ImageBoxHeight);
                Bitmap originalBitmap = BitmapImage2Bitmap(refImg);
                for (int i = 0; i < ImageBoxWidth; i++)
                {
                    for (int j = 0; j < ImageBoxHeight; j++)
                    {
                        int redColorValue = originalBitmap.GetPixel(i, j).R;
                        int approxColor = Approximate(redColorValue);
                        resultBitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(approxColor, approxColor, approxColor));
                    }
                }
                resultBox.Width = ImageBoxWidth;
                resultBox.Height = ImageBoxHeight;
                resultBox.Source = Bitmap2BitmapImage(resultBitmap);


            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (isLoaded())
            {
                Bitmap originalBitmap = BitmapImage2Bitmap(refImg);
                originalBitmap = ConvToGray(originalBitmap);
                BurkAlgorithm(originalBitmap);
                resultBox.Width = ImageBoxWidth;
                resultBox.Height = ImageBoxHeight;
                resultBox.Source = Bitmap2BitmapImage(originalBitmap);

            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            if (isLoaded())
            {

                Bitmap originalBitmap = BitmapImage2Bitmap(refImg);

                resultBox.Width = ImageBoxWidth;
                resultBox.Height = ImageBoxHeight;
                resultBox.Source = Bitmap2BitmapImage(ConvToGray(originalBitmap));


            }

        }
    }
}
