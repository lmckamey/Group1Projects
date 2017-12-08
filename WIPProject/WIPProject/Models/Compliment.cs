using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WIPProject.Enums;

namespace WIPProject.Models
{
    public class Compliment
    {
        public Border border;
        public Label message;

        private static Random random;

        public Compliment()
        {
            border = new Border();
            message = new Label();
            random = new Random();

            message.FontFamily = new FontFamily("Maiandra GD");
            message.FontSize = 12;
            message.Foreground = new SolidColorBrush((Color)
                ColorConverter.ConvertFromString("#FFCEFFFF"));

            border.Child = message;

            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(6);
            border.HorizontalAlignment = HorizontalAlignment.Left;
            border.VerticalAlignment = VerticalAlignment.Top;
        }

        public static void CreateNewCompliment(ComplimentType type, Canvas area)
        {
            Compliment c = new Compliment();

            AddComplimentColor(c, type);

            SetRandomPosition(c, area);

            StartAnimationTimer(c, 2, 1);
        }

        private static void AddComplimentColor(Compliment c, ComplimentType type)
        {
            Color yellow = (Color)ColorConverter.ConvertFromString("#CCACAC0A");
            Color green = (Color)ColorConverter.ConvertFromString("#CC0AA2B1");
            Color blue = (Color)ColorConverter.ConvertFromString("#CC0AB138");
            Color pink = (Color)ColorConverter.ConvertFromString("#CCDD75E8");

            switch (type)
            {
                case ComplimentType.WOW:
                    c.border.Background = new SolidColorBrush(yellow);
                    c.message.Content = "WOW!";
                    break;
                case ComplimentType.NICE:
                    c.border.Background = new SolidColorBrush(blue);
                    c.message.Content = "Nice!";
                    break;
                case ComplimentType.COOL:
                    c.border.Background = new SolidColorBrush(green);
                    c.message.Content = "Cool!";
                    break;
                case ComplimentType.THANKS:
                    c.border.Background = new SolidColorBrush(pink);
                    c.message.Content = "Thanks!";
                    break;
                default:
                    c.border.Background = new SolidColorBrush(yellow);
                    c.message.Content = "WOW!";
                    break;
            }
        }

        private static void SetRandomPosition(Compliment c, Canvas area)
        {
            //int offsetX = (int)c.message.ActualWidth;
            //int offsetY = (int)c.message.ActualHeight;
            int offsetX = 50;
            int offsetY = 25;
            int width = (int)area.ActualWidth;
            int height = (int)area.ActualHeight;

            int x = random.Next(0, width - offsetX);
            int y = random.Next(0, height - offsetY);

            area.Children.Add(c.border);

            c.border.Margin = new Thickness(x, y, 0, 0);
        }

        private static void StartAnimationTimer(Compliment c, double startTime = 2, double duration = 1)
        {
            var animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(startTime),
                Duration = TimeSpan.FromSeconds(duration),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => c.border.Opacity = 0;

            c.border.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}
