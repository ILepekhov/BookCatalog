using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shared.Binding
{
    public abstract class BaseBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T target, T value, [CallerMemberName] string propertyName = "")
        {
            target = value;
            OnPropertyChanged(propertyName);
        }
    }
}
