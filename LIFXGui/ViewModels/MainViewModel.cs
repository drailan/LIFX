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
    class MainViewModel
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
                        _bulbs.Clear();
                        Initialize();
                });
            }
        }
    }
}
