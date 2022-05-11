﻿using System;
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
        DispatcherTimer timer = new DispatcherTimer(); // for timer
        TimeSpan _time; // time duration

        Random rnd = new Random(); // for random number generator
        int minInt = 0; // minimum value for random number generator
        int maxInt = 12; // maximum value for random number generator

        bool isStart = false; // boolean to check if state is at start
        List<int> pastResult = new List<int>(); // to store the previous result and current result

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 1); // timer will "tick" every 1 second
            timer.Tick += Timer_Count; // Process time every 1 second
            _time = TimeSpan.FromSeconds(60); // Set time to 60 seconds
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            isStart = false; // if enter button is clicked, state is no longer at start hence set isStart to false
            GenerateQuestions(); // generate a random new set of question
        }

        private void Timer_Count(object sender, object e)
        {
            _time = _time.Add(TimeSpan.FromSeconds(-1)); // decrement the time by every second
            timerText.Text = _time.ToString("c"); // set timer text to current time

            if (timerText.Text == "00:00:00") // if timer hits zero
            {
                timer.Stop(); // stop the timer
                EndGame(); // end of game
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //Reset all information to initial state

            _time = TimeSpan.FromSeconds(60); // reset time to 60 seconds
            timerText.Text = _time.ToString("c"); // reset time to current 60 seconds
            scoreText.Text = "0"; // reset score to zero
            userInputText.Text = ""; // reset user input to null
            isStart = true; // reset state to start
            startButton.IsEnabled = false; // disable the startButton as long as timer is running
            enterButton.IsEnabled = true; // enable the enterButton as long as timer is running
            optCombo.IsEnabled = false; // disable the choosing of 4 operations
            timer.Start(); // start the timer
            GenerateQuestions(); // generate a random new set of question
        }

        // Grid itself handles the KeyDown event, and sets the flag to stop the event from bubbling further
        // So need to handle the PreviewKeyDown event instead. This gives us a chance to respond to the key down event before the Grid does.
        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                EnterButton_Click(sender, e);
            }
        }

        private void GenerateQuestions()
        {
            int num1 = RandGenerate()[0]; // generate the first number
            int num2 = RandGenerate()[1]; // generate the second number

            string chosenItems = optCombo.SelectionBoxItem.ToString(); // check which of the 4 operations are chosen

            if (chosenItems == "Addition") // if addition does addition
            {
                AddFunc(num1, num2);
            }
            else if (chosenItems == "Subtraction") // if subtraction does subtraction
            {
                SubtractFunc(num1, num2);
            }
            else if (chosenItems == "Multiplication") // if multiplication does multiplication
            {
                MultiplyFunc(num1, num2);
            }
            else // if division does division
            {
                DivideFunc(num1, num2);
            }
        }

        private List<int> RandGenerate() // generate and return 2 random numbers
        {
            var retList = new List<int>();
            // need to +1 because max is exclusive
            int num1 = rnd.Next(minInt, maxInt + 1);  // generates a number between 0 and 12 inclusive
            int num2 = rnd.Next(minInt, maxInt + 1);  // generates a number between 0 and 12 inclusive

            retList.Add(num1);
            retList.Add(num2);

            return retList;
        }

        private void AddFunc(int num1, int num2) // addition function
        {
            int result = num1 + num2; // add the numbers
            questionText.Text = num1.ToString() + " + " + num2.ToString(); // set the question
        }

        private void SubtractFunc(int num1, int num2) // subtraction function
        {
            int result = num1 - num2; // subtract the numbers
            questionText.Text = num1.ToString() + " - " + num2.ToString(); // set the question
        }

        private void MultiplyFunc(int num1, int num2) // multiplication function
        {
            int result = num1 * num2; // multiply the numbers
            questionText.Text = num1.ToString() + " x " + num2.ToString(); // set the question

            if (isStart) // this is necessary because when start button is clicked, result is tabulated but dont have any user input
            {
                pastResult.Add(result); // add result to list (list will have one result)
            }
            else // if it is not start state ie enter button is clicked (another question will be generated at the same time)
            {
                if (userInputText.Text.ToString() != "") // if user input is not null
                {
                    pastResult.Add(result); // add result to list (list will have two results)
                    int userInput = int.Parse(userInputText.Text); // get user input as integer
                    userInputText.Text = ""; // erase user input 
                    if (pastResult[0] == userInput) // check if user input is the same as the first result in the list 
                    {
                        scoreText.Text = (int.Parse(scoreText.Text) + 1).ToString(); // same result increment score by 1
                    }
                    else
                    {
                        scoreText.Text = (int.Parse(scoreText.Text) - 1).ToString(); // different result decrement score by 1
                    }
                    pastResult.RemoveAt(0); // remove the first result from list
                }
            }
        }

        private void DivideFunc(int num1, int num2) // division function
        {
            double result = Math.Round((double)num1 / num2, 2); // divide the numbers
            questionText.Text = num1.ToString() + " / " + num2.ToString(); // set the question
        }

        private void EndGame() // end of game
        {
            MsgBoxEndGame(); // dialog box to tell user it is end of game
            startButton.IsEnabled = true; // enable the start button
            enterButton.IsEnabled = false; // disable the enter button
            optCombo.IsEnabled = true; // enable the choosing of 4 operations
        }

        private void MsgBoxEndGame() // dialog box for end of game
        {
            string messageBoxText = "Time is up"; // set dialog box text message
            string caption = "Game Over"; // set dialog box title
            MessageBoxButton button = MessageBoxButton.OK; // ok button

            MessageBox.Show(messageBoxText, caption, button); // show the dialog box
        }
    }
}
