using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoL_Checked(object sender, RoutedEventArgs e)
        {
            golButtons.Visibility = Visibility.Visible;
            laButtons.Visibility = Visibility.Hidden;
            GoLRulesGrid.Visibility = Visibility.Visible;
            LARulesGrid.Visibility = Visibility.Hidden;
        }

        private void AliveCellsBoxColorPicker_Closed(object sender, RoutedEventArgs e)
        {
            aliveCellsBox.Foreground = new SolidColorBrush((System.Windows.Media.Color)aliveCellsBoxColorPicker.SelectedColor);
        }

        private void DeadCellsBoxColorPicker_Closed(object sender, RoutedEventArgs e)
        {
            deadCellsBox.Foreground = new SolidColorBrush((System.Windows.Media.Color)deadCellsBoxColorPicker.SelectedColor);
        }

        private void LA_Checked(object sender, RoutedEventArgs e)
        {
            golButtons.Visibility = Visibility.Hidden;            
            laButtons.Visibility = Visibility.Visible;
            GoLRulesGrid.Visibility = Visibility.Hidden;
            LARulesGrid.Visibility = Visibility.Visible;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {            
            float space = 14f;
            if((bool)GoL.IsChecked)
            {                
                GoLPlay p = new GoLPlay(aliveCellsBoxColorPicker.SelectedColor.ToString(), deadCellsBoxColorPicker.SelectedColor.ToString(), Int32.Parse(widthBox.Text), Int32.Parse(heightBox.Text), Int32.Parse(cellSizeBox.Text))
                {
                    Width = (Int32.Parse(widthBox.Text) * Int32.Parse(cellSizeBox.Text)) + space,
                    Height = (Int32.Parse(heightBox.Text) * Int32.Parse(cellSizeBox.Text)) + SystemParameters.WindowCaptionHeight + space
                };
                p.Start();
                p.Show();
            }
            if((bool)LA.IsChecked)
            {
                LAPlay p = new LAPlay(aliveCellsBoxColorPicker.SelectedColor.ToString(), deadCellsBoxColorPicker.SelectedColor.ToString(), Int32.Parse(widthBox.Text), Int32.Parse(heightBox.Text), Int32.Parse(cellSizeBox.Text))
                {
                    Width = (Int32.Parse(widthBox.Text) * Int32.Parse(cellSizeBox.Text)) + space,
                    Height = (Int32.Parse(heightBox.Text) * Int32.Parse(cellSizeBox.Text)) + SystemParameters.WindowCaptionHeight + space
                };
                p.Start();
                p.Show();
            }            
            this.Close();
        }

        private void HeightBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
              Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }            
        }

        private void HeightBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                int val = Convert.ToInt32(tb.Text);
                if(val <= 0 || val > 500)
                    tb.Text = "30";
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void CellSizeBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                int val = Convert.ToInt32(tb.Text);
                if(val <= 0 || val > 250)
                    tb.Text = "20";
            }
            catch
            {
                e.Handled = true;
            }
        }
    }
}
