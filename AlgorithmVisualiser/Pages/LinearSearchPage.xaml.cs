using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>
    /// Interaction logic for LinearSearchPage.xaml
    /// </summary>
    public partial class LinearSearchPage : Page
    {
        Sorter.LinearSearch searcher;

        public LinearSearchPage()
        {
            InitializeComponent();
            searcher = new Sorter.LinearSearch();
            BigO.Content = $"Big {searcher.BigO}";
        }

        Rectangle[]? rects;
        int[]? vals;

        Color baseColor = Color.FromRgb(46, 46, 46);

        // On LoadElements button clicked
        async void LoadElements(object sender, RoutedEventArgs e)
        {
            // Clear all elements from the canvas
            Display.Children.Clear();

            // Load values from file & generate rectanlges from them
            vals = await InputManager.LoadFromFile();
            rects = InputManager.GenerateRectsFromData(Display, vals, baseColor);

            // MessageBox.Show($"{rects.Length} elements loaded", "Loaded!", MessageBoxButton.OK, MessageBoxImage.Information);
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
                (rects, vals) = InputManager.GenerateRandomRects(Display, Convert.ToInt32(ElementInput.Text), baseColor);

                if (rects == null)
                {
                    MessageBox.Show("Too many elements to display!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Fail validation, display error
            {
                MessageBox.Show("Invalid data input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // On Search button clicked
        async void SearchElements(object sender, RoutedEventArgs e)
        {

        }
    }
}
