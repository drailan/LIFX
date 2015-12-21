using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LIFXSeeSharp;
using System.Windows.Input;
using LIFXGui.Commands;

namespace LIFXGui.ViewModels
{
    class MainViewModel
    {
        private LifxController _controller;
        private ObservableCollection<BulbViewModel> _bulbs;
        private bool _isExecuting;

        public ObservableCollection<BulbViewModel> Bulbs
        {
            get { return _bulbs; }
            set { _bulbs = value; }
        }

        public MainViewModel()
        {
            _controller = new LifxController();
            _bulbs = new ObservableCollection<BulbViewModel>();

            //Task t = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await _controller.RunInitialDiscovery();

            _controller.Bulbs.ForEach(b =>
            {
                Console.WriteLine("----------------------------------");
                Console.WriteLine(b);
                _bulbs.Add(new BulbViewModel(b, _controller));
            });

            await _controller.GetLightState();
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new CommandBase(() =>
                {
                    if (!_isExecuting)
                    {
                        _isExecuting = true;
                        _bulbs.Clear();
                        var t = InitializeAsync();
                        _isExecuting = false;
                    }
                });
            }
        }
    }
}
