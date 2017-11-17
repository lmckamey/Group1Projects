using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using WIPProject.UserControls;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for BasicDrawingControl.xaml
    /// </summary>
    public partial class BasicDrawingControl : UserControl
    {
        public bool ignoreNextLines = false;

        public Point mouseOldPosition;
        public Point mouseCurrentPosition;

        private Line[] prevLines = new Line[1000];
        private int numOfLines = 0;

        private int removedLinesPerUndo = 10;

        public BasicDrawingControl()
        {
            InitializeComponent();

            uscColorPicker.bdc = this;
            uscColorPicker.drawControls = grdDrawControls;
            uscColorPicker.grid = baseGrid;
            baseGrid.Children.Add(uscColorPicker.qcac);
            double height = uscColorPicker.qcac.ActualHeight;
            uscColorPicker.qcac.Margin = new Thickness(0, 257 - height - 23, 0, 0);

            cnvDrawArea.Focus();
            //AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)cnvDrawArea_KeyDown);

            mouseOldPosition = new Point();
            mouseCurrentPosition = new Point();

            //EntryClass.Start();
        }

        private void cnvDrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            mouseCurrentPosition = e.GetPosition(cnvDrawArea);

            DrawLine(e);

            mouseOldPosition = e.GetPosition(cnvDrawArea);
        }

        private void DrawLine(MouseEventArgs e)
        {
            if (e.LeftButton.Equals(MouseButtonState.Pressed) && !ignoreNextLines)
            {
                Line l = new Line();

                l.X1 = mouseOldPosition.X;
                l.Y1 = mouseOldPosition.Y;
                l.X2 = mouseCurrentPosition.X;
                l.Y2 = mouseCurrentPosition.Y;

                l.StrokeThickness = uscBrushSize.sldBrushSize.Value;
                l.Stroke = uscColorPicker.elpCurrentColor.Fill.Clone();
                l.Fill = l.Stroke;

                l.StrokeEndLineCap = PenLineCap.Round;
                l.StrokeStartLineCap = PenLineCap.Round;
                l.StrokeLineJoin = PenLineJoin.Round;

                cnvDrawArea.Children.Add(l);

                if (numOfLines >= prevLines.Length)
                    RemoveFirstLine();

                prevLines[numOfLines++] = l;
            }
        }

        private void cnvDrawArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            uscBrushSize.sldBrushSize.Value += e.Delta / 120;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cnvDrawArea.Children.Clear();

            //var children = cnvDrawArea.Children;
            //int num = children.Count;
            //for (int i = 1; i < num; ++i)
            //{
            //    if (true)
            //    {
            //        var child = children[i];
            //        cnvDrawArea.Children.Remove(child);
            //        --i;
            //        --num;
            //    }
            //}
        }

        private void UndoLastLine()
        {
            if (numOfLines > 0)
            {
                for (int i = 0; i < removedLinesPerUndo; ++i)
                {
                    if (numOfLines > 0)
                    {
                        Line lastLine = prevLines[numOfLines - 1];

                        //int index = prevLines.Length - numOfLines;
                        cnvDrawArea.Children.RemoveAt(cnvDrawArea.Children.Count - 1);
                        prevLines[numOfLines - 1] = null;

                        --numOfLines;
                    }
                }
            }
        }

        private void RemoveFirstLine()
        {
            prevLines[0] = null;

            for (int i = 0; i < prevLines.Length - 1; ++i)
            {
                Line top = prevLines[i + 1];
                Line bottom = prevLines[i];

                bottom = top;
            }
            prevLines[prevLines.Length - 1] = null;

            numOfLines = prevLines.Length - 1;
        }

        private void cnvDrawArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            if (true)
            {
                UndoLastLine();
                cnvDrawArea.Background = new SolidColorBrush(Colors.Black);
            }
        }

        private void cnvDrawArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ignoreNextLines = false;
        }

        private void cnvDrawArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DrawLine(e);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                UndoLastLine();
            }
        }
    }


    //public class app : Application
    //{
    //    Window mainWindow;

    //    protected override void OnStartup(StartupEventArgs e)
    //    {
    //        base.OnStartup(e);
    //        CreateAndShowMainWindow();
    //    }
    //    private void CreateAndShowMainWindow()
    //    {
    //        // Create the application's main window
    //        mainWindow = new Window();
    //        mainWindow.Title = "Writeable Bitmap";
    //        mainWindow.Height = 200;
    //        mainWindow.Width = 200;

    //        // Define the Image element
    //        _random.Stretch = Stretch.None;
    //        _random.Margin = new Thickness(20);

    //        // Define a StackPanel to host Controls
    //        StackPanel myStackPanel = new StackPanel();
    //        myStackPanel.Orientation = Orientation.Vertical;
    //        myStackPanel.Height = 200;
    //        myStackPanel.VerticalAlignment = VerticalAlignment.Top;
    //        myStackPanel.HorizontalAlignment = HorizontalAlignment.Center;

    //        // Add the Image to the parent StackPanel
    //        myStackPanel.Children.Add(_random);

    //        // Add the StackPanel as the Content of the Parent Window Object
    //        mainWindow.Content = myStackPanel;
    //        mainWindow.Show();

    //        // DispatcherTimer setup
    //        // The DispatcherTimer will be used to update _random every
    //        //    second with a new random set of colors.
    //        DispatcherTimer dispatcherTimer = new DispatcherTimer();
    //        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
    //        dispatcherTimer.IsEnabled = true;
    //        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
    //        dispatcherTimer.Start();
    //    }
    //    //  System.Windows.Threading.DispatcherTimer.Tick handler
    //    //
    //    //  Updates the Image element with new random colors
    //    private void dispatcherTimer_Tick(object sender, EventArgs e)
    //    {
    //        //Update the color array with new random colors
    //        Random value = new Random();
    //        value.NextBytes(_colorArray);

    //        //Update writeable bitmap with the colorArray to the image.
    //        _wb.WritePixels(_rect, _colorArray, _stride, 0);

    //        //Set the Image source.
    //        _random.Source = _wb;
    //    }

    //    private Image _random = new Image();
    //    // Create the writeable bitmap will be used to write and update.
    //    private static WriteableBitmap _wb =
    //        new WriteableBitmap(100, 100, 96, 96, PixelFormats.Bgra32, null);
    //    // Define the rectangle of the writeable image we will modify. 
    //    // The size is that of the writeable bitmap.
    //    private static Int32Rect _rect = new Int32Rect(0, 0, _wb.PixelWidth, _wb.PixelHeight);
    //    // Calculate the number of bytes per pixel. 
    //    private static int _bytesPerPixel = (_wb.Format.BitsPerPixel + 7) / 8;
    //    // Stride is bytes per pixel times the number of pixels.
    //    // Stride is the byte width of a single rectangle row.
    //    private static int _stride = _wb.PixelWidth * _bytesPerPixel;

    //    // Create a byte array for a the entire size of bitmap.
    //    private static int _arraySize = _stride * _wb.PixelHeight;
    //    private static byte[] _colorArray = new byte[_arraySize];

    //}

    //// Define a static entry class
    //internal static class EntryClass
    //{
    //    [System.STAThread()]
    //    public static void Start()
    //    {
    //        app app = new app();
    //        app.Run();
    //    }
    //}
}
