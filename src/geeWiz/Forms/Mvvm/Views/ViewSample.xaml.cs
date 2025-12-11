// System
using System.Windows;

// Using the Mvvm namespace
namespace geeWiz.Forms.Mvvm.Views
{
    /// <summary>
    /// Manages the Mvvm model.
    /// </summary>
    public partial class ViewSample : Window
    {
        // Constructor using view model
        public ViewSample(Models.ModelSample viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}