namespace mrBatchSheetExport
{
    using System.Collections.Generic;
    using ModPlusAPI.Abstractions;
    using ModPlusAPI.Enums;

    /// <inheritdoc/>
    public class ModPlusConnector : IModPlusPluginForRenga
    {
        private static ModPlusConnector _instance;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ModPlusConnector Instance => _instance ?? (_instance = new ModPlusConnector());

        /// <inheritdoc/>
        public SupportedProduct SupportedProduct => SupportedProduct.Renga;

        /// <inheritdoc/>
        public string Name => "mrBatchSheetExport";

        /// <inheritdoc/>
        public RengaFunctionUILocation UiLocation => RengaFunctionUILocation.PrimaryPanel;

        /// <inheritdoc/>
        public RengaContextMenuShowCase ContextMenuShowCase => RengaContextMenuShowCase.Scene;

        /// <inheritdoc/>
        public List<RengaViewType> ViewTypes => new List<RengaViewType>();

        /// <inheritdoc/>
        public bool IsAddingToMenuBySelf => false;

        /// <inheritdoc/>
        public string LName => "Пакетный экспорт листов";

        /// <inheritdoc/>
        public string Description => "Функция позволяет экспортировать все указанные листы в dwg/dxf";

        /// <inheritdoc/>
        public string Author => "Пекшев Александр aka Modis";

        /// <inheritdoc/>
        public string Price => "0";

        /// <inheritdoc/>
        public string FullDescription => string.Empty;
    }
}