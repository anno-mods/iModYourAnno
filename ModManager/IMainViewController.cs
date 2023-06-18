using System.Windows.Controls;

namespace Imya.UI
{
    public interface IMainViewController
    {
        public delegate void ViewChangedEventHandler(View view);
        public event ViewChangedEventHandler ViewChanged;

        void SetView(View view);
        void GoToLastView();
        View GetView();
    }
}
