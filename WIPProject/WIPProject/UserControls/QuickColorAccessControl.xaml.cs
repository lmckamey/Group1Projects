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

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for QuickColorAccessControl.xaml
    /// </summary>
    public partial class QuickColorAccessControl : UserControl
    {
        ColorPickerControl cpc;
        public QuickColorAccessControl(ColorPickerControl c)
        {
            InitializeComponent();

            cpc = c;
        }

        private void QuickColorSelectionPress(object sender, MouseButtonEventArgs e)
        {
            wrpQuickColors.Visibility = Visibility.Hidden;

            cpc.elpCurrentColor.Fill = ((Border)sender).Background;
            cpc.bdc.mouseOldPosition = cpc.bdc.mouseCurrentPosition;

            byte r = ((SolidColorBrush)cpc.elpCurrentColor.Fill).Color.R;
            byte g = ((SolidColorBrush)cpc.elpCurrentColor.Fill).Color.G;
            byte b = ((SolidColorBrush)cpc.elpCurrentColor.Fill).Color.B;
            byte a = ((SolidColorBrush)cpc.elpCurrentColor.Fill).Color.A;
            cpc.sldRed.Value = r;
            cpc.sldGreen.Value = g;
            cpc.sldBlue.Value = b;
            cpc.sldAlpha.Value = a;
        }
    }
}
