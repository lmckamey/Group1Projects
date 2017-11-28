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
using WIPProject.Enums;
using WIPProject.Models;
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

        private List<int> lineStrokes;
        private static int maxNumberOfUndos = 5000;
        private int currentLineCount = 0;

        private NextAction nextAction = NextAction.DRAW;

        private Brush originalButtonColor;

        public BasicDrawingControl()
        {
            InitializeComponent();

            lineStrokes = new List<int>();

            uscColorPicker.bdc = this;
            uscColorPicker.drawControls = grdDrawControls;
            uscColorPicker.grid = baseGrid;
            baseGrid.Children.Add(uscColorPicker.qcac);
            double height = uscColorPicker.qcac.ActualHeight;
            uscColorPicker.qcac.Margin = new Thickness(0, 257 - height - 23, 0, 0);

            //cnvDrawArea.Focus();

            mouseOldPosition = new Point();
            mouseCurrentPosition = new Point();

            originalButtonColor = btnEraser.Background.Clone();

            //EntryClass.Start();
        }

        private void cnvDrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            mouseCurrentPosition = e.GetPosition(cnvDrawArea);

            DoNextCommand(e, true);

            mouseOldPosition = e.GetPosition(cnvDrawArea);
        }

        private void DoNextCommand(MouseEventArgs e, bool move)
        {
            switch(nextAction.ToString())
            {
                case "DRAW":
                    DrawLine(e);
                    break;
                case "ERASE":
                    if (e.LeftButton == MouseButtonState.Pressed)
                        EraseLine();
                break;
                case "FILL":
                    if (!move)
                        FillCanvas();
                break;
                default:
                break;
            }
        }

        private void FillCanvas()
        {
            cnvDrawArea.Background = uscColorPicker.elpCurrentColor.Fill.Clone();
            nextAction = NextAction.DRAW;
            btnBucket.Background = originalButtonColor.Clone();
        }

        private void EraseLine()
        {
            int brushSize = (int)(uscBrushSize.sldBrushSize.Value / 2);

            Point topLeft = new Point(mouseCurrentPosition.X - brushSize, 
                mouseCurrentPosition.Y - brushSize);
            Point bottomRight = new Point(mouseCurrentPosition.X + brushSize,
                mouseCurrentPosition.Y + brushSize);
            Rect r = new Rect(topLeft, bottomRight);

            int count = cnvDrawArea.Children.Count;
            for (int i = 0; i < count; ++i)
            {
                if (i < cnvDrawArea.Children.Count)
                {
                    UIElement child = cnvDrawArea.Children[i];
                    if (child.GetType().Equals(typeof(Line)))
                    {
                        if (r.IntersectsWith(
                            ((Line)child).RenderedGeometry.Bounds))
                        {
                            cnvDrawArea.Children.RemoveAt(i);
                            --i;
                        }
                    }
                }
            }
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

                l.MouseMove += LineMove;
                l.MouseDown += LineDown;

                cnvDrawArea.Children.Add(l);

                ++currentLineCount;
            }
        }

        private void LineMove(object sender, MouseEventArgs e)
        {
            if (nextAction == NextAction.ERASE && e.LeftButton == MouseButtonState.Pressed)
            {
                cnvDrawArea.Children.Remove((Line)sender);
            }
        }

        private void LineDown(object sender, MouseButtonEventArgs e)
        {
            if (nextAction == NextAction.ERASE)
            {
                cnvDrawArea.Children.Remove((Line)sender);
            }
        }

        private void cnvDrawArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            uscBrushSize.sldBrushSize.Value += e.Delta / 120;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cnvDrawArea.Children.Clear();

            currentLineCount = 0;
        }

        private void UndoLastLine()
        {
            if (lineStrokes.Count > 0)
            {
                int numberOfUndos = lineStrokes.ElementAt(lineStrokes.Count - 1);
                lineStrokes.RemoveAt(lineStrokes.Count - 1);

                for (int i = 0; i < numberOfUndos; ++i)
                {
                    cnvDrawArea.Children.RemoveAt(cnvDrawArea.Children.Count - 1);
                }
            }
        }

        private void IncrementCurrentStroke()
        {
            if (currentLineCount > 0)
            {
                if (lineStrokes.Count >= maxNumberOfUndos)
                {
                    RemoveFirstStroke();
                }

                lineStrokes.Add(currentLineCount);
                currentLineCount = 0;
            }
        }

        private void RemoveFirstStroke()
        {
            if (lineStrokes.Count > 0)
                lineStrokes.RemoveAt(0);
        }

        private void cnvDrawArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void cnvDrawArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ignoreNextLines = false;

            if (e.LeftButton.Equals(MouseButtonState.Released))
            {
                IncrementCurrentStroke();
            }
        }

        private void cnvDrawArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DoNextCommand(e, false);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                UndoLastLine();
            }
        }

        private void btnBucket_Click(object sender, RoutedEventArgs e)
        {
            if (nextAction != NextAction.FILL)
            {
                nextAction = NextAction.FILL;
                btnBucket.Background = new SolidColorBrush(Colors.SkyBlue);
            }
            else
            {
                nextAction = NextAction.DRAW;
                btnBucket.Background = originalButtonColor.Clone();
            }
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            if (nextAction == NextAction.DRAW)
            {
                nextAction = NextAction.ERASE;
                btnEraser.Background = new SolidColorBrush(Colors.SkyBlue);
            }
            else
            {
                nextAction = NextAction.DRAW;
                btnEraser.Background = originalButtonColor.Clone();
            }
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            UndoLastLine();
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
