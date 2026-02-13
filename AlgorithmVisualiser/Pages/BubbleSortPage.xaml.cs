using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AlgorithmVisualiser.Pages
{
    public partial class BubbleSortPage : Page
    {
        Sorter.BubbleSort sorter;

        public BubbleSortPage()
        {
            InitializeComponent();
            sorter = new Sorter.BubbleSort();
            BigO.Content = $"Big {sorter.BigO}";
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

                if(rects == null)
                {
                    MessageBox.Show("Too many elements to display!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // Fail validation, display error
            {
                MessageBox.Show("Invalid data input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // On SortElements button clicked
        async void SortElements(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(DelayInput.Text, out int delay))
            {
                // Disable button to prevent double-clicking
                SortButton.IsEnabled = false;

                long time;
                // Returns time to sort in ms and list of sorted values
                (time, vals) = await sorter.SortElements(rects, vals, baseColor, delay);

                SortButton.IsEnabled = true;

                // Display time taken
                TimeToFinish.Content = $"Time: {time}ms";
            }

            // Ask to export?
            if(MessageBox.Show("Would you like to export the sorted data-set?", "Export", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                OpenFolderDialog dialog = new OpenFolderDialog();

                dialog.Title = "Select an export folder";
                
                if(dialog.ShowDialog() == true)
                {
                    string path = dialog.FolderName;

                    try // Try write file
                    {
                        // Convert rect array into string of heights seperated by comma
                        string output = string.Join(", ", vals);

                        File.WriteAllText(System.IO.Path.Combine(path, "output.txt"), output);
                    }
                    catch // Write failed
                    {
                        MessageBox.Show("An error occured during export.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}