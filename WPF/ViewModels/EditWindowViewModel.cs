
using Key_master.WPF.Services;
using Key_master.WPF.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Key_master.WPF.ViewModels
{
    internal class EditWindowViewModel : IKeyViewModel
    {
        private string ?_width;
        public string ?Width 
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;

                OnPropertyChanged();
            }
        }


        private string ?_length;
        public string? Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;

                OnPropertyChanged();
            }
        }


        private bool _isOkButtonEnabled;
        public bool IsOkButtonEnabled
        {
            get
            {
                return _isOkButtonEnabled;
            }
            set
            {
                _isOkButtonEnabled = value;
                OnPropertyChanged();
            }
        }


        private bool? _dialogResult;

        public bool? DialogResult
        {
            get { return _dialogResult; }
            protected set
            {
                _dialogResult = value;
                OnPropertyChanged("DialogResult");
            }
        }


        public RelayCommand OkCommand 
        {
            get
            {
                return new RelayCommand((obj) => Ok());
            }
                
        }

        private void Ok()
        {
            DialogResult = true;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public EditWindowViewModel(double width,double length)
        {
            Width = Convert.ToString(width);

            Length = Convert.ToString(length);
        }
    }
}
