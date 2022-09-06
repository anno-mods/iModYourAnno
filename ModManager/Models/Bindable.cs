using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Imya.UI.Models
{
    public class Bindable<TModel> : INotifyPropertyChanged where TModel: class
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public TModel Model { get; init; }

        protected readonly DispatcherObject _context;

        public Bindable(TModel model, DispatcherObject context)
        {
            Model = model;
            _context = context;

            // use weak events as we do not own the model and don't want the hassle to figure out when to remove it
            if (Model is INotifyPropertyChanged notifying)
                WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(notifying, nameof(PropertyChanged), (s, e) => NotifyDispatched(e));
        }

        protected void NotifyDispatched(string propertyName)
        {
            NotifyDispatched(new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyDispatched(PropertyChangedEventArgs e)
        {
            if (_context.Dispatcher.CheckAccess())
            {
                PropertyChanged?.Invoke(this, e);
            }
            else
            {
                _context.Dispatcher.Invoke(() =>
                {
                    PropertyChanged?.Invoke(this, e);
                });
            }
        }
    }
}
