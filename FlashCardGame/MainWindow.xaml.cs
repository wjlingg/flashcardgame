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
        DispatcherTimer timer = new DispatcherTimer(); // for timer
        TimeSpan _time; // time duration

        Random rnd = new Random(); // for random number generator
        static int maxInt = 12; // maximum value for random number generator
        static int numCols = (maxInt+1)*(maxInt+1); // number of columns for 1d array
        int[] array = new int[numCols]; // to track number combinations
        List<List<int>> subTrackCombi = new List<List<int>>(); // all possible combi, 169 of them

        bool isStart = false; // boolean to check if state is at start
        List<double> pastResult = new List<double>(); // to store the previous result and current result

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 1); // timer will "tick" every 1 second
            timer.Tick += Timer_Count; // Process time every 1 second
            _time = TimeSpan.FromSeconds(60); // Set time to 60 seconds

            // this is to generate [0,0], [0,1], ... , [12,11], [12,12]
            for (int i = 0; i < maxInt+1; i++)
            {
                for (int j = 0; j < maxInt+1; j++)
                {
                    subTrackCombi.Add(new List<int> { i, j });
                }
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e) // event called when enter buttion is clicked
        {
            isStart = false; // if enter button is clicked, state is no longer at start hence set isStart to false
            GenerateQuestions(); // generate a random new set of question
        }

        private void Timer_Count(object sender, object e) // processing of time
        {
            _time = _time.Add(TimeSpan.FromSeconds(-1)); // decrement the time by every second
            timerText.Text = _time.ToString("c"); // set timer text to current time

            if (timerText.Text == "00:00:00") // if timer hits zero
            {
                timer.Stop(); // stop the timer
                EndGame(); // end of game
                MsgBoxEndGame(); // dialog box to tell user it is end of game due to time is up
                MsgBoxSuccess(); // dialog box to show percentage of score
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) // event called when start button is called
        {
            //Reset all information to initial state

            _time = TimeSpan.FromSeconds(60); // reset time to 60 seconds
            timerText.Text = _time.ToString("c"); // reset time to current 60 seconds
            scoreText.Text = "0"; // reset score to zero
            userInputText.Text = ""; // reset user input to null
            remainingText.Text = "169"; // reset the remaining number of questions to 169 because 13x13 of them
            isStart = true; // reset state to start
            startButton.IsEnabled = false; // disable the startButton as long as timer is running
            enterButton.IsEnabled = true; // enable the enterButton as long as timer is running
            optCombo.IsEnabled = false; // disable the choosing of 4 operations
            array = new int[numCols]; // reset array to zeros
            pastResult = new List<double>(); // reset past result to zeros
            rnd = new Random(); // reset random
            timer.Start(); // start the timer
            GenerateQuestions(); // generate a random new set of question
        }

        // Grid itself handles the KeyDown event, and sets the flag to stop the event from bubbling further
        // So need to handle the PreviewKeyDown event instead. This gives us a chance to respond to the key down event before the Grid does.
        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (enterButton.Foreground == Brushes.Gray) // if enter button is released, reset the colour of the button
            {
                enterButton.Foreground = Brushes.Black;
            }
            if (e.Key == Key.Return)
            {
                if (!startButton.IsEnabled) // if the start button is not enabled then allow enter key to be pressed
                {
                    enterButton.Foreground = Brushes.Gray; // set colour of enter button to Gray when enter button is pressed
                    EnterButton_Click(sender, e);
                }
            }
        }

        private void GenerateQuestions() // Generate questions for user and verify user input
        {
            int[] listRand = RandGenerate().ToArray(); // generate the 2 random numbers

            int num1 = listRand[0]; // get the first number
            int num2 = listRand[1]; // get the second number

            if (!array.Contains(0)) // check if all questions have been answered
            {
                timer.Stop(); // stop the timer
                EndGame();
                MsgBoxSuccess(); // dialog box to show percentage score
                extraText.Text = "All questions have been answered";
            } else
            {
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
        }

        private int[] RandGenerate() // generate and return 2 random numbers
        {
            int[] retList = new int[2]; // return list for the 2 random numbers
            bool isNotSelected = true; // boolean to track if 2 random numbers have been selected
            int i = 0; // starting search index to find first occurrence of zero in array

            while(isNotSelected && i < numCols && array.Contains(0))
            {
                int index = Array.IndexOf(array, 0, i);

                if (index == -1) // if no index is being chosen
                {
                    i = 0; // reset the starting search index
                    index = Array.IndexOf(array, 0, i); // get the index of the first occurrence of zero in array
                }

                // select the question with a probability of 1 / (169-i)
                // as we loop through the for loop, higher the probability of choosing a random number same as i
                // technique adopted is called selection sampling
                // this is to ensure that all 169 pairs are guaranteed to be shown eventually

                int getRandNum = rnd.Next(index, numCols); // get a random number between index inclusive and 169 exclusive
                if (getRandNum == index) // if question is selected
                {
                    array[index] = 1; // mark the question as selected
                    remainingText.Text = (int.Parse(remainingText.Text) - 1).ToString(); // decrement the number of question left
                    retList[0] = subTrackCombi[index][0]; // add the first selected number
                    retList[1] = subTrackCombi[index][1]; // add the second selected number
                    isNotSelected = false; // set to false because 2 random numbers have been selected
                }
                else // if no question is being selected
                {
                    i = index+1; // increment the starting search index to find the next occurrence of zero in array
                }
            }
            return retList;
        }

        private void AddFunc(int num1, int num2) // addition function
        {
            int result = num1 + num2; // add the numbers
            questionText.Text = num1.ToString() + " + " + num2.ToString(); // set the question
            VerifyUserInput((double)result);
        }

        private void SubtractFunc(int num1, int num2) // subtraction function
        {
            int result = num1 - num2; // subtract the numbers
            questionText.Text = num1.ToString() + " - " + num2.ToString(); // set the question
            VerifyUserInput((double)result);
        }

        private void MultiplyFunc(int num1, int num2) // multiplication function
        {
            int result = num1 * num2; // multiply the numbers
            questionText.Text = num1.ToString() + " x " + num2.ToString(); // set the question
            VerifyUserInput((double)result);
        }

        private void DivideFunc(int num1, int num2) // division function
        {
            double result;
            if (num2 == 0) // to avoid division by zero
            {
                result = Math.Round((double)num2 / num1, 2); // divide the numbers
                questionText.Text = num2.ToString() + " / " + num1.ToString(); // set the question
            } else
            {
                result = Math.Round((double)num1 / num2, 2); // divide the numbers
                questionText.Text = num1.ToString() + " / " + num2.ToString(); // set the question
            }
            VerifyUserInput(result);
        }

        private void VerifyUserInput(double result) // check if the user input is same as the answer to the generated question
        {
            if (isStart) // this is necessary because when start button is clicked, result is tabulated but dont have any user input
            {
                pastResult.Add(result); // add result to list (list will have one result)
            }
            else // if it is not start state ie enter button is clicked (another question will be generated at the same time)
            {
                if (userInputText.Text.ToString() != "") // if user input is not null
                {
                    pastResult.Add(result); // add result to list (list will have two results)

                    if (double.TryParse(userInputText.Text.ToString(), out _)) // check if user input is a valid number
                    {
                        double userInput = double.Parse(userInputText.Text); // get user input as integer
                        if (pastResult[0] == userInput) // check if user input is the same as the first result in the list 
                        {
                            scoreText.Text = (int.Parse(scoreText.Text) + 1).ToString(); // same result increment score by 1
                        }
                        else
                        {
                            scoreText.Text = (int.Parse(scoreText.Text) - 1).ToString(); // different result decrement score by 1
                        }
                    }
                    userInputText.Text = ""; // erase user input 
                    pastResult.RemoveAt(0); // remove the first result from list
                }
            }
        }

        private void EndGame() // end of game
        {
            enterButton.IsEnabled = false; // disable the enter button
            startButton.IsEnabled = true; // enable the start button
            optCombo.IsEnabled = true; // enable the choosing of 4 operations
        }

        private void MsgBoxEndGame() // dialog box for end of game
        {
            string messageBoxText = "Time is up"; // set dialog box text message
            string caption = "Game Over"; // set dialog box title
            MessageBoxButton button = MessageBoxButton.OK; // ok button

            MessageBox.Show(messageBoxText, caption, button); // show the dialog box
        }

        private void MsgBoxEmptyList() // dialog box for empty list
        {
            string messageBoxText = "List is empty"; // set dialog box text message
            string caption = "Error"; // set dialog box title
            MessageBoxButton button = MessageBoxButton.OK; // ok button

            MessageBox.Show(messageBoxText, caption, button); // show the dialog box
        }

        private void MsgBoxSuccess() // dialog box for having answered all questions
        {
            string messageBoxText;
            if (int.Parse(scoreText.Text) < 0)
            {
                messageBoxText = "Sorry your score is " + int.Parse(scoreText.Text) + ". Better luck next round!"; // set dialog box text message
            } else
            {
                messageBoxText = "Congratulation you have scored " + Math.Round(100.0 * double.Parse(scoreText.Text) / numCols, 2) + "%"; // set dialog box text message
            }
                
            string caption = "Success"; // set dialog box title
            MessageBoxButton button = MessageBoxButton.OK; // ok button

            MessageBox.Show(messageBoxText, caption, button); // show the dialog box
        }
    }
}
