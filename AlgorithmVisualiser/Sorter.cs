using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using AlgorithmVisualiser.Pages;
using System.Runtime.CompilerServices;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics;

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
        static public List<Page> pages = new List<Page> { new BubbleSortPage() };

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
            double tempPos = Canvas.GetLeft(a);
            Canvas.SetLeft(a, Canvas.GetLeft(b));
            Canvas.SetLeft(b, tempPos);
            await Task.Delay(delay);
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
                if(elements == null || vals == null) { return (-1, vals); }
                if(elements.Length == 1 || vals.Length == 1) { return (-1, vals); }

                this.baseColor = new SolidColorBrush(baseColor);

                long sortTime;

                // If width < 1, no visual sort
                if (elements[0].Width < 1)
                {
                    sortTime = TimeSort(ref vals);
                    return (sortTime, vals);
                }
                else
                {
                    Rectangle[] sorted = elements.ToArray();
                    sortTime = TimeSort(ref vals);
                }

                // Flag to check early sort
                bool swap = false;

                // Sort
                for (int i = 0; i < elements.Length - 1; i++)
                {
                    // Reset swap flag every pass
                    swap = false;

                    for (int j = 0; j < elements.Length - i - 1; j++)
                    {
                        await SetColors(elements[j..(j + 2)], checkColor, delay);

                        double currentVal = elements[j].Height;
                        double nextVal = elements[j + 1].Height;

                        // If next element < current, swap
                        if (nextVal < currentVal)
                        {
                            await SetColors(elements[j..(j + 2)], swapColor, delay);

                            // Swap elements using a temp variable to prevent overwriting value
                            await SwapElementPos(elements[j], elements[j + 1], delay);

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

                        await SetColors(elements[j..(j + 2)], this.baseColor, delay);
                    }

                    if (!swap) return (sortTime, vals);
                }

                return (sortTime,vals);
            }

            // Timed regular sort (also modifies the array it was invoked with, hence sorting the elements)
            public override long TimeSort(ref int[] elements)
            {
                // Start timing
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // Flag to check if the elements are sorted early
                bool swap = false;

                // Optimised so that after each pass, the last element in the previous pass is removed from the next pas
                // Therefore reducing the number of passes by 1 per pass
                for(int i = 0; i < elements.Length - 1; i++)
                {
                    // Reset swap flag every pass
                    swap = false;
                    for(int j = 0; j < elements.Length - i - 1; j++)
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
            // Constructor 
            public InsertionSort()
            {
                this.BigO = "O(n^2)";
            }

            // Perform the actual sort
            public override async Task<(long, int[])> SortElements(Rectangle[] elements, int[] vals, Color baseColor, int delay)
            {
                return (-1, null);
            }

            // Timed regular sort
            public override long TimeSort(ref int[] vals)
            {
                return 0;
            }
        }
    }
}
