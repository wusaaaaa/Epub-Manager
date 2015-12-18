using Caliburn.Micro;
using System.Runtime.CompilerServices;

namespace Epub_Manager.Extensions
{
    public static class PropertyChangedBaseExtensions
    {
        public static bool SetProperty<T>(this PropertyChangedBase self, ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(value, field))
                return false;

            field = value;
            self.NotifyOfPropertyChange(propertyName);

            return true;
        }
    }
}