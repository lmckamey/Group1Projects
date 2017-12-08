﻿using System;
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
using WIPProject.Enums;
using WIPProject.Models;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for BasicViewingControl.xaml
    /// </summary>
    public partial class BasicViewingControl : UserControl
    {
        public BasicViewingControl()
        {
            InitializeComponent();
        }

        private void lblWow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Compliment.CreateNewCompliment(ComplimentType.WOW, cnvDrawArea);
        }

        private void lblCool_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Compliment.CreateNewCompliment(ComplimentType.COOL, cnvDrawArea);
        }

        private void lblNice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Compliment.CreateNewCompliment(ComplimentType.NICE, cnvDrawArea);
        }

        private void lblThanks_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Compliment.CreateNewCompliment(ComplimentType.THANKS, cnvDrawArea);
        }
    }
}
