using LIFXGui.Commands;
using LIFXSeeSharp;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;

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

			if (_controller != null)
			{
				_controller.Dispose();
			}

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

		// Forced to do thise because udp client is blocking at ReceiveAsync, so
		// the VM will never be GC'd it seems
		public ICommand WindowClosing
		{
			get
			{
				return new CommandBase(() =>
				{
					Dispose();
				});

			}
		}
	}
}
