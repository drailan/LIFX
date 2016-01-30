using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIFXGui.ViewModels
{
    class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsBusy { get; set; }

        protected void NotifyPropertyChanged(string propertyName)
        {
            var h = PropertyChanged;
            if (h != null)
            {
                h(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        ~ViewModelBase()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
        }
    }
}
