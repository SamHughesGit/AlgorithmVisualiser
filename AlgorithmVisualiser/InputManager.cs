using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Automation.Peers;

namespace AlgorithmVisualiser
{
    static class InputManager
    {
        public static async Task<int[]> LoadFromFile()
        {
            // Open new file dialog popup
            OpenFileDialog dialog = new OpenFileDialog();

            // Filter for text files (name|extension)
            dialog.Filter = "Text files|*.txt";

            // If user selects a file
            if (dialog.ShowDialog() == true)
            {
                // Get file path
                string path = dialog.FileName;

                try 
                {
                    // Read data from file
                    string content = File.ReadAllText(path);

                    // Parse data from file
                    int[] parsed = ParseData(content);

                    return parsed;
                }
                catch (Exception e) // Catch error opening file
                {
                    // Error pop up, ok box + error symbol
                    MessageBox.Show("Error reading file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Return null if error occurs. 
            return null;
        }

        // Parse content from string to int array
        public static int[] ParseData(string data)
        {
            // Anything that is not a number or a comma using regex and replacing with an empty string
            string cleanedInput = Regex.Replace(data, "[^0-9,]", "");

            // Split cleaned string into parts using ,'s
            string[] parts = cleanedInput.Split(',');

            // New int array
            int[] result = new int[parts.Length];

            // For each part in parts, add to result array as a parsed int
            for(int i = 0; i < parts.Length; i++)
            {
                // If can be parsed, add to array
                if (int.TryParse(parts[i], out int num))
                {
                    result[i] = num;
                }
                else // Else default to zero, shouldnt happen
                {
                    result[i] = 0;
                }
            }

            return result;
        }

        // Convert int array to array of rectangle objects
        public static Rectangle[] GenerateRectsFromData(Canvas canvas, int[] data, Color color)
        {
            // If no data return early
            if(data == null) { return null; }

            // Gap of 1px between bars
            int gap = 1;

            // Calculate rectangle width
            double width = (canvas.ActualWidth / data.Length) - gap;

            // Prevent <1 widths
            if (width < 1)
            {
                if (MessageBox.Show("There are too many elements to display, would you still like to sort them?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return null;
                }
                width = 0.1;
            }

            Rectangle[] rects = new Rectangle[data.Length];

            // Create bars
            for(int i = 0; i <  data.Length; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = MapRange(data[i], data.Min(), data.Max(), canvas);
                rect.Width = width;
                rect.Fill = new SolidColorBrush(color);
                if (!(width < 1))
                {
                    // Dynamically place
                    Canvas.SetLeft(rect, width * i + gap * i + 1);
                    Canvas.SetBottom(rect, 0);
                    canvas.Children.Add(rect);
                }
                rects[i] = rect;
            }

            return rects;
        }

        // Generate random rectangles
        public static (Rectangle[]?, int[]?) GenerateRandomRects(Canvas canvas, int elementCount, Color color)
        {
            Rectangle[] rects = new Rectangle[elementCount];
            int[] vals = new int[elementCount];

            // Gap of 1px
            int gap = 1;

            // Calculate rectangle width
            double width = (canvas.ActualWidth / elementCount) - gap;

            // Prevent < 1 pixel widths
            if (width < 1) { return (null, null); }

            // New random
            Random rng = new Random();

            // Random nums list
            List<int> randomNums = new List<int>();
            for (int i = 1; i < elementCount+1; i++)
            {
                randomNums.Add(i);
            }
            
            // Generate random rectangles, ensuring no dulicated values using a list of values
            for (int i = 0; i < elementCount; i++)
            {
                Rectangle rect = new Rectangle();
                int index = rng.Next(0, randomNums.Count);
                int randomValue = randomNums[index];
                randomNums.RemoveAt(index);
                rect.Height = MapRange(randomValue, 1, elementCount, canvas);
                rect.Width = width;
                rect.Fill = new SolidColorBrush(color);
                // Dynamically place
                Canvas.SetLeft(rect, width * i + gap * i + 1);
                Canvas.SetBottom(rect, 0);
                canvas.Children.Add(rect);
                rects[i] = rect;
                vals[i] = randomValue;
            }

            return (rects, vals);
        }        

        // Map range of data set to pixel size
        public static int MapRange(int val, int dataMin, int dataMax, Canvas canvas)
        {
            int targetMax = (int)(canvas.ActualHeight - 10);
            int targetMin = 5;

            // Map value into range of canvas size
            return targetMin + (val - dataMin) * (targetMax - targetMin) / (dataMax - dataMin);
        }
    }
}
