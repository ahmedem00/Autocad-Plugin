using Autodesk.AutoCAD.ApplicationServices;
using Demo00;
using System;
using System.Collections.Generic;
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


namespace Dimsion
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AutocadApi _autocadApi;
        private readonly Document _doc;

        public MainWindow()
        {
            InitializeComponent();
            _autocadApi = new AutocadApi();
            _doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(xAxisInput.Text, out int xCount) && int.TryParse(yAxisInput.Text, out int yCount)&& float.TryParse(spacingInput.Text, out float spacing)&& !string.IsNullOrWhiteSpace(layerNameInput.Text))
                {
                    Environment.SetEnvironmentVariable("GRID_X_COUNT", xCount.ToString());
                    Environment.SetEnvironmentVariable("GRID_Y_COUNT", yCount.ToString());
                    Environment.SetEnvironmentVariable("GRID_SPACING", spacing.ToString());
                    Environment.SetEnvironmentVariable("GRID_LAYER_NAME", layerNameInput.Text);
                    _doc.SendStringToExecute("CreateDimension ", true, false, true);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter valid numbers for X and Y axis counts.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _doc.Editor.WriteMessage($"\nError: {ex.Message}");
            }
        }
    }
}
