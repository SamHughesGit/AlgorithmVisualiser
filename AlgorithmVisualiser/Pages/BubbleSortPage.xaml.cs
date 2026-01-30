using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        Sorter.BubbleSort sorter;

        public BubbleSortPage()
        {
            InitializeComponent();
            sorter = new Sorter.BubbleSort();
        }

        Rectangle[] rects;
        Color baseColor = Color.FromRgb(46, 46, 46);

        // On LoadElements button clicked
        async void LoadElements(object sender, RoutedEventArgs e)
        {            
            // Clear all elements from the canvas
            Display.Children.Clear();

            // Load rects from file
            rects = InputManager.GenerateRectsFromData(Display, await InputManager.LoadFromFile(), baseColor);

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
                rects = InputManager.GenerateRandomRects(Display, Convert.ToInt32(ElementInput.Text), baseColor);

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

                long time = await sorter.SortElements(rects, baseColor, delay);

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
                        string output = string.Join(", ", rects.Select(r => r.Height.ToString()).ToArray());

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

///
/// TODO: FIX EXPORT FROM HEIGHT, THIS EXPORTS THE MAPPED VALUE, THATS BAD WE NEED THE ORIGINAL
///

///
/// TODO: MORE ALGORITHMS, COMPARE GRAPH <-- THIS IS PRIORITY
///