using System;
using System.ComponentModel;

namespace NextPark.Mobile.UIModels
{
    public class UISelectionItem : INotifyPropertyChanged
    {
        // PRIVATE VARIABLES
        private bool _selected;
        private string _text;
        private int _id;

        // PUBLIC VARIABLES
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged("Text"); } 
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged("Selected"); }
        }

        public int Id 
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
