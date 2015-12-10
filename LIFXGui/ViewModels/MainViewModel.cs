using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIFXSeeSharp;

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

            Task t = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await Task.Run(() => _controller.RunInitialDiscovery());

            _controller.Bulbs.ForEach(b =>
            {
                _bulbs.Add(new BulbViewModel(b));
            });

            await Task.Run(() => _controller.GetLightState());
        }
    }
}
