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
using System.Windows.Shapes;
using WIPProject.UserControls;

using WIPProject.Networking;
using System.Windows.Threading;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using WIPProject.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Animation;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace WIPProject
{
    /// <summary>
    /// Interaction logic for DrawingPage.xaml
    /// </summary>
    public partial class DrawingPage : Window
    {
        private bool isActive;
        public bool Active
        {
            get { return isActive; }
            set
            {
                isActive = value;
                //if (updateTimer != null)
                //{
                //    if (isActive)
                //        updateTimer.Start();
                //    else
                //        updateTimer.Stop();
                //}
            }
        }
        public string userName;
        public MainWindow main;

        private int startingWindowWidth;
        private int startingWindowHeight;

        //private string filePath = @"C: \Users\Colin Misbach\Desktop\serializedCanvas.txt";
        private Canvas tempCanvas = new Canvas();

        Color[] userColors = new Color[]
        { Colors.DarkRed, Colors.Goldenrod, Colors.HotPink, Colors.BlueViolet,
            Colors.ForestGreen, Colors.DodgerBlue};
        int userColor;

        public DrawingPage(bool active, MainWindow window = null, string name = "")
        {
            InitializeComponent();
            Active = active;

            main = window;
            userName = name;

            uscRoomSelector.page = this;

            uscBasicDrawing.drawingPage = this;

            mnuChatOptions.MouseLeave += MnuChatOptions_MouseLeave;

            Random r = new Random();
            int x = r.Next(0, userColors.Length - 1);
            userColor = x;

            startingWindowWidth = (int)Width;
            startingWindowHeight = (int)Height;
        }

        private void MnuChatOptions_MouseLeave(object sender, MouseEventArgs e)
        {
            mnuChatOptions.Visibility = Visibility.Hidden;
        }

        public void HideChatOptions(object sender, RoutedEventArgs e)
        {
            mnuChatOptions.Visibility = Visibility.Hidden;
        }

        private void txbChatBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return))
            {
                SendMessage();
            }
        }

        public void AddMessage(string userName, string message, int color) {
            this.Dispatcher.Invoke(() =>
            {
                TextBlock tb = new TextBlock();
                tb.Padding = new Thickness(0, 0, 5, 0);
                tb.TextAlignment = TextAlignment.Left;
                tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                tb.VerticalAlignment = VerticalAlignment.Top;
                tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEFFFF"));
                tb.FontFamily = new FontFamily("Maiandra GD");
                tb.Background = null;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(5, 0, 0, 5);

                Run user = new Run($"{userName}:");
                byte r = (byte)(color >> 8);
                byte g = (byte)((color >> 4) - (r << 4));
                byte b = (byte)((color) - (r << 8) - (g << 4));
                user.Foreground = new SolidColorBrush(Color.FromRgb(r, g, b));
                user.MouseEnter += User_MouseEnter;
                user.MouseLeave += User_MouseLeave;
                user.MouseDown += User_MouseDown;
                Run text = new Run($" {message}");
                text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEFFFF"));
                tb.Inlines.Add(user);
                tb.Inlines.Add(text);

                tblChatWindow.Children.Add(tb);

                ChangeFontSizes();
            });
            
        }

        public void DrawMessage(string[] lines) {
            this.Dispatcher.Invoke(() => {

                foreach (string s in lines) {
                    var line = XamlReader.Parse(s);
                    if (line is Line) {
                        uscBasicDrawing.cnvDrawArea.Children.Add((Line)line);                     
                    }

                }
                    

            });
        }

        public void ToggleDrawing() {
            Active = !Active;
        }

        private void SendMessage()
        {
            TextBlock tb = new TextBlock();
            tb.Padding = new Thickness(0, 0, 5, 0);
            tb.TextAlignment = TextAlignment.Left;
            tb.HorizontalAlignment = HorizontalAlignment.Stretch;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEFFFF"));
            tb.FontFamily = new FontFamily("Maiandra GD");
            tb.Background = null;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Margin = new Thickness(5, 0, 0, 5);

            Run user = new Run($"{userName}:");
            //int color = 0;
            //int.TryParse("0xFFFFFF", NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color);
            //byte r = (byte)(color >> 8);
            //byte g = (byte)((color >> 4) - (r << 4));
            //byte b = (byte)((color) - (r << 8) - (g << 4));
            user.Foreground = new SolidColorBrush(userColors[userColor]);
            //user.Foreground = new SolidColorBrush(userColors[userColor]);
            user.MouseEnter += User_MouseEnter;
            user.MouseLeave += User_MouseLeave;
            user.MouseDown += User_MouseDown;
            Run text = new Run($" {tbxChatBox.Text}");
            text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CEFFFF"));
            tb.Inlines.Add(user);
            tb.Inlines.Add(text);

            tblChatWindow.Children.Add(tb);

            ChangeFontSizes();

            //tblChatWindow.Text = $"{tblChatWindow.Text}\n{userName}: {tbxChatBox.Text}";
            Client.WriteChatMessage(userName, tbxChatBox.Text);
            //tbxChatWindow.AppendText($"\n{userName}: {tbxChatBox.Text}\n");

            tbxChatBox.Clear();

            scvChatScrollbar.ScrollToBottom();
        }

        private void User_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                mnuChatOptions.Visibility = Visibility.Visible;
                Grid.SetColumn(mnuChatOptions, 2);
                Panel.SetZIndex(mnuChatOptions, 5);
                Point mouse = e.GetPosition(scvChatScrollbar);
                mouse.X -= 10;
                mouse.Y -= 20;
                mnuChatOptions.Margin = new Thickness(mouse.X, mouse.Y, 0, 0);
            }
        }

        private void User_MouseLeave(object sender, MouseEventArgs e)
        {
            Run r = sender as Run;
            Color c = ((SolidColorBrush)r.Foreground).Color;

            ((Run)sender).Foreground = new SolidColorBrush(
                new Color() { R = c.R, G = c.G, B = c.B, A = 255 });

            r.Cursor = Cursors.Arrow;
        }

        private void User_MouseEnter(object sender, MouseEventArgs e)
        {
            Run r = sender as Run;
            Color c = ((SolidColorBrush)r.Foreground).Color;

            ((Run)sender).Foreground = new SolidColorBrush(
                new Color() {R=c.R, G=c.G, B=c.B, A=125});

            r.Cursor = Cursors.Hand;
        }

        private void btnSwitchMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnViewActiveDraw_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnModeChange_Click(object sender, RoutedEventArgs e)
        {
            ReverseVisibility(uscBasicDrawing);
            ReverseVisibility(uscViewer);
        }

        private void ReverseVisibility(UIElement element)
        {
            if (element.Visibility == Visibility.Hidden)
            {
                element.Visibility = Visibility.Visible;
            }
            else
            {
                element.Visibility = Visibility.Hidden;
            }
        }

        private void btnRoomSelect_Click(object sender, RoutedEventArgs e)
        {
            ReverseVisibility(uscRoomSelector);

            double x = stckPnlSideMenu.ActualWidth;

            //uscRoomSelector.Margin = new Thickness(x, 50, 0, 0);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            uscBasicDrawing.updateTimer.Stop();
            Client.Shutdown();
            System.Windows.Application.Current.Shutdown();
            //main.Close();
        }

        private void uscBasicDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            uscRoomSelector.Visibility = Visibility.Hidden;
        }

        private void lblToggleChat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lblToggleChat.Content.Equals(">"))
            {
                HideChat();
            }
            else
            {
                ShowChat();
            }

            uscBasicDrawing.ignoreNextLines = true;
        }

        private void HideChat()
        {
            lblToggleChat.Content = "<";
            grdRoot.ColumnDefinitions.ElementAt(2).Width = new GridLength(0, GridUnitType.Star);
        }

        private void ShowChat()
        {
            lblToggleChat.Content = ">";
            grdRoot.ColumnDefinitions.ElementAt(2).Width = new GridLength(3, GridUnitType.Star);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeFontSizes();
        }

        private void ChangeFontSizes()
        {
            float mult = GetSizeMultiplier();
            ApplySizeChanges(mult);
        }

        public float GetSizeMultiplier()
        {
            float size;

            float widthMult = (float)(ActualWidth / startingWindowWidth);
            float heightMult = (float)(ActualHeight / startingWindowHeight);

            size = (widthMult > heightMult) ? heightMult : widthMult;

            return size;
        }

        private void ApplySizeChanges(float multiplier)
        {
            int modeSize = (int)(11 * multiplier);
            int roomsSize = (int)(9 * multiplier);
            int friendLabelSize = (int)(9 * multiplier);
            int friendListSize = (int)(9 * multiplier);
            int settingsSize = (int)(9 * multiplier);
            int chatSize = (int)(12 * multiplier);

            int logoWidth = (int)(35 * multiplier);
            int logoHeight = (int)(26 * multiplier);

            lblLogo.Width = logoWidth;
            lblLogo.Height = logoHeight;

            btnModeChange.FontSize = modeSize;
            btnRoomSelect.FontSize = roomsSize;
            lblFriends.FontSize = friendLabelSize;
            lbxFriendList.FontSize = friendListSize;
            btnSettings.FontSize = settingsSize;
            lblTextWatermark.FontSize = chatSize;
            tbxChatBox.FontSize = chatSize;
            for (int i = 0; i < tblChatWindow.Children.Count; ++i)
            {
                ((TextBlock)tblChatWindow.Children[i]).FontSize = chatSize;
            }
            //tblChatWindow.FontSize = chatSize;

            int drawingControlSize = (int)(8 * multiplier);

            uscBasicDrawing.btnBucket.FontSize = drawingControlSize;
            uscBasicDrawing.btnClear.FontSize = drawingControlSize;
            uscBasicDrawing.btnEraser.FontSize = drawingControlSize;
            uscBasicDrawing.btnUndo.FontSize = drawingControlSize;
            
            uscBasicDrawing.uscColorPicker.lblR.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.lblG.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.lblB.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.lblA.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.tbxR.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.tbxG.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.tbxB.FontSize = drawingControlSize;
            uscBasicDrawing.uscColorPicker.tbxA.FontSize = drawingControlSize;

            uscBasicDrawing.uscBrushSize.lblSize.FontSize = drawingControlSize;
            uscBasicDrawing.uscBrushSize.tbxSize.FontSize = drawingControlSize;

            int currentColorDims = (int)(40 * multiplier);
            uscBasicDrawing.uscColorPicker.elpCurrentColor.Width = currentColorDims;
            uscBasicDrawing.uscColorPicker.elpCurrentColor.Height = currentColorDims;

            int width = (int)(94 * multiplier);
            uscBasicDrawing.uscColorPicker.grdSliders.Width = width;
        }

        private void tbxChatBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbxChatBox.GetLineText(0).Length > 0)
            {
                lblTextWatermark.Visibility = Visibility.Hidden;
            }
            else if (tbxChatBox.GetLineText(0).Length <= 0)
            {
                lblTextWatermark.Visibility = Visibility.Visible;
            }
        }
    }
}
