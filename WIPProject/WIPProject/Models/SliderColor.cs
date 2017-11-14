using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WIPProject.Models
{
    public class SliderColor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
        private byte Red = 0;
        private byte Green = 0;
        private byte Blue = 0;
        private byte Alpha = 255;

        public SolidColorBrush ColorBrush
        {
            get { return solidColorBrush; }
            set
            {
                solidColorBrush = value;
                NotifyStateChanged("ColorBrush");
            }
        }

        public void UpdateColors(int r = -1, int g = -1, int b = -1, int a = -1)
        {
            Red = r >= 0 && r <= 255 ? (byte)r : Red;
            Green = g >= 0 && g <= 255 ? (byte)g : Green;
            Blue = b >= 0 && b <= 255 ? (byte)b : Blue;
            Alpha = a >= 0 && a <= 255 ? (byte)a : Alpha;

            Color c = new Color() { R = Red, G = Green, B = Blue, A = Alpha };

            ColorBrush = new SolidColorBrush(c);
        }

        public void NotifyStateChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
