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

namespace FlashCardGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        TimeSpan _time;

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 1); // timer will "tick" every 1 second
            timer.Tick += Timer_Count; // Process time every 1 second
            _time = TimeSpan.FromSeconds(10);
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Timer_Count(object sender, object e)
        {
            _time = _time.Add(TimeSpan.FromSeconds(-1));
            timerText.Text = _time.ToString("c");

            if (timerText.Text == "00:00:00")
            {
                timer.Stop();
                EndGame();
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //Reset all information to initial state

            _time = TimeSpan.FromSeconds(10);
            timerText.Text = _time.ToString("c");
            startButton.IsEnabled = false;
            timer.Start();
            GenerateQuestions();
        }

        private void GenerateQuestions()
        {

        }

        private void EndGame()
        {
            MsgBox();
            timerText.Text = _time.ToString("c"); ;
            startButton.IsEnabled = true;
        }

        private void MsgBox()
        {
            string messageBoxText = "Time is up";
            string caption = "Game Over";
            MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show(messageBoxText, caption, button);
        }
    }
}
