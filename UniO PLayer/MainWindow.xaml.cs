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
using System.Windows.Threading;
using Unosquare.FFME;

namespace UniO_PLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool playing = false;
        bool sync = false;

        public MainWindow()
        {
            Library.FFmpegDirectory = @"i:\ffmpeg_bin\bin";
            //Library.EnableWpfMultiThreadedVideo = true;

            InitializeComponent();

            var infoTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };

            infoTimer.Tick += (s, e) =>
            {
                if (sync)
                {
                    if (MediaRight.FramePosition != MediaLeft.FramePosition)
                        MediaRight.Seek(MediaLeft.Position);
                }

                if (MediaRight.FramePosition == MediaLeft.FramePosition)
                    StatusLabel.Content = "IN SYNC";
                if (MediaRight.FramePosition != MediaLeft.FramePosition)
                {
                    var diff = ((MediaRight.FramePosition - MediaLeft.FramePosition).TotalMilliseconds / 1000.0 * MediaLeft.VideoFrameRate);
                    if (Math.Abs(diff) < 1)
                    {
                        StatusLabel.Content = "IN SYNC";
                    }
                    else
                    {
                        StatusLabel.Content = "OFF BY: " + diff.ToString("0.0") + " Frame";
                    }
                }

                double leftTime = ((double)((TimeSpan)MediaLeft.FramePosition).Ticks / (double)((TimeSpan)MediaLeft.NaturalDuration).Ticks);
                double rightTime = ((double)((TimeSpan)MediaRight.FramePosition).Ticks / (double)((TimeSpan)MediaRight.NaturalDuration).Ticks);

                if (double.IsNaN(leftTime))
                    leftTime = 0;
                if (double.IsNaN(rightTime))
                    rightTime = 0;

                if(!MediaLeft.IsSeeking)
                    Left_Progress.Value = leftTime;
                if (!MediaRight.IsSeeking)
                    Right_Progress.Value = rightTime;


                Left_Time.Content = MediaLeft.FramePosition.ToString(@"mm\:ss") + " (" + Math.Round((MediaLeft.FramePosition).TotalMilliseconds / 1000.0 * MediaLeft.VideoFrameRate) + ")";
                Right_Time.Content = MediaRight.FramePosition.ToString(@"mm\:ss") + " (" + Math.Round((MediaRight.FramePosition).TotalMilliseconds / 1000.0 * MediaRight.VideoFrameRate) + ")";
            };
            infoTimer.Start();
        }

        private void MediaLeft_Drop(object sender, DragEventArgs e)
        {
            MediaLeft.Open(new Uri(((string[])e.Data.GetData(DataFormats.FileDrop))[0]));
        }
        private void MediaRight_Drop(object sender, DragEventArgs e)
        {
            MediaRight.Open(new Uri(((string[])e.Data.GetData(DataFormats.FileDrop))[0]));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.Key == Key.Space)
            {
                Console.WriteLine("Space");
                playing = !playing;
                if (playing)
                {
                    MediaLeft.Play();
                    MediaRight.Play();
                    sync = false;
                }
                else
                {
                    MediaLeft.Pause();
                    MediaRight.Pause();
                    sync = true;
                }
            }
            if (e.Key == Key.R)
            {
                playing = false;
                sync = true;
                MediaLeft.Stop();
                MediaRight.Stop();
            }
            if (e.Key == Key.Left)
            {
                playing = false;
                sync = true;
                MediaLeft.StepBackward();
                MediaRight.StepBackward();
            }
            if (e.Key == Key.Right)
            {

                playing = false;
                sync = true;
                MediaLeft.StepForward();
                MediaRight.StepForward();
            }
            */
        }

        private void Left_Play_Click(object sender, RoutedEventArgs e)
        {
            MediaLeft.Play();
            Left_Play.Visibility = Visibility.Hidden;
            Left_Pause.Visibility = Visibility.Visible;
        }

        private void Left_Pause_Click(object sender, RoutedEventArgs e)
        {
            MediaLeft.Pause();
            Left_Play.Visibility = Visibility.Visible;
            Left_Pause.Visibility = Visibility.Hidden;
        }

        private void Left_Abort_Click(object sender, RoutedEventArgs e)
        {
            MediaLeft.Stop();
        }

        private void Left_Back_Click(object sender, RoutedEventArgs e)
        {
            MediaLeft.StepBackward();
        }

        private void Left_Forward_Click(object sender, RoutedEventArgs e)
        {
            MediaLeft.StepForward();
        }

        private void Right_Play_Click(object sender, RoutedEventArgs e)
        {
            MediaRight.Play();
            Right_Play.Visibility = Visibility.Hidden;
            Right_Pause.Visibility = Visibility.Visible;
        }

        private void Right_Pause_Click(object sender, RoutedEventArgs e)
        {
            MediaRight.Pause();
            Right_Play.Visibility = Visibility.Visible;
            Right_Pause.Visibility = Visibility.Hidden;
        }

        private void Right_Abort_Click(object sender, RoutedEventArgs e)
        {
            MediaRight.Stop();
        }

        private void Right_Back_Click(object sender, RoutedEventArgs e)
        {
            MediaRight.StepBackward();
        }

        private void Right_Forward_Click(object sender, RoutedEventArgs e)
        {
            MediaRight.StepForward();
        }


        bool Left_scrub = false;

        private void Left_Progress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Left_Progress.Value = SetProgressBarValue(e.GetPosition(Left_Progress).X, Left_Progress);
            TimeSpan newTime = new TimeSpan((long)((double)(((TimeSpan)MediaLeft.NaturalDuration).Ticks) * Left_Progress.Value));

            MediaLeft.ScrubbingEnabled = true;
            MediaLeft.Position = newTime;
            MediaLeft.ScrubbingEnabled = false;
            Left_scrub = true;
        }

        private void Left_Progress_MouseMove(object sender, MouseEventArgs e)
        {
            if (Left_scrub)
            {
                Left_Progress.Value = SetProgressBarValue(e.GetPosition(Left_Progress).X, Left_Progress);
                TimeSpan newTime = new TimeSpan((long)((double)(((TimeSpan)MediaLeft.NaturalDuration).Ticks) * Left_Progress.Value));

                MediaLeft.ScrubbingEnabled = true;
                MediaLeft.Position = newTime;
                MediaLeft.ScrubbingEnabled = false;
            }
        }

        private void Left_Progress_MouseLeave(object sender, MouseEventArgs e)
        {
            Left_scrub = false;
        }

        private void Left_Progress_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Left_scrub = false;
        }

        private double SetProgressBarValue(double mousePosition, ProgressBar bar)
        {
            double ratio = mousePosition / bar.ActualWidth;
            double ProgressBarValue = ratio * bar.Maximum;
            return ProgressBarValue;
        }

        bool Right_scrub = false;

        private void Right_Progress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Right_Progress.Value = SetProgressBarValue(e.GetPosition(Right_Progress).X, Left_Progress);
            TimeSpan newTime = new TimeSpan((long)((double)(((TimeSpan)MediaRight.NaturalDuration).Ticks) * Right_Progress.Value));

            MediaRight.ScrubbingEnabled = true;
            MediaRight.Position = newTime;
            MediaRight.ScrubbingEnabled = false;
            Right_scrub = true;
        }

        private void Right_Progress_MouseMove(object sender, MouseEventArgs e)
        {
            if (Right_scrub)
            {
                Right_Progress.Value = SetProgressBarValue(e.GetPosition(Right_Progress).X, Left_Progress);
                TimeSpan newTime = new TimeSpan((long)((double)(((TimeSpan)MediaRight.NaturalDuration).Ticks) * Right_Progress.Value));

                MediaRight.ScrubbingEnabled = true;
                MediaRight.Position = newTime;
                MediaRight.ScrubbingEnabled = false;
            }
        }

        private void Right_Progress_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Right_scrub = false;
        }

        private void Right_Progress_MouseLeave(object sender, MouseEventArgs e)
        {
            Right_scrub = false;
        }
    }
}
