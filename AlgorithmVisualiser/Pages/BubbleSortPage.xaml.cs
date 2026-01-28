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

namespace AlgorithmVisualiser.Pages
{
    public partial class BubbleSortPage : Page
    {
        public BubbleSortPage()
        {
            InitializeComponent();
        }

        Rectangle[] rects;

        // On LoadElements button clicked
        void LoadElements(object sender, RoutedEventArgs e)
        {            
            // Clear all elements from the canvas
            Display.Children.Clear();

            // Load rects from file
            rects = InputManager.GenerateRectsFromData(Display, InputManager.LoadFromFile(), Color.FromRgb(46, 46, 46));
        }

        // On GenerateElements button clicked
        void GenerateElements(object sender, RoutedEventArgs e)
        {
            // Clear all elements from the canvas
            Display.Children.Clear();

            // Input validation
            if (int.TryParse(ElementInput.Text, out int elements)) 
            {
                // Get random rects
                rects = InputManager.GenerateRandomRects(Display, Convert.ToInt32(ElementInput.Text), Color.FromRgb(46, 46, 46));
            }
            else // Fail validation, display error
            {
                MessageBox.Show("Invalid data input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // On SortElements button clicked
        void SortElements(object sender, RoutedEventArgs e)
        {

        }
    }
}
