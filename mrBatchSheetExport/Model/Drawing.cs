namespace mrBatchSheetExport.Model
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Renga;

    public class Drawing : INotifyPropertyChanged
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
                if (Equals(value, _selected)) return;
                _selected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
