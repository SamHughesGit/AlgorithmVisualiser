using AlgorithmVisualiser.Pages;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AlgorithmVisualiser
{
    // Base class sorters derive from
    public abstract class Sort
    {
        public string BigO { get; set; }
        public abstract Task<(long, int[])> SortElements(Rectangle[] elements, int[] vals, Color baseColor, int delay);
        public abstract long TimeSort(ref int[] vals);

    }

    // Base class for searchers derive from
    public abstract class Search
    {
        public string BigO { get; set; }
        public abstract Task<(long, int)> SearchElements(Rectangle[] elements, int[] vals, Color baseColor, int delay, int target);
        public abstract (long, int) TimeSearch(int[] vals, int target);
    }

    static class Sorter
    {
        // List of pages for dynamically creating buttons
        static public List<Page> pages = new List<Page> { new BubbleSortPage(), new InsertionSortPage(), new QuickSortPage() };

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

            bool AreInPosition() => (Math.Abs(aPos - aDesired) <= step && Math.Abs(bPos - bDesired) <= step);
            bool shouldAnimate = delay > steps;

            while (!AreInPosition() && shouldAnimate)
            {
                aPos = Canvas.GetLeft(a);
                bPos = Canvas.GetLeft(b);
                Canvas.SetLeft(a, aPos + (aPos < aDesired?step:-step));
                Canvas.SetLeft(b, bPos + (bPos < bDesired?step:-step));
                await Task.Delay(wait);
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

        public static class Util
        {
            static public bool isValidArray(Rectangle[] rects, int[] ints)
            {
                bool isInvalidArray = (rects == null || ints == null);
                if (isInvalidArray) return false;
                bool isArrayShort = (rects.Length == 1 || ints.Length == 1);
                if (isArrayShort) return false;
                return true;
            }
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
                if(!Util.isValidArray(elements,vals)) { return (-1, vals); }
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

                            // Swap elements visual position
                            await SwapElementPos(elements[j+1], elements[j], delay);

                            // Swap in rect array
                            (elements[j], elements[j + 1]) = (elements[j + 1], elements[j]);

                            // Swap in vals array
                            (vals[j], vals[j + 1]) = (vals[j + 1], vals[j]);


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
                            (elements[j], elements[j + 1]) = (elements[j + 1], elements[j]);


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
                if (!Util.isValidArray(elements, vals)) { return (-1, vals); }
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
                        await SetColors(new Rectangle[] { elements[j] }, checkColor, 0);
                        
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
                    await SetColors(new Rectangle[] { keyRect }, this.baseColor, 0);
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

        public class QuickSort : Sort
        {
            private SolidColorBrush pivotColor = new SolidColorBrush(Colors.Gold);
            private SolidColorBrush checkColor = new SolidColorBrush(Color.FromRgb(56, 65, 235));
            private SolidColorBrush swapColor = new SolidColorBrush(Color.FromRgb(222, 104, 89));
            private SolidColorBrush baseColor;

            public QuickSort()
            {
                this.BigO = "O(n log n)";
            }

            public override async Task<(long, int[])> SortElements(Rectangle[] elements, int[] vals, Color baseColor, int delay)
            {
                if (!Util.isValidArray(elements, vals)) return (-1, vals);
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
                else { int[] sortedVals = (int[])vals.Clone(); timeToSort = TimeSort(ref sortedVals); }

                await QuickSortVisual(elements, vals, 0, vals.Length - 1, delay);

                await SetColors(elements, this.baseColor, 0);
                return (timeToSort, vals);
            }

            private async Task QuickSortVisual(Rectangle[] elements, int[] vals, int left, int right, int delay)
            {
                if(left < right)
                {
                    int pivot = await PartitionVisual(elements, vals, left, right, delay);

                    await QuickSortVisual(elements, vals, left, pivot - 1, delay);
                    await QuickSortVisual(elements, vals, pivot + 1, right, delay);
                }
            }

            private async Task<int> PartitionVisual(Rectangle[] elements, int[] vals, int left, int right, int delay)
            {
                int pivot = vals[right];
                Rectangle pivotRect = elements[right];
                await SetColors(new Rectangle[] { pivotRect }, pivotColor, delay);

                int i = left - 1;

                for (int j = left; j < right; j++)
                {
                    await SetColors(new Rectangle[] { elements[j] }, checkColor, delay);

                    if (vals[j] < pivot)
                    {
                        i++;
                        await SetColors(new Rectangle[] { elements[i], elements[j] },  swapColor, delay);

                        // Swap visual positions
                        await SwapElementPos(elements[i], elements[j], delay);

                        // Swap array positions using tuples
                        (elements[i], elements[j]) = (elements[j], elements[i]);
                        (vals[i], vals[j]) = (vals[j], vals[i]);
                    }
                    await SetColors(new Rectangle[] { elements[j] }, this.baseColor, 0);
                }

                // Place pivot into correct position
                await SetColors(new Rectangle[] { elements[i + 1], elements[right] }, swapColor, delay);
                await SwapElementPos(elements[i + 1], elements[right], delay);

                (elements[i + 1], elements[right]) = (elements[right], elements[i + 1]);
                (vals[i + 1], vals[right]) = (vals[right], vals[i + 1]);

                await SetColors(elements[left..(right+1)], this.baseColor, 0);
                return i + 1;
            }

            public override long TimeSort(ref int[] vals)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                QuickSortTime(vals, 0, vals.Length - 1);
                sw.Stop();
                return sw.ElapsedMilliseconds;
            }

            private void QuickSortTime(int[] vals, int left, int right)
            {
                if(left < right)
                {
                    int pivot = Partition(vals, left, right);
                    QuickSortTime(vals, left, pivot - 1);
                    QuickSortTime(vals, pivot + 1, right);
                }
            }

            private int Partition(int[] vals, int left, int right)
            {
                int pivot = vals[right];
                int i = left - 1;
                for(int j = left; j < right; j++)
                {
                    if (vals[j] < pivot)
                    {
                        i++;
                        // Swap using a tuple
                        (vals[i], vals[j]) = (vals[j], vals[i]);
                    }
                }
                (vals[i+1] , vals[right]) = (vals[right], vals[i+1]);
                return i + 1;
            }
        }

        public class LinearSearch : Search
        {
            private SolidColorBrush checkColor = new SolidColorBrush(Color.FromRgb(56, 65, 235));
            private SolidColorBrush correctColor = new SolidColorBrush(Color.FromRgb(143, 204, 102));
            private SolidColorBrush baseColor;

            public LinearSearch()
            {
                this.BigO = "O(n)";
            }

            public override async Task<(long, int)> SearchElements(Rectangle[] elements, int[] vals, Color baseColor, int delay, int target) 
            {
                // If either array is invalid, return now
                if (!Util.isValidArray(elements, vals)) return (-1, -1);
                bool isInvalidWidth = elements[0].Width < 1;
                this.baseColor = new SolidColorBrush(baseColor);
                bool isIteminArray = false;
                int timeToFind = 0;

                (long, int) found;

                // If invalid width, find without visuals
                if (isInvalidWidth)
                {
                    found = TimeSearch(vals, target);
                    return found;
                }
                else
                {
                    found = TimeSearch(vals, target);
                }

                // Iterate over each rectangle
                for (int i = 0; i < elements.Length; i++)
                {
                    Rectangle currentRect = elements[i];
                    // Set to check color
                    currentRect.Fill = checkColor;
                    await Task.Delay(delay); // Artificial delay

                    // If element is the target, highlight new color and return, otherwise reset color and move on
                    if (vals[i] == target)
                    {
                        currentRect.Fill = correctColor;
                        return found;
                    }

                    currentRect.Fill = this.baseColor;
                }

                // If item not in array, return;
                return found;
            }

            // Time search, return -1 if not found
            public override (long, int) TimeSearch(int[] vals, int target)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for(int i = 0; i < vals.Length; i++)
                {
                    if (vals[i] == target)
                    {
                        sw.Stop();
                        return (sw.ElapsedMilliseconds, i);
                    }
                }
                sw.Stop();
                return (-1, -1);
            }
        }
    }
}
