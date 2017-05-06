using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            Properties.Settings.Default.ShowGridLines = (bool)ShowGridCheckBox.IsChecked;
            Properties.Settings.Default.ShowNextFigure = (bool)ShowNextFigureCheckbox.IsChecked;
            Properties.Settings.Default.PlaySound = (bool)PlaySoundCheckBox.IsChecked;
            Properties.Settings.Default.WithAcceleration = (bool)WithAccelerationCheckBox.IsChecked;

            Properties.Settings.Default.Save();

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowGridCheckBox.IsChecked = Properties.Settings.Default.ShowGridLines;
            ShowNextFigureCheckbox.IsChecked = Properties.Settings.Default.ShowNextFigure;
            PlaySoundCheckBox.IsChecked = Properties.Settings.Default.PlaySound;
            WithAccelerationCheckBox.IsChecked = Properties.Settings.Default.WithAcceleration;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainWindow = Application.Current.Windows[0] as MainWindow;

            if (null == mainWindow) return;

            mainWindow.PlaySound(Properties.Settings.Default.PlaySound);
            mainWindow.GridLines(Properties.Settings.Default.ShowGridLines);
            mainWindow.ShowNextFigure(Properties.Settings.Default.ShowNextFigure);
         
            if (mainWindow.CurrentState == (byte)MainWindow.States.Pause)
            {               
                mainWindow.StartTimers();                   
            }
        }
    }
}
