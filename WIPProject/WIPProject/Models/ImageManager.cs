using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WIPProject.Models
{
    static class ImageManager
    {
        private static int imageCount = 1;

        public static bool SaveImage(string filePath, string userName, Canvas canvas, Window window)
        {
            string path = $"{filePath}\\{userName}-CanvasImage";

            while (File.Exists(path))
            {
                path = $"{filePath}\\{userName}-CanvasImage{imageCount++}";
            }

            Size size = new Size(window.Width, window.Height);
            canvas.Measure(size);
            //uscBasicDrawing.cnvDrawArea.Arrange(new Rect(size));

            const int dpi = 96;

            var rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );
            rtb.Render(canvas);

            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

            using (var fileStream = File.Create(path))
            {
                enc.Save(fileStream);
            }

            return true;
        }

        public static bool SaveImageToDesktop(string userName, Canvas canvas, Window window)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            return SaveImage(path, userName, canvas, window);
        }
    }
}
