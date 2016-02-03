using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using LIFXSeeSharp;
using System.Windows.Input;
using LIFXGui.Commands;
using System.Reactive.Linq;
using System.Threading;

namespace LIFXGui.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private LifxController _controller;
        private ObservableCollection<BulbViewModel> _bulbs;

        public ObservableCollection<BulbViewModel> Bulbs
        {
            get { return _bulbs; }
            set { _bulbs = value; }
        }

        public MainViewModel()
        {
            _controller = new LifxController();
            _bulbs = new ObservableCollection<BulbViewModel>();

            InitObservableProperties();
        }

        public override void Dispose()
        {
            _controller = null;
            _bulbs.Clear();
            _bulbs = null;
            base.Dispose();
        }

        private void InitObservableProperties()
        {
            _controller.ObserveBulbDiscovery()
                .ObserveOn(SynchronizationContext.Current)
                .Do(b =>
                {
                    _bulbs.Add(new BulbViewModel(b, _controller));
                })
                .Subscribe();
        }

        public void Initialize()
        {
            _controller.RunInitialDiscovery();
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new CommandBase(() =>
                {
                    IsBusy = true;
                    _bulbs.Clear();
                    Initialize();
                });
            }
        }
    }
}
