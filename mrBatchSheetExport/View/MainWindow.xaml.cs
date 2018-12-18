namespace mrBatchSheetExport.View
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Model;
    using ViewModel;

    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            ((MainViewModel)DataContext).GetDrawings();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = SizeToContent.Manual;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ((MainViewModel)DataContext).SaveSettings();
        }

        private void BtSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in LbDrawings.Items)
            {
                if (item is Drawing drawing)
                    drawing.Selected = true;
            }
        }

        private void BtUnselectAll_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in LbDrawings.Items)
            {
                if (item is Drawing drawing)
                    drawing.Selected = false;
            }
        }

        private void BtReverseSelection_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in LbDrawings.Items)
            {
                if (item is Drawing drawing)
                    drawing.Selected = !drawing.Selected;
            }
        }
    }
}
