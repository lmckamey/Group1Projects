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
using System.Windows.Shapes;
using WIPProject.UserControls;

using WIPProject.Networking;
using System.Windows.Threading;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using WIPProject.Models;
using System.Runtime.Serialization.Formatters.Binary;

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
                if (updateTimer != null)
                {
                    if (isActive)
                        updateTimer.Start();
                    else
                        updateTimer.Stop();
                }
            }
        }
        public string userName;
        public MainWindow main;

        private DispatcherTimer updateTimer;

        private int startingWindowWidth;
        private int startingWindowHeight;

        //private string filePath = @"C: \Users\Colin Misbach\Desktop\serializedCanvas.txt";
        private Canvas tempCanvas = new Canvas();

        public DrawingPage(bool active, MainWindow window = null, string name = "")
        {
            InitializeComponent();
            Active = active;

            main = window;
            userName = name;

            uscRoomSelector.page = this;

            startingWindowWidth = (int)Width;
            startingWindowHeight = (int)Height;

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = new TimeSpan(0,0,0,0,1000);
            updateTimer.Tick += UpdateTimer_Tick;
            
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Active)
            {

                Line[] lines = uscBasicDrawing.dirtyLines;
                Client.WriteDrawMessage(lines);

                uscBasicDrawing.ClearDirtyLines();
                tempCanvas.Children.Clear();
            }
        }

        private void txbChatBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return))
            {
                lblTextWatermark.Visibility = Visibility.Visible;
                SendMessage();
            }
            else if (tbxChatBox.GetLineText(0).Length >= 0 && !e.Key.Equals(Key.Back) 
                && (e.Key >= Key.A && e.Key <= Key.Z))
            {
                lblTextWatermark.Visibility = Visibility.Hidden;
            }
            else if (e.Key.Equals(Key.Back) && tbxChatBox.GetLineText(0).Length <= 1)
            {
                lblTextWatermark.Visibility = Visibility.Visible;
            }
        }

        public void AddMessage(string userName, string message, string color) {
            this.Dispatcher.Invoke(() =>
            {
                tblChatWindow.Text = $"{tblChatWindow.Text}\n{userName}: {message}";
            });
            
        }

        public void DrawMessage(string[] lines) {
            this.Dispatcher.Invoke(() => {

                for(int i = 0; i < lines.Length; i++) {
                    Object line = null;
                    try {
                        line = XamlReader.Parse(lines[i]);
                    } catch(Exception e) {

                    }
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
            //tblChatWindow.Text = $"{tblChatWindow.Text}\n{userName}: {tbxChatBox.Text}";
            Client.WriteChatMessage(userName, tbxChatBox.Text);
            //tbxChatWindow.AppendText($"\n{userName}: {tbxChatBox.Text}\n");

            tbxChatBox.Clear();

            scvChatScrollbar.ScrollToBottom();
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
            updateTimer.Stop();
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
            tblChatWindow.FontSize = chatSize;

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
    }
}
