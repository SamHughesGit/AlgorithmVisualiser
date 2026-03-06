using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AlgorithmVisualiser.Pages
{
    public partial class BinarySearchPage : Page
    {
        Sorter.BinarySearch searcher;

        public BinarySearchPage()
        {
            InitializeComponent();
            searcher = new Sorter.BinarySearch();
            BigO.Content = $"Big {searcher.BigO}";
        }

        Rectangle[]? rects;
        int[]? vals;
        int indexFound;

        Color baseColor = Color.FromRgb(46, 46, 46);

        // On LoadElements button clicked
        async void LoadElements(object sender, RoutedEventArgs e)
        {
            // Clear all elements from the canvas
            Display.Children.Clear();

            // Load values from file & generate rectanlges from them
            vals = await InputManager.LoadFromFile();
            rects = InputManager.GenerateRectsFromData(Display, vals, baseColor);
        }

        // On GenerateElements button clicked
        void GenerateElements(object sender, RoutedEventArgs e)
        {
            // Clear all elements from the canvas
            Display.Children.Clear();

            // Input validation
            if (int.TryParse(ElementInput.Text, out int elements))
            {
                // Get rects
                (rects, vals) = InputManager.GenerateRects(Display, Convert.ToInt32(ElementInput.Text), baseColor);

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
            // Restore colors
            SolidColorBrush baseColorRect = new SolidColorBrush(baseColor);
            foreach (Rectangle r in rects) r.Fill = baseColorRect;

            bool isSorted = Sorter.Util.isSorted(vals);
            if (!isSorted) { MessageBox.Show("Array must be sorted!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            // If valid inputs, search
            if (int.TryParse(SearchInput.Text, out int target))
            {
                if (int.TryParse(DelayInput.Text, out int delay))
                {
                    SearchButton.IsEnabled = false;

                    long time;
                    (time, indexFound) = await searcher.SearchElements(rects, vals, baseColor, delay, target);

                    SearchButton.IsEnabled = true;

                    TimeToFinish.Content = $"Time: {time}ms";

                    if (indexFound != -1) MessageBox.Show($"Item {target} found at index {indexFound}", "Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    else MessageBox.Show($"Item {target} was not found in the dataset", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Invalid delay!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid search item!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
