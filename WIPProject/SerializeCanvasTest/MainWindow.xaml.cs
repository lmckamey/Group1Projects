using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerializeCanvasTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Rectangle rec = new Rectangle();
            rec.Height = 200;
            rec.Width = 200;
            SolidColorBrush brush = new SolidColorBrush(Colors.CadetBlue);
            rec.Fill = brush;
            myCanvas.Children.Add(rec);
        }

        private void bntSerialze_Click(object sender, RoutedEventArgs e)
        {
            SerializeToXML(this, myCanvas, 500, "Test.xaml");
        }
        public static void SerializeToXML(MainWindow window, Canvas canvas, int dpi, string filename)
        {
            string mystrXAML = XamlWriter.Save(canvas);
            FileStream filestream = File.Create(filename);
            StreamWriter streamwriter = new StreamWriter(filestream);
            streamwriter.Write(mystrXAML);
            streamwriter.Close();
            filestream.Close();
        }
    }
}
