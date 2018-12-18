namespace mrBatchSheetExport
{
    using ModPlus;
    using ModPlusAPI;
    using View;
    using ViewModel;

    public class PluginStarter : IRengaFunction
    {
        public void Start()
        {
            Statistic.SendCommandStarting(Interface.Instance);

            MainWindow mainWindow = new MainWindow();
            MainViewModel mainViewModel = new MainViewModel(mainWindow);
            mainWindow.DataContext = mainViewModel;
            mainWindow.ShowDialog();
        }
    }
}
