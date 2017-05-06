using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region State
        public enum States : byte { Start, Pause, Continue };
        #endregion

        #region Property changed event handler
        // property changed event
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        #region Private properties       
        private string _stateButton;
        private string _stateButtonToolTip;
        private string _playSoundImage;
        private string _playSoundButtonToolTip;
        private string _bestScore;
        private int _score;
        private int _linesCount;
        private Visibility _showGridLines;
        private Visibility _showNext;
        private Visibility _showGameOver;
        private string _timerValue;
        private byte _currentState;
        private Random _random;
        private Figure _currentFigure;
        private bool _canGenerateFigure;
        private int _nextFigureNumber;
        private string _stateButtonHeader;
        #endregion

        #region Properties
        public string StateButton
        {
            get { return _stateButton; }
            set
            {
                _stateButton = value;
                OnPropertyChanged("StateButton");
            }
        }
        public string StateButtonToolTip
        {
            get { return _stateButtonToolTip; }
            set
            {
                _stateButtonToolTip = value;
                OnPropertyChanged("StateButtonToolTip");
            }
        }
        public string PlaySoundButtonToolTip
        {
            get { return _playSoundButtonToolTip; }
            set
            {
                _playSoundButtonToolTip = value;
                OnPropertyChanged("PlaySoundButtonToolTip");
            }
        }
        public string PlaySoundImage
        {
            get { return _playSoundImage; }
            set
            {
                _playSoundImage = value;
                OnPropertyChanged("PlaySoundImage");
            }
        }
        public string BestScore
        {
            get { return _bestScore; }
            set
            {
                _bestScore = value;
                OnPropertyChanged("BestScore");
            }
        }
        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                OnPropertyChanged("Score");
            }
        }
        public int LinesCount
        {
            get { return _linesCount; }
            set
            {
                _linesCount = value;
                OnPropertyChanged("LinesCount");
            }
        }
        public Visibility ShowGridLines
        {
            get { return _showGridLines; }
            set
            {
                _showGridLines = value;
                OnPropertyChanged("ShowGridLines");
            }
        }
        public Visibility ShowNext
        {
            get { return _showNext; }
            set
            {
                _showNext = value;
                OnPropertyChanged("ShowNext");
            }
        }
        public Visibility ShowGameOver
        {
            get { return _showGameOver; }
            set
            {
                _showGameOver = value;
                OnPropertyChanged("ShowGameOver");
            }
        }
        public string TimerValue
        {
            get { return _timerValue; }
            set
            {
                _timerValue = value;
                OnPropertyChanged("TimerValue");
            }
        }
        public string StateButtonHeader
        {
            get { return _stateButtonHeader; }
            set
            {
                _stateButtonHeader = value;
                OnPropertyChanged("StateButtonHeader");
            }
        }
        public byte CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
        private Figure CurrentFigure
        {
            get { return _currentFigure; }
            set { _currentFigure = value; }
        }
        private bool CanGenerateFigure
        {
            get { return _canGenerateFigure; }
            set { _canGenerateFigure = value; }
        }      
                
        private Random Random
        {
            get { return _random; }
            set { _random = value; }
        }
        #endregion

        #region Timer
        private static DispatcherTimer _dispatcherTimer;
        private DispatcherTimer _movementTimer;
        private static DateTime _startTime;
        private static TimeSpan _currentTimeValue;

        public TimeSpan CurrentTimeValue
        {
            get { return _currentTimeValue; }
            set { _currentTimeValue = value; }
        }

        public void InitTimers(DateTime timerStart)
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _startTime = timerStart;
            _dispatcherTimer.Start();

            _movementTimer = new DispatcherTimer();
            _movementTimer.Tick += new EventHandler(MovementTimerTick);
            _movementTimer.Interval = new TimeSpan(0, 0, 0, 0, Properties.Settings.Default.SpeedNormal);
            _movementTimer.Start();
        }

        public void StopTimers()
        {
            if (_dispatcherTimer == null)
                return;
            if (_movementTimer == null)
                return;
            _dispatcherTimer.Stop();                     
            _movementTimer.Stop();
            
        }

        public void StartTimers()
        {
            if (_dispatcherTimer == null)
                return;
            if (_movementTimer == null)
                return;
            _dispatcherTimer.Start();
            _movementTimer.Start();

        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            _currentTimeValue = DateTime.Now - _startTime;
            TimerValue = string.Format("{0:D2}:{1:D2}:{2:D2}", _currentTimeValue.Hours, _currentTimeValue.Minutes, _currentTimeValue.Seconds);
        }

        private void MovementTimerTick(object sender, EventArgs e)
        {
            GenerateNewFigure(_nextFigureNumber, Board, 24, 24, 25);

            if (CurrentFigure == null)
            {
                return;
            }

            if (MovementHelper.CanMoveDown(CurrentFigure, Board))
            {
                MoveDown(CurrentFigure);
            }
            else
            {
                //finish the game if figure can't move down from start point
                if (CurrentFigure.Top <= 0)
                {
                    StopTheGame();
                }
                int lines;
                Score = MovementHelper.CalculateScore(CurrentFigure, Board, Score, out lines);
                LinesCount += lines;
                CurrentFigure = null;
                if (Properties.Settings.Default.WithAcceleration)
                {
                    SetMovementSpeed(Properties.Settings.Default.SpeedWithAcceleration);
                }
                else
                {
                    SetMovementSpeed(Properties.Settings.Default.SpeedNormal);
                }       
            }
        }
        #endregion

        #region Figures
        private void GenerateNewFigure(int number, Canvas canvas, int width, int height, int gridCellWidth)
        {
            if (CurrentFigure == null && CanGenerateFigure)
            {
                CurrentFigure = new Figure(number, canvas, width, height, gridCellWidth);

                int max = (int)(canvas.Width - CurrentFigure.Width) / CurrentFigure.Offset;

                int left = Random.Next(1, max) * CurrentFigure.Offset;

                CurrentFigure.SetLocation(left, (-1) * CurrentFigure.Offset);

                GenerateNextFigure();
            }
        }

        private void GenerateNextFigure()
        {
            NextFigurePreview.Children.Clear();
            _nextFigureNumber = Random.Next(0, 1000) % 7;
            Figure nextFigure = new Figure(_nextFigureNumber, NextFigurePreview, 15, 15, 16);
            nextFigure.SetLocation((int)((NextFigurePreview.Width - nextFigure.Width) / 2), 10);
        }

        private void MoveDown(Figure figure)
        {
            figure.SetLocation(figure.Left, figure.Offset + figure.Top);
        }

        private void StopTheGame()
        {               
            Board.Opacity = 0.3;
            ResetCurrentState();
            ShowGameOver = Visibility.Visible;
        }
        #endregion

        #region Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StateButton = Properties.Settings.Default.StateButtonInitialValue;
            StateButtonToolTip = Properties.Settings.Default.StateButtonToolTipInitialValue;
            BestScore = Properties.Settings.Default.BestScoreInitialValue.ToString();
            Score = Properties.Settings.Default.ScoreInitialValue;           
            CurrentState = (byte)States.Start;
            PlaySound(Properties.Settings.Default.PlaySound);
            GridLines(Properties.Settings.Default.ShowGridLines);
            ShowNextFigure(Properties.Settings.Default.ShowNextFigure);
            StateButtonHeader = "_Start";
            ShowGameOver = Visibility.Hidden;
            ResetSpeedSettings();
        }

        private void StateButton_Click(object sender, MouseButtonEventArgs e)
        {
            SetStateFromCurrentState();
        }              

        private void SetStateFromCurrentState()
        {
            if (ShowGameOver == Visibility.Visible)
            {
                ShowGameOver = Visibility.Hidden;
            }
            switch (CurrentState)
            {
                case (byte)States.Start:
                TimerValue = Properties.Settings.Default.TimerInitialValue;
                InitTimers(DateTime.Now);
                StateButton = Properties.Settings.Default.StateButtonPauseValue;
                StateButtonToolTip = Properties.Settings.Default.StateButtonToolTipPausedValue;
                Score = Properties.Settings.Default.ScoreInitialValue;
                LinesCount = 0;
                CurrentState = (byte)States.Pause;
                StateButtonHeader = "_Pause";
                EnableArrowButtons(true);
                Board.Opacity = 1;
                Board.Children.Clear();
                CurrentFigure = null;
                CanGenerateFigure = true;
                SetMovementSpeed(Properties.Settings.Default.SpeedWithAcceleration);
                break;
                case (byte)States.Pause:
                StopTimers();
                StateButton = Properties.Settings.Default.StateButtonContinueValue;
                StateButtonToolTip = Properties.Settings.Default.StateButtonToolTipContinueValue;
                CurrentState = (byte)States.Continue;
                StateButtonHeader = "_Continue";
                EnableArrowButtons(false);
                break;
                case (byte)States.Continue:
                InitTimers(DateTime.Now - CurrentTimeValue);
                StateButton = Properties.Settings.Default.StateButtonPauseValue;
                StateButtonToolTip = Properties.Settings.Default.StateButtonToolTipPausedValue;
                CurrentState = (byte)States.Pause;
                StateButtonHeader = "_Pause";
                EnableArrowButtons(true);
                break;
            }
        }

        private void ResetCurrentState()
        {
            CanGenerateFigure = false;
            StopTimers();
            StateButton = Properties.Settings.Default.StateButtonInitialValue;
            StateButtonToolTip = Properties.Settings.Default.StateButtonToolTipInitialValue;
            CurrentState = (byte)States.Start;
            StateButtonHeader = "_Start";
            EnableArrowButtons(false);
            NextFigurePreview.Children.Clear();
            SaveNewBestScore(Score);
            ResetSpeedSettings();
        }

        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PlaySound = !Properties.Settings.Default.PlaySound;
            Properties.Settings.Default.Save();

            PlaySound(Properties.Settings.Default.PlaySound);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentState == (byte)States.Pause)
            {
                e.Handled = true;
            }

            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    //handle G key
                    case Key.G:
                    SetStateFromCurrentState();
                    break;
                    //handle R key
                    case Key.R:
                    ResetCurrentState();
                    Board.Children.Clear();
                    TimerValue = Properties.Settings.Default.TimerInitialValue;
                    Score = Properties.Settings.Default.ScoreInitialValue;
                    break;
                    default:
                    e.Handled = true;
                    break;
                }                
            }
            //handle F1 key
            if (e.Key == Key.F1)
            {
                MenuItemHelp_Click(sender, e);
                e.Handled = true;
            }

            if ((CurrentFigure == null) || (CurrentState == (byte)States.Continue)) return;

            //rotate current figur to left
            if (e.Key == Key.Down)
            {
                MovementHelper.RotateLeft(CurrentFigure, Board);
            }
            //rotate current figure to right
            if (e.Key == Key.Up)
            {
                MovementHelper.RotateRight(CurrentFigure, Board);
            }

            //move current figure to left
            if (e.Key == Key.Left)
            {
                MovementHelper.MoveLeft(CurrentFigure, Board);
            }

            //move current figure to right
            if (e.Key == Key.Right)
            {
                MovementHelper.MoveRight(CurrentFigure, Board);
            }

            //move current figure to bottom of the board
            if (e.Key == Key.Space)
            {
                SetMovementSpeed(0);
            }            
        }

        private void ButtonRotateLeft_Click(object sender, RoutedEventArgs e)
        {
            MovementHelper.RotateLeft(CurrentFigure, Board);
        }

        private void ButtonMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            MovementHelper.MoveLeft(CurrentFigure, Board);
        }

        private void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            SetMovementSpeed(0);
        }

        private void ButtonMoveRight_Click(object sender, RoutedEventArgs e)
        {
            MovementHelper.MoveRight(CurrentFigure, Board);
        }

        private void ButtonRotateRight_Click(object sender, RoutedEventArgs e)
        {
            MovementHelper.RotateRight(CurrentFigure, Board);
        }
        #endregion

        #region Utilities
        private void EnableArrowButtons(bool isEnable)
        {
            ButtonMoveLeft.IsEnabled = ButtonMoveRight.IsEnabled = ButtonMoveDown.IsEnabled = ButtonRotateLeft.IsEnabled = ButtonRotateRight.IsEnabled = isEnable;
        }

        private void SaveNewBestScore(int score)
        {
            if ((Properties.Settings.Default.BestScoreInitialValue == Properties.Settings.Default.ScoreInitialValue) ||
                (Properties.Settings.Default.BestScoreInitialValue < score))
            {
                Properties.Settings.Default.BestScoreInitialValue = score;
                Properties.Settings.Default.Save();
                BestScore = Properties.Settings.Default.BestScoreInitialValue.ToString();
            }
        }

        private void SetMovementSpeed(int speed)
        {
            _movementTimer.Interval = new TimeSpan(0, 0, 0, 0, speed);
        }

        public void PlaySound(bool playSound)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string appRoot = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(appRoot, "tetris_theme.wav");
            if (!System.IO.File.Exists(filePath))
            {
                // The path to the Sounds folder.
                string directory = System.IO.Path.Combine(appRoot, @"../..");
                // The path to the sound file.
                filePath = System.IO.Path.Combine(directory, "Sounds/tetris_theme.wav");
            }

            // Start music
            if (System.IO.File.Exists(filePath))
            { 
                SoundPlayer sound = new SoundPlayer(filePath);
                if (playSound)
                {
                    PlaySoundImage = Properties.Settings.Default.PlaySoundImage;
                    PlaySoundButtonToolTip = Properties.Settings.Default.PlaySoundButtonToolTip;
                    sound.PlayLooping();
                }
                else
                {
                    sound.Stop();
                    PlaySoundImage = Properties.Settings.Default.MuteImage;
                    PlaySoundButtonToolTip = Properties.Settings.Default.MuteButtonToolTip;
                }
            }
        }

        public void GridLines(bool showGrid)
        {
            if (showGrid)
            {
                ShowGridLines = Visibility.Visible;
            }
            else
            {
                ShowGridLines = Visibility.Hidden;
            }
        }

        public void ShowNextFigure(bool showNextFigure)
        {
            if (showNextFigure)
            {
                ShowNext = Visibility.Visible;
            }
            else
            {
                ShowNext = Visibility.Hidden;
            }
        }

        private void ResetSpeedSettings()
        {
            Properties.Settings.Default.SpeedWithAcceleration = Properties.Settings.Default.SpeedNormal;
            Properties.Settings.Default.Save();
        }
       
        #endregion

        #region Menu
        
        private void MenuItemStart_Click(object sender, RoutedEventArgs e)
        {
            SetStateFromCurrentState();
        }

        private void MenuItemReset_Click(object sender, RoutedEventArgs e)
        {
            ResetCurrentState();
            Board.Children.Clear();
            TimerValue = Properties.Settings.Default.TimerInitialValue;
            Score = Properties.Settings.Default.ScoreInitialValue;
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            StopTimers();
            Window options = new Options();
            options.Show();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string appRoot = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(appRoot, "Help.chm");
            if (!System.IO.File.Exists(filePath))
            {
                // The path to the Help folder.
                string directory = System.IO.Path.Combine(appRoot, @"../..");
                // The path to the Help file.
                filePath = System.IO.Path.Combine(directory, "Help/Help.chm");
            }
            // Launch the Help file.
            if (System.IO.File.Exists(filePath))
            {
                Process.Start(filePath);
            }
            else
            {
                MessageBox.Show("File not found!");
            }
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            using (AboutBox box = new AboutBox())
            {
                box.ShowDialog();
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Random = new Random();
        }        
    }
}
