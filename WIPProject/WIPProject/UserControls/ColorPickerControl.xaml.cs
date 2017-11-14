using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WIPProject.Models;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for ColorPickerControl.xaml
    /// </summary>
    public partial class ColorPickerControl : UserControl
    {
        public ColorPickerControl()
        {
            InitializeComponent();

            SliderColor sc = new SliderColor();
            UpdateEllipse();

            elpCurrentColor.DataContext = sc;
            //Binding b = new Binding("ColorBrush");
            //SetBinding(Ellipse.FillProperty, b);
        }

        private void sldRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateEllipse(r: (int)((Slider)sender).Value);
        }

        private void sldGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateEllipse(g: (int)((Slider)sender).Value);
        }

        private void sldBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateEllipse(b: (int)((Slider)sender).Value);
        }

        private void sldAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateEllipse(a: (int)((Slider)sender).Value);
        }

        private void UpdateEllipse(int r = -1, int g = -1, int b = -1, int a = -1)
        {
            if (elpCurrentColor != null && elpCurrentColor.DataContext != null)
                ((SliderColor)elpCurrentColor.DataContext).UpdateColors(r, g, b, a);
        }

        private Color GetEllipseColor()
        {
            Color c = new Color()
            {
                R = (byte)sldRed.Value,
                G = (byte)sldGreen.Value,
                B = (byte)sldBlue.Value,
                A = (byte)sldAlpha.Value,
            };

            return c;
        }

        private void SetEllipseColor(Color color)
        {
            if (elpCurrentColor != null)
                elpCurrentColor.Fill = new SolidColorBrush(color);
        }
    }
}
