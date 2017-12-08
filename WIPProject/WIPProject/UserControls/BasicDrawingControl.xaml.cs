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
using WIPProject.Networking;
using WIPProject.UserControls;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for BasicDrawingControl.xaml
    /// </summary>
    public partial class BasicDrawingControl : UserControl
    {
        public DrawingPage drawingPage;
        public Line[] dirtyLines = new Line[5000];
        public int numOfDirtyLines = 0;

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

            var color = ((Color)uscColorPicker.elpCurrentColor.Fill.GetValue(SolidColorBrush.ColorProperty));
            Client.WriteFillMessage(color.ToString());
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
                            Client.WriteEraseMessage(i);
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

                cnvDrawArea.Children.Add(l);

                //dirtyLines.Add(l);
                AddDirtyLine(l);
                Client.WriteDrawMessage(l);

                ++currentLineCount;
            }
        }

        private void AddDirtyLine(Line l)
        {
            if (numOfDirtyLines < dirtyLines.Length)
            {
                dirtyLines[numOfDirtyLines] = l;
                numOfDirtyLines += 1;
            }
        }

        public void ClearDirtyLines()
        {
            for (int i = 0; i < dirtyLines.Length; ++i)
            {
                if (dirtyLines[i] != null)
                    dirtyLines[i] = null;
            }

            if (numOfDirtyLines > 0)
                numOfDirtyLines = 0;
        }

        private void cnvDrawArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            uscBrushSize.sldBrushSize.Value += e.Delta / 120;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cnvDrawArea.Children.Clear();

            currentLineCount = 0;
            Client.WriteClearMessage();
        }

        private void UndoLastLine()
        {
            if (lineStrokes.Count > 0)
            {
                int numberOfUndos = lineStrokes.ElementAt(lineStrokes.Count - 1);
                lineStrokes.RemoveAt(lineStrokes.Count - 1);

                for (int i = 0; i < numberOfUndos; ++i)
                {
                    if (numOfDirtyLines > 0)
                    {
                        dirtyLines[--numOfDirtyLines] = null;
                    }
                    if (cnvDrawArea.Children.Count > 0)
                    {
                        cnvDrawArea.Children.RemoveAt(cnvDrawArea.Children.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                Client.WriteUndoMessage(numberOfUndos);
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

        private void cnvDrawArea_MouseLeave(object sender, MouseEventArgs e)
        {
            IncrementCurrentStroke();
        }
    }
}
