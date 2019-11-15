namespace mrBatchSheetExport.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Model;
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;
    using ModPlusAPI.Windows;
    using ModPlusStyle.Controls.Dialogs;
    using Renga;
    using View;

    public class MainViewModel : VmBase
    {
        private readonly Renga.Application _rengaApplication;
        private const string LangItem = "mrBatchSheetExport";
        private bool _overwriteExist;
        private readonly MainWindow _mainWindow;
        private string _selectedAutocadFileVersion;
        private int _exportVariant;

        /// <summary>
        /// Empty for editor
        /// </summary>
        public MainViewModel()
        {
        }

        public MainViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _mainWindow.Title = Language.GetFunctionLocalName(LangItem, ModPlusConnector.Instance.LName);
            _rengaApplication = new Renga.Application();
            LoadSettings();
            Drawings = new ObservableCollection<Drawing>();
            ExportCommand = new RelayCommand(Export);
        }

        public void GetDrawings()
        {
            var drawings = new List<Drawing>();
            for (var i = 0; i < _rengaApplication.Project.Drawings.Count; i++)
            {
                var drawing = _rengaApplication.Project.Drawings.Get(i);
                drawings.Add(new Drawing(drawing));
            }

            drawings.Sort((d1, d2) => string.Compare(d1.Name, d2.Name, StringComparison.Ordinal));
            drawings.ForEach(Drawings.Add);
        }

        /// <summary>0 - dwg, 1 - dxf</summary>
        public int ExportVariant
        {
            get => _exportVariant;
            set
            {
                if (Equals(value, _exportVariant))
                    return;
                _exportVariant = value;
                OnPropertyChanged();
            }
        }

        public List<string> AutocadFileVersions => new List<string> { "2000", "2004", "2007", "2010", "2013", "2018" };

        /// <summary>Selected AutoCAD file version</summary>
        public string SelectedAutocadFileVersion
        {
            get => _selectedAutocadFileVersion;
            set
            {
                if (Equals(value, _selectedAutocadFileVersion))
                    return;
                _selectedAutocadFileVersion = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Переписать существующие файлы</summary>
        public bool OverwriteExist
        {
            get => _overwriteExist;
            set
            {
                if (Equals(value, _overwriteExist))
                    return;
                _overwriteExist = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Drawing> Drawings { get; }

        private void LoadSettings()
        {
            _exportVariant = int.TryParse(UserConfigFile.GetValue(LangItem, nameof(ExportVariant)), out var i) ? i : 0;
            var selectedAutocadFileVersion = UserConfigFile.GetValue(
                LangItem, nameof(SelectedAutocadFileVersion));
            _selectedAutocadFileVersion = string.IsNullOrEmpty(selectedAutocadFileVersion)
                ? "2013" : selectedAutocadFileVersion;
            _overwriteExist =
                !bool.TryParse(UserConfigFile.GetValue(LangItem, nameof(OverwriteExist)), out var b) || b; // true
        }

        public void SaveSettings()
        {
            UserConfigFile.SetValue(LangItem, nameof(ExportVariant), ExportVariant.ToString(), true);
            UserConfigFile.SetValue(LangItem, nameof(SelectedAutocadFileVersion), SelectedAutocadFileVersion, true);
            UserConfigFile.SetValue(LangItem, nameof(OverwriteExist), OverwriteExist.ToString(), true);
        }

        public ICommand ExportCommand { get; }

        private async void Export(object o)
        {
            var selectedDrawings = Drawings.Where(d => d.Selected).ToList();
            if (selectedDrawings.Count == 0)
            {
                ModPlusAPI.Windows.MessageBox.Show(
                    Language.GetItem(LangItem, "m1"),
                    ModPlusAPI.Windows.MessageBoxIcon.Alert);
                return;
            }

            if (ModPlusAPI.Windows.MessageBox.ShowYesNo(
                Language.GetItem(LangItem, "m2")))
            {
                _rengaApplication.Project.Save();
            }

            var fbd = new FolderBrowserDialog
            {
                Description = Language.GetItem(LangItem, "h5"),
                SelectedPath = GetSavedLastPath(),
                ShowNewFolderButton = true
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                UserConfigFile.SetValue(LangItem, "LastSelectedPath", fbd.SelectedPath, true);
                ProgressDialogController controller = null;
                try
                {
                    // Создаем контроллер для асинхронного диалога
                    var settings = new MetroDialogSettings
                    {
                        NegativeButtonText = Language.GetItem(LangItem, "h6"),
                        AnimateShow = true,
                        AnimateHide = true,
                        DialogTitleFontSize = 20
                    };
                    controller = await _mainWindow.ShowProgressAsync(
                        ExportVariant == 0
                            ? Language.GetItem(LangItem, "h7") + " dwg"
                            : Language.GetItem(LangItem, "h7") + " dxf",
                        string.Empty, true, settings);
                    controller.Minimum = 1;
                    controller.Maximum = selectedDrawings.Count;
                    for (var i = 0; i < selectedDrawings.Count; i++)
                    {
                        if (controller.IsCanceled)
                            break;
                        var selectedDrawing = selectedDrawings[i];
                        controller.SetMessage(Language.GetItem(LangItem, "h8") + selectedDrawing.Name);
                        var fileName = GetFileNameForNewFile(fbd.SelectedPath, selectedDrawing);
                        if (ExportVariant == 0)
                        {
                            selectedDrawing.SourceIDrawing.ExportToDwg(
                                fileName,
                                GetAutocadFileVersionEnum(SelectedAutocadFileVersion),
                                OverwriteExist);
                        }
                        else
                        {
                            selectedDrawing.SourceIDrawing.ExportToDxf(
                                fileName,
                                GetAutocadFileVersionEnum(SelectedAutocadFileVersion),
                                OverwriteExist);
                        }

                        await Task.Delay(100);
                        controller.SetProgress(i + 1);
                    }
                }
                catch (Exception exception)
                {
                    ExceptionBox.Show(exception);
                }
                finally
                {
                    if (controller != null)
                        await controller.CloseAsync();
                }
            }
        }

        private AutocadVersion GetAutocadFileVersionEnum(string fv)
        {
            switch (fv)
            {
                case "2000": return AutocadVersion.AutocadVersion_v2000;
                case "2004": return AutocadVersion.AutocadVersion_v2004;
                case "2007": return AutocadVersion.AutocadVersion_v2007;
                case "2010": return AutocadVersion.AutocadVersion_v2010;
                case "2013": return AutocadVersion.AutocadVersion_v2013;
            }

            return AutocadVersion.AutocadVersion_v2018;
        }

        private string GetSavedLastPath()
        {
            var savedPath = UserConfigFile.GetValue(
                LangItem, "LastSelectedPath");
            if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
                return savedPath;
            var projectFilePath = _rengaApplication.Project.FilePath;
            if (File.Exists(projectFilePath))
            {
                return new FileInfo(projectFilePath).DirectoryName;
            }

            return string.Empty;
        }

        private string GetFileNameForNewFile(string path, Drawing drawing)
        {
            var extension = ExportVariant == 0 ? ".dwg" : "dxf";

            var fileName = Path.Combine(path, drawing.Name + extension);
            if (!OverwriteExist)
            {
                var i = 1;
                while (File.Exists(fileName))
                {
                    fileName = Path.Combine(path, drawing.Name + "_" + i + extension);
                    i++;
                }
            }

            return fileName;
        }
    }
}
