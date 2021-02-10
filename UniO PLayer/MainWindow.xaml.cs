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
                    SyncLabel.Content = "IN SYNC";
                if (MediaRight.FramePosition != MediaLeft.FramePosition)
                {
                    var diff = ((MediaRight.FramePosition - MediaLeft.FramePosition).Milliseconds / 1000.0 * MediaLeft.VideoFrameRate);
                    if(diff < 1)
                    {
                        SyncLabel.Content = "IN SYNC";
                    }
                    else
                    {
                        SyncLabel.Content = "OFF BY: " + diff.ToString("0.0") + " Frame";
                    }
                }

                DLabel1.Content = MediaLeft.FramePosition;
                DLabel2.Content = MediaRight.FramePosition;
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
            if (e.Key == Key.Space)
            {
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
        }
    }
}
