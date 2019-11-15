namespace mrBatchSheetExport.Model
{
    using ModPlusAPI.Mvvm;
    using Renga;

    public class Drawing : VmBase
    {
        public IDrawing SourceIDrawing;

        public Drawing(IDrawing sourceIDrawing)
        {
            SourceIDrawing = sourceIDrawing;
            Name = sourceIDrawing.Name;
        }

        /// <summary>
        /// Name of sourceIDrawing
        /// </summary>
        public string Name { get; }

        private bool _selected = true;

        /// <summary>Is selected</summary>
        public bool Selected
        {
            get => _selected;
            set
            {
                if (Equals(value, _selected))
                    return;
                _selected = value;
                OnPropertyChanged();
            }
        }
    }
}
