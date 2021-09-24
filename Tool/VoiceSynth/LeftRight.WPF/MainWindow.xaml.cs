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

namespace LeftRight.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Media.SoundPlayer playerLeft;
        System.Media.SoundPlayer playerRight;

        System.IO.StreamWriter writer = null;
        System.IO.StreamReader reader;

        DateTime? dateTime = null;
        DateTime playTimer = new DateTime();

        public MainWindow()
        {
            InitializeComponent();

            playerLeft = new System.Media.SoundPlayer("left.wav");
            playerRight = new System.Media.SoundPlayer("right.wav");
        }

        private async void Button_Click_left(object sender, RoutedEventArgs e)
        {
            if(writer is null) writer = new System.IO.StreamWriter("timing.txt", false);

            playerLeft.Stop();
            playerLeft.Play();

            dateTime = dateTime ?? DateTime.Now;
            var past = DateTime.Now - dateTime.Value;

            await writer.WriteLineAsync(past.TotalMilliseconds.ToString());
            await writer.WriteLineAsync("left.wav");
        }

        private async void Button_Click_right(object sender, RoutedEventArgs e)
        {
            if (writer is null) writer = new System.IO.StreamWriter("timing.txt", false);

            playerRight.Stop();
            playerRight.Play();

            dateTime = dateTime ?? DateTime.Now;
            var past = DateTime.Now - dateTime.Value;

            await writer.WriteLineAsync(past.TotalMilliseconds.ToString());
            await writer.WriteLineAsync("right.wav");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            writer?.Close();
            writer = null;

            base.OnClosing(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            writer?.Close();
            writer = null;

            string text;
            using (reader = new System.IO.StreamReader("timing.txt"))
            {
                text = reader.ReadToEnd();
            }
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);

            var sReader = new System.IO.StringReader(text);
            double nextMS = double.Parse(sReader.ReadLine().Trim());
            string nextWave = sReader.ReadLine().Trim();

            playTimer = DateTime.Now;

            timer.Tick += new EventHandler((s, e) =>
            {
                if((DateTime.Now-playTimer).TotalMilliseconds > nextMS)
                {
                    if (nextWave.StartsWith("left"))
                    {
                        playerLeft.Stop();
                        playerLeft.Play();
                    }
                    else
                    {
                        playerRight.Stop();
                        playerRight.Play();
                    }
                    var next = sReader.ReadLine();
                    if (next is null)
                    {
                        timer.Stop();
                        reader.Close();
                        return;
                    }
                    nextMS = double.Parse(next.Trim());
                    nextWave = sReader.ReadLine().Trim();
                }
            });
            timer.Start();

        }
    }
}
