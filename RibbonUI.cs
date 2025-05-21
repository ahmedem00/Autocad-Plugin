using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Private.Windows;
using Autodesk.Windows;
using Dimsion;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Demo00
{
    public class RibbonUI : IExtensionApplication
    {
        public void Initialize()
        {
            try
            {
                RibbonControl ribbon = ComponentManager.Ribbon;
                if (ribbon == null) return;

                // Check if the tab already exists
                RibbonTab tab = ribbon.Tabs.FirstOrDefault(t => t.Id == "MyToolsTab");
                if (tab == null)
                {
                    // Create a new tab if it doesn't exist
                    tab = new RibbonTab();
                    tab.Title = "My Tools";
                    tab.Id = "MyToolsTab";
                    ribbon.Tabs.Add(tab);
                }

                // Check if the panel already exists
                RibbonPanel panel = tab.Panels.FirstOrDefault(p => p.Source.Title == "Grid Tools");
                if (panel == null)
                {
                    // Create a panel if it doesn't exist
                    panel = new RibbonPanel();
                    panel.Source = new RibbonPanelSource();
                    panel.Source.Title = "Grid Tools";
                    tab.Panels.Add(panel);
                }

                // Check if the button already exists
                bool buttonExists = panel.Source.Items.OfType<RibbonButton>().Any(b => b.Id == "CreateGridButton");
                if (!buttonExists)
                {
                    // Create the button only if it doesn't exist
                    RibbonButton btn = new RibbonButton
                    {
                        Text = "Create Grid",
                        ShowText = true,
                        Id = "CreateGridButton",
                        Orientation = System.Windows.Controls.Orientation.Vertical,
                        Size = RibbonItemSize.Large,
                        CommandParameter = "CreateDimension", // This matches your command
                        CommandHandler = new RibbonCommandHandler()
                    };

                    // Add the button to the panel
                    panel.Source.Items.Add(btn);
                }

                // Add debug message to confirm initialization
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nRibbon initialized successfully.");
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nRibbon error: " + ex.Message);
            }
        }

        public void Terminate() { }

        private System.Windows.Media.ImageSource LoadImage(string path)
        {
            var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            return decoder.Frames[0];
        }
    }

    //public class RibbonCommandHandler : System.Windows.Input.ICommand
    //{
    //    public event EventHandler CanExecuteChanged { add { } remove { } }

    //    public bool CanExecute(object parameter) => true;

    //    public void Execute(object parameter)
    //    {
    //        string commandName = parameter as string;
    //        if (!string.IsNullOrEmpty(commandName))
    //        {
    //            Document doc = Application.DocumentManager.MdiActiveDocument;

    //            // Check if a drawing is open
    //            if (doc != null)
    //            {
    //                // Send the command to execute
    //                doc.SendStringToExecute(commandName, true, false, true);
    //            }
    //            else
    //            {
    //                Application.ShowAlertDialog("No drawing is open.");
    //            }
    //        }
    //    }
    //}
    public class RibbonCommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            
            var doc = Application.DocumentManager.MdiActiveDocument;
            string command = 
                parameter as string?? "CreateDimension ";

            if (!string.IsNullOrEmpty(command))
            {
                try
                {
                    // Add debug message
                    //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nAttempting to execute: " + commandName);

                    // Send the command with proper formatting
                    //Application.DocumentManager.MdiActiveDocument.SendStringToExecute(
                    //    "_.CreateDimension", true, false, true);
                    //using (doc.LockDocument())
                    //{
                    //    doc.SendStringToExecute(command, true, false, true);
                    //}

                    var wpfWindow = new MainWindow();
                    Application.ShowModalWindow(wpfWindow);









                }
                catch (System.Exception ex)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nError: " + ex.Message);
                }
            }
        }
    }

}
