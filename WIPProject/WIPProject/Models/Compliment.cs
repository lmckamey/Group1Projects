using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WIPProject.Delegates;

namespace WIPProject.Models
{
    public class Compliment
    {
        private DispatcherTimer dispatcherTimer;
        //public Timer Timer { get; set; }
        private Random random;
        public ComplimentDestroyHandler destroyer;
        public ComplimentDestroyHandler ComplimentDestroyHandler
        { get { return destroyer; }  set { destroyer = value; } }
        public Border Border { get; set; }
        //public string UserName { get; set; }
        //public string Message { get; set; }
        public int MaxHorizontal { get; set; }
        public int MaxVertical { get; set; }
        public int FadeDuration { get; private set; }
        public int FadeOffset { get; private set; }

        public float delta = 0;
        public int currentMillis = 0;

        private void FadeTick(object sender, EventArgs e)
        {
            //SolidColorBrush scb = (SolidColorBrush) TextBlock.Background;
            //SolidColorBrush fore = (SolidColorBrush) TextBlock.Foreground;
            
            if (currentMillis >= FadeOffset)
            {
                (Border).Opacity -= delta;
            }

            //TextBlock.Background = scb;

            ++currentMillis;
            if (currentMillis >= FadeDuration)
            {
                dispatcherTimer.Stop();
                //Timer.Stop();
                //Timer.Dispose();
                destroyer?.Invoke(Border);
            }
        }

        public void StartTimer()
        {
            currentMillis = 0;

            SetRandomLocation();

            if (FadeDuration < 0)
                FadeDuration = 0;

            dispatcherTimer?.Start();
            //Timer.Start();
        }

        private void SetRandomLocation()
        {
            int padding = 5;
            int width = (int)Border.Width;
            int height = (int)Border.Height;

            int x = random.Next(padding, (MaxHorizontal + 1) - width - padding);
            int y = random.Next(padding, (MaxVertical + 1) - height - padding);

            (Border).Margin = new Thickness(x, y, 0, 0);
        }

        public void StopTimer()
        {
            dispatcherTimer.Stop();
            //Timer.Stop();
        }

        public Border CreateTextBlock(string username, string message, Color backGroundColor)
        {
            TextBlock tb = new TextBlock();
            Border = new Border();
            Border.BorderThickness = new Thickness(1);
            Border.CornerRadius = new CornerRadius(8);
            Border.BorderBrush = new SolidColorBrush(Colors.Black);
            Border.Child = tb;

            tb.Text = $"{username} says:\n{message}";
            //tb.Background = new SolidColorBrush(backGroundColor);
            tb.Foreground = new SolidColorBrush(Colors.Black);
            tb.Width = 85;
            tb.Height = 35;
            tb.TextWrapping = TextWrapping.NoWrap;
            tb.FontFamily = new FontFamily("Maiandra GD");
            tb.Padding = new Thickness(1);

            Border.Width = tb.Width;
            Border.Height = tb.Height;
            Border.Background = new SolidColorBrush(backGroundColor);

            return Border;
        }

        public void CreateTimer(int fadeTime, int fadeOffset, bool start = false)
        {
            random = new Random();
            //Timer = new Timer(1);
            //Timer.Interval = 1;
            //Timer.AutoReset = true;
            //Timer.Elapsed += FadeTick;
            //Timer.Elapsed += new ElapsedEventHandler(FadeTick);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += FadeTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);


            FadeOffset = fadeOffset;
            FadeDuration = fadeTime + FadeOffset;

            delta = 1.0f / (float)fadeTime;

            if (start)
                StartTimer();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
