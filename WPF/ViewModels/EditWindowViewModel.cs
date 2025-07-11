
using Key_master.WPF.Services;
using Multicad.Wpf.ValidationRules;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Key_master.WPF.ViewModels
{
    internal class EditWindowViewModel : IKeyViewModel
    {
        private RealValueValidationRule _realValueValidationRule = new RealValueValidationRule() {MinIsStrict = true, MinValue = 0};

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
            get 
            {
                return _dialogResult; 
            }
            set
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


        public string Error => throw new NotImplementedException();


        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case nameof(Width):
                        {
                            if (!_realValueValidationRule.Validate(Width, new CultureInfo(CultureInfo.CurrentCulture.Name)).IsValid)
                            {
                                error = "Ввод некорректных значений в текстовое поле!";

                                IsOkButtonEnabled = false;

                                return error;
                            }
                            break;
                        }

                    case nameof(Length):
                        {
                            if (!_realValueValidationRule.Validate(Length, new CultureInfo(CultureInfo.CurrentCulture.Name)).IsValid)
                            {
                                error = "Ввод некорректных значений в текстовое поле!";

                                IsOkButtonEnabled = false;

                                return error;
                            }
                            break;
                        }
                }

                IsOkButtonEnabled = _realValueValidationRule.Validate(Length, new CultureInfo(CultureInfo.CurrentCulture.Name)).IsValid && _realValueValidationRule.Validate(Width, new CultureInfo(CultureInfo.CurrentCulture.Name)).IsValid;

                return error;
            }
        }


        public void Ok()
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
            Width = Convert.ToString(Math.Round(width, 6));

            Length = Convert.ToString(Math.Round(length, 6));

            IsOkButtonEnabled = true;
            //OkCommand = new RelayCommand(o => Ok(), o => !_propertiesWithErrors.Any());
        }
    }
}
