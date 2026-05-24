// System
using System.Windows;

// Using the Mvvm namespace
namespace geeWiz.Forms.Mvvm.Views
{
    /// <summary>
    /// The View of the MVVM Sample system.
    /// </summary>
    public partial class ViewSample : Window
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="viewModel">The related Model.</param>
        public ViewSample(Models.ModelSample viewModel)
        {
            InitializeComponent();
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.DataContext = viewModel;
        }
    }
}