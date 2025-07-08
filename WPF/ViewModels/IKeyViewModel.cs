
using Key_master.WPF.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Key_master.WPF.ViewModels
{
    internal interface IKeyViewModel : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName] string prop = "");

        string Width { get; set; }

        string Length { get; set; }

        bool IsOkButtonEnabled { get; set; }

        RelayCommand OkCommand { get; }

        void Ok();

        bool? DialogResult { get; set; }
    }
}
