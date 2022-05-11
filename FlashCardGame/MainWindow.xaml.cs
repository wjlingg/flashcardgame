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

        Random rnd = new Random();
        int minInt = 0;
        int maxInt = 12;

        bool isStart = false;
        List<int> pastResult = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 1); // timer will "tick" every 1 second
            timer.Tick += Timer_Count; // Process time every 1 second
            _time = TimeSpan.FromSeconds(60);
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            isStart = false;
            GenerateQuestions();
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

            _time = TimeSpan.FromSeconds(60);
            timerText.Text = _time.ToString("c");
            scoreText.Text = "0";
            userInputText.Text = "";
            isStart = true;
            startButton.IsEnabled = false;
            timer.Start();
            GenerateQuestions();
        }

        private void GenerateQuestions()
        {
            int num1 = RandGenerate()[0];
            int num2 = RandGenerate()[1];

            string chosenItems = optCombo.SelectionBoxItem.ToString();

            if (chosenItems == "Addition")
            {
                AddFunc(num1, num2);
            }
            else if (chosenItems == "Subtraction")
            {
                SubtractFunc(num1, num2);
            }
            else if (chosenItems == "Multiplication")
            {
                MultiplyFunc(num1, num2);
            }
            else
            {
                DivideFunc(num1, num2);
            }
        }

        private List<int> RandGenerate()
        {
            var retList = new List<int>();
            int num1 = rnd.Next(minInt, maxInt + 1);  // generates a number between 0 and 12 inclusive
            int num2 = rnd.Next(minInt, maxInt + 1);  // generates a number between 0 and 12 inclusive

            retList.Add(num1);
            retList.Add(num2);

            return retList;
        }

        private void AddFunc(int num1, int num2)
        {
            int result = num1 + num2;
            questionText.Text = num1.ToString() + " + " + num2.ToString() + " =";
        }

        private void SubtractFunc(int num1, int num2)
        {
            int result = num1 - num2;
            questionText.Text = num1.ToString() + " - " + num2.ToString() + " =";
        }

        private void MultiplyFunc(int num1, int num2)
        {
            int result = num1 * num2;
            questionText.Text = num1.ToString() + " x " + num2.ToString();

            if (isStart)
            {
                pastResult.Add(result);
            }
            else
            {
                if (userInputText.Text.ToString() != "")
                {
                    pastResult.Add(result);
                    int userInput = int.Parse(userInputText.Text);
                    if (pastResult[0] == userInput)
                    {
                        scoreText.Text = (int.Parse(scoreText.Text) + 1).ToString();
                        //remainingText.Text = userInput + " " + pastResult[0];
                    }
                    else
                    {
                        //remainingText.Text = userInput + " " + pastResult[0];
                        scoreText.Text = (int.Parse(scoreText.Text) - 1).ToString();
                    }
                    pastResult.RemoveAt(0);
                }
            }
        }

        private void DivideFunc(int num1, int num2)
        {
            double result = Math.Round((double)num1 / num2, 2);
            questionText.Text = num1.ToString() + " / " + num2.ToString() + " =";
        }

        private void EndGame()
        {
            MsgBoxEndGame();
            timerText.Text = _time.ToString("c"); ;
            startButton.IsEnabled = true;
        }

        private void MsgBoxEndGame()
        {
            string messageBoxText = "Time is up";
            string caption = "Game Over";
            MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show(messageBoxText, caption, button);
        }
    }
}
