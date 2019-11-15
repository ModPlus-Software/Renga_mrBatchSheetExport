namespace mrBatchSheetExport
{
    using ModPlus;
    using ModPlusAPI;
    using View;
    using ViewModel;

    /// <inheritdoc />
    public class PluginStarter : IRengaFunction
    {
        /// <inheritdoc />
        public void Start()
        {
            Statistic.SendCommandStarting(ModPlusConnector.Instance);

            var mainWindow = new MainWindow();
            var mainViewModel = new MainViewModel(mainWindow);
            mainWindow.DataContext = mainViewModel;
            mainWindow.ShowDialog();
        }
    }
}
