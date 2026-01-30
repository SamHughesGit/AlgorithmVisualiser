using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using AlgorithmVisualiser.Pages;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.ComponentModel;

namespace AlgorithmVisualiser
{
    // Base class sorters derive from
    public abstract class Sort
    {
        public string BigO { get; set; }
        public abstract void SortElements(List<Rectangle> elements);
        public abstract long TimeSort(List<Rectangle> elements);

    }

    static class Sorter
    {
        // List of pages for dynamically creating buttons
        static public List<Page> pages = new List<Page> { new BubbleSortPage() };

        class BubbleSort : Sort
        {
            // Constructor
            public BubbleSort()
            {
                this.BigO = "O(n^2)";
            }

            // Perform the actual sort
            public override void SortElements(List<Rectangle> elements)
            {
                // Get time to sort without animations or UI delay
                long sortTime = TimeSort(elements);

                
            }

            // Timed regular sort
            public override long TimeSort(List<Rectangle> elements)
            {
                // Store time in ms to time the sort
                long startTime = DateTime.UtcNow.Millisecond;

                // Flag to check if the elements are sorted early
                bool swap = false;

                // Optimised so that after each pass, the last element in the previous pass is removed from the next pas
                // Therefore reducing the number of passes by 1 per pass
                for(int i = 0; i < elements.Count - 1; i++)
                {
                    // Reset swap flag every pass
                    swap = false;
                    for(int j = 0; j < elements.Count - i - 1; j++)
                    {
                        int currentVal = elements[j].Height;
                        int nextVal = elements[j + 1].Height;
                        
                        // If next element < current, swap
                        if(nextVal < currentVal)
                        {
                            // Swap elements using a temp variable to prevent overwriting value
                            Rectangle temp = elements[j];
                            elements[j] = elements[j+1];
                            elements[j + 1] = temp;

                            // Since a swap is performed, toggle flag
                            swap = true;
                        }
                    }
                    // If no element was swapped at the end of a pass, the elements are sorted and we can break early
                    if (!swap) return DateTime.UtcNow.Millisecond - startTime;
                } 

                // Time when algorithm finished (ms) subtract start time (ms) = time taken (ms)
                return DateTime.UtcNow.Millisecond - startTime;
            }

        }

        class InsertionSort : Sort
        {
            // Constructor 
            public InsertionSort()
            {
                this.BigO = "O(n^2)";
            }

            // Perform the actual sort
            public override void SortElements(List<Rectangle> elements)
            {

            }

            // Timed regular sort
            public override long TimeSort(List<Rectangle> elements)
            {
                return 0;
            }
        }
    }
}
