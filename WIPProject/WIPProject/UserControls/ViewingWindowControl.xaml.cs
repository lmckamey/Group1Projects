using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WIPProject.Models;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for ViewingWindowControl.xaml
    /// </summary>
    public partial class ViewingWindowControl : UserControl
    {
        public string UserName { get; set; }

        public ViewingWindowControl()
        {
            InitializeComponent();
        }

        private void lblWow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayCompliment("Colin", "WOW!", Colors.LightBlue);
        }

        private void lblNice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayCompliment("Colin", "Nice Job!", Colors.IndianRed);
        }

        private void lblCool_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayCompliment("Colin", "Cool!", Colors.MediumPurple);
        }

        private void lblThanks_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DisplayCompliment("Colin", "Thanks!", Colors.LightGoldenrodYellow);
        }

        private void DisplayCompliment(string username, string message, Color backGroundColor)
        {
            Compliment c = new Compliment();
            Border b = c.CreateTextBlock(username, message, backGroundColor);

            c.MaxHorizontal = (int)cnvViewArea.ActualWidth;
            c.MaxVertical = (int)cnvViewArea.ActualHeight;

            c.CreateTimer(100, 35, true);
            
            c.ComplimentDestroyHandler += DestroyCompliment;

            cnvViewArea.Children.Add(b);
        }

        private void DestroyCompliment(Border sender)
        {
            cnvViewArea.Children.Remove(sender);
        }
    }
}
