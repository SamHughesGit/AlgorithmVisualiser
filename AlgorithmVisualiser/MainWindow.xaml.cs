using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlgorithmVisualiser;

public partial class MainWindow : Window
{

    // Entry point of MainWindow (Constructor)
    public MainWindow()
    {
        InitializeComponent();
        InitializeButtons();
    }

    // Generate buttons for each algorithm 
    private void InitializeButtons()
    {
        // For each pagetype in the page list
        for(int i = Sorter.pages.Count-1; i > -1; i--)
        {
            Button btn = new Button();

            // If button is the last button, ignore adding margin to keep it vertically centered
            btn.Margin = new Thickness(0, 0, 0, i == 0 ? 0 : 10);
            btn.Width = 100;
            btn.Height = Double.NaN; // Makes height automatic

            // Set button text to page title
            btn.Content = Sorter.pages[i].Title;

            // Assign click event to each button, loading algorithm page
            int current = i;
            btn.Click += (sender, e) =>
            {
                PageContent.Content = Sorter.pages[current];
            };

            // Add new button as child of stack-pannel
            ModesContainer.Children.Add(btn);
        }
    }
}