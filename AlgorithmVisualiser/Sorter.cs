using AlgorithmVisualiser.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AlgorithmVisualiser
{
    // Base class sorters derive from
    public abstract class Sort
    {
        public string BigO { get; set; }
        public abstract Task<(long, int[])> SortElements(Rectangle[] elements, int[] vals, Color baseColor, int delay);
        public abstract long TimeSort(ref int[] vals);

    }

    static class Sorter
    {
        // List of pages for dynamically creating buttons
        static public List<Page> pages = new List<Page> { new BubbleSortPage(), new InsertionSortPage() };

        // Set color of a list of elements
        static public async Task SetColors(Rectangle[] items, SolidColorBrush color, int delay)
        {
            foreach (Rectangle r in items)
            {
                r.Fill = color;
            }
            await Task.Delay(delay);
        }

        // Swap visual positions
        static public async Task SwapElementPos(Rectangle a, Rectangle b, int delay)
        {
            double bDesired = Canvas.GetLeft(a);
            double aDesired = Canvas.GetLeft(b);

            double difference = Math.Abs(aDesired - bDesired);
            int steps = 20;
            double step = difference / steps;

            int wait = delay / steps;

            double aPos = Canvas.GetLeft(a);
            double bPos = Canvas.GetLeft(b);

            bool areInPosition = (Math.Abs(aPos - aDesired) <= step && Math.Abs(bPos - bDesired) <= step);
            bool shouldAnimate = delay > steps;

            while (!areInPosition && shouldAnimate)
            {
                aPos = Canvas.GetLeft(a);
                bPos = Canvas.GetLeft(b);
                Canvas.SetLeft(a, aPos + (aPos < aDesired?step:-step));
                Canvas.SetLeft(b, bPos + (bPos < bDesired?step:-step));
                await Task.Delay(wait);
                areInPosition = (Math.Abs(aPos - aDesired) <= step && Math.Abs(bPos - bDesired) <= step);
            }

            // Finally, set desired positions to account for small floating point errors
            Canvas.SetLeft(a, aDesired);
            Canvas.SetLeft(b, bDesired);
        }

        // Animated move to pos
        static public async Task MoveElementTo(Rectangle a, double pos, int delay)
        {
            double aPos = Canvas.GetLeft(a);
            double difference = Math.Abs(aPos - pos);
            int steps = 20;
            double step = difference / steps;
            int time = (int)(Math.Abs((aPos - pos)) / step);

            while (Math.Abs(aPos - pos) >= step)
            {
                aPos = Canvas.GetLeft(a);
                Canvas.SetLeft(a, aPos + (aPos < pos ? step : -step));
                await Task.Delay(time);
            }

            // Finally, set desired positions to account for small floating point errors
            Canvas.SetLeft(a, pos);
        }

        static public bool isValidArray(Rectangle[] rects, int[] ints)
        {
            bool isInvalidArray = (rects == null || ints == null);
            if (isInvalidArray) return false;
            bool isArrayShort = (rects.Length == 1 || ints.Length == 1);
            if (isArrayShort) return false;
            return true;
        }

        public class BubbleSort : Sort
        {
            // Color vars
            private SolidColorBrush checkColor = new SolidColorBrush(Color.FromRgb(56, 65, 235));
            private SolidColorBrush swapColor = new SolidColorBrush(Color.FromRgb(222, 104, 89));
            private SolidColorBrush correctColor = new SolidColorBrush(Color.FromRgb(143, 204, 102));
            private SolidColorBrush baseColor;

            // Constructor
            public BubbleSort()
            {
                this.BigO = "O(n^2)";
            }

            // Perform the actual visual sort on the rectangle elements
            // Returns a long (time) and int array (values)
            public override async Task<(long, int[])> SortElements(Rectangle[] elements, int[] vals, Color baseColor, int delay)
            {
                // Ensure valid array
                if(!isValidArray(elements,vals)) { return (-1, vals); }

                // Store base color
                this.baseColor = new SolidColorBrush(baseColor);

                bool isInvalidWidth = elements[0].Width < 1;
                long timeToSort;

                // If invalid width, no visual sort
                if (isInvalidWidth)
                {
                    timeToSort = TimeSort(ref vals);
                    return (timeToSort, vals);
                }
                // Get time to sort and continue to visualise
                else { int[] sortedVals = (int[])vals.Clone();  timeToSort = TimeSort(ref sortedVals); }

                // Flag to check early sort
                bool swap = false;

                int elementCount = elements.Length;

                // Sort
                for (int i = 0; i < elementCount - 1; i++)
                {
                    // Reset swap flag every pass
                    swap = false;

                    for (int j = 0; j < elementCount - i - 1; j++)
                    {
                        await SetColors(elements[j..(j + 2)], checkColor, delay);

                        double currentVal = elements[j].Height;
                        double nextVal = elements[j + 1].Height;

                        // If next element < current, swap
                        if (nextVal < currentVal)
                        {
                            await SetColors(elements[j..(j + 2)], swapColor, delay);

                            // Swap elements using a temp variable to prevent overwriting value
                            await SwapElementPos(elements[j+1], elements[j], delay);

                            // Swap in rect array
                            Rectangle temp = elements[j];
                            elements[j] = elements[j + 1];
                            elements[j + 1] = temp;

                            // Swap in vals array
                            int val = vals[j];
                            vals[j] = vals[j + 1];
                            vals[j + 1] = val;

                            // Since a swap is performed, toggle flag
                            swap = true;
                        }
                        else
                        {
                            await SetColors(elements[j..(j + 2)], this.correctColor, delay);
                        }

                        await SetColors(elements[j..(j + 2)], this.baseColor, 0);
                    }
                    // Finish early if sorted
                    if (!swap) return (timeToSort, vals);
                }

                return (timeToSort, vals);
            }

            // Timed regular sort (also modifies the array it was invoked with, hence sorting the elements)
            public override long TimeSort(ref int[] elements)
            {
                // Start timing
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // Flag to check if the elements are sorted early
                bool swap = false;

                int elementCount = elements.Length;
                // Optimised so that after each pass, the last element in the previous pass is removed from the next pas
                // Therefore reducing the number of passes by 1 per pass
                for (int i = 0; i < elementCount - 1; i++)
                {
                    // Reset swap flag every pass
                    swap = false;
                    for(int j = 0; j < elementCount - i - 1; j++)
                    {
                        double currentVal = elements[j];
                        double nextVal = elements[j + 1];
                        
                        // If next element < current, swap
                        if(nextVal < currentVal)
                        {
                            // Swap elements using a temp variable to prevent overwriting value
                            int temp = elements[j];
                            elements[j] = elements[j+1];
                            elements[j + 1] = temp;

                            // Since a swap is performed, toggle flag
                            swap = true;
                        }
                    }
                    // If no element was swapped at the end of a pass, the elements are sorted and we can break early
                    if (!swap) { sw.Stop(); return sw.ElapsedMilliseconds; }
                }

                sw.Stop();
                return sw.ElapsedMilliseconds; 
            }

        }

        public class InsertionSort : Sort
        {
            // Color vars
            private SolidColorBrush checkColor = new SolidColorBrush(Color.FromRgb(56, 65, 235));
            private SolidColorBrush swapColor = new SolidColorBrush(Color.FromRgb(222, 104, 89));
            private SolidColorBrush correctColor = new SolidColorBrush(Color.FromRgb(143, 204, 102));
            private SolidColorBrush baseColor;

            // Constructor 
            public InsertionSort()
            {
                this.BigO = "O(n^2)";
            }

            // Perform the actual sort
            public override async Task<(long, int[])> SortElements(Rectangle[] elements, int[] vals, Color baseColor, int delay)
            {
                // Ensure valid array 
                if (!isValidArray(elements, vals)) { return (-1, vals); }

                // Save original color
                this.baseColor = new SolidColorBrush(baseColor);

                bool isInvalidWidth = elements[0].Width < 1;
                long timeToSort;

                if (isInvalidWidth)
                {
                    // Sort elements and return without any visualisation
                    timeToSort = TimeSort(ref vals);
                    return (timeToSort, vals);
                }
                else { int[] sortedVals = (int[]) vals.Clone(); timeToSort = TimeSort(ref sortedVals); }

                int elementCount = vals.Length;
                for (int i = 1; i < elementCount; i++)
                {
                    int keyVal = vals[i];
                    Rectangle keyRect = elements[i];

                    double nextPos = Canvas.GetLeft(keyRect); ;

                    // Highlight active element red
                    await SetColors(new Rectangle[] { keyRect }, swapColor, delay);

                    int j = i - 1;

                    // Shift right
                    while (j >= 0 && vals[j] > keyVal)
                    {
                        // Set color of moving element
                        await SetColors(new Rectangle[] { elements[j] }, checkColor, delay);
                        
                        // Shift
                        vals[j + 1] = vals[j];
                        elements[j + 1] = elements[j];

                        // Update visual positions
                        nextPos = Canvas.GetLeft(elements[j]);
                        await SwapElementPos(elements[j], keyRect, delay);

                        // Reset element color
                        await SetColors(new Rectangle[] { elements[j] }, this.baseColor, 0);
                        j--;
                    }

                    // Insert key
                    vals[j + 1] = keyVal;
                    elements[j + 1] = keyRect;

                    // Restore color and pos
                    Canvas.SetLeft(keyRect, nextPos);
                    await SetColors(new Rectangle[] { keyRect }, this.baseColor, delay);
                }

                return (timeToSort, vals);
            }

            // Timed regular sort
            public override long TimeSort(ref int[] vals)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                int elementCount = vals.Length;
                for (int i = 1; i < elementCount; i++)
                {
                    int keyVal = vals[i];
                    int j = i - 1;

                    // Shift right
                    while (j >= 0 && vals[j] > keyVal)
                    {
                        vals[j + 1] = vals[j];
                        j--;
                    }

                    // Insert key
                    vals[j + 1] = keyVal;
                }
                sw.Stop();
                return sw.ElapsedMilliseconds;
            }
        }
    }
}
