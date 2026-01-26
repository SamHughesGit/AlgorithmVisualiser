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

namespace AlgorithmVisualiser
{
    // Base clas sorters derive from
    public abstract class Sort
    {
        public string BigO { get; set; }
        public abstract void SortElements(List<Rectangle> elements);

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
        }
    }
}
