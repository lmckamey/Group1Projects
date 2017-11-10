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
using WIPProject.UserControls;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for BasicDrawingControl.xaml
    /// </summary>
    public partial class BasicDrawingControl : UserControl
    {
        Point mouseOldPosition;
        Point mouseCurrentPosition;

        private Line[] prevLines = new Line[100];
        private int numOfLines = 0;

        public BasicDrawingControl()
        {
            InitializeComponent();

            cnvDrawArea.Focus();
            //AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)cnvDrawArea_KeyDown);

            mouseOldPosition = new Point();
            mouseCurrentPosition = new Point();
        }

        private void cnvDrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            mouseCurrentPosition = e.GetPosition(cnvDrawArea);

            DrawLine(e);

            mouseOldPosition = e.GetPosition(cnvDrawArea);
        }

        private void DrawLine(MouseEventArgs e)
        {
            if (e.LeftButton.Equals(MouseButtonState.Pressed))
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
            uscBrushSize.sldBrushSize.Value += e.Delta / 40;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cnvDrawArea.Children.Clear();
        }

        private void UndoLastLine()
        {
            if (numOfLines > 0)
            {
                Line lastLine = prevLines[numOfLines - 1];

                cnvDrawArea.Children.Remove(lastLine);
                prevLines[numOfLines - 1] = null;

                --numOfLines;
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

            --numOfLines;
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
    }
}
