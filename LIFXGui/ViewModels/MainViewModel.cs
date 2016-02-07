using LIFXGui.Commands;
using LIFXGui.Extensions;
using LIFXSeeSharp;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

namespace LIFXGui.ViewModels
{
	class MainViewModel : ViewModelBase
	{
		private LifxController _controller;

		public ObservableCollection<BulbViewModel> Bulbs
		{
			get { return GetValue(() => Bulbs); }
			set { SetValue(() => Bulbs, value); }
		}

		public bool IsInitialized
		{
			get { return GetValue(() => IsInitialized); }
			set { SetValue(() => IsInitialized, value); }
		}

		public MainViewModel()
		{
			_controller = new LifxController();
			Bulbs = new ObservableCollection<BulbViewModel>();

			InitObservableProperties();
		}

		protected override void Dispose(bool hardDispose)
		{
			if (_controller != null)
			{
				_controller.Dispose();
			}

			base.Dispose(hardDispose);
		}

		private void InitObservableProperties()
		{
			_controller.ObserveBulbDiscovery()
				.ObserveOnDispatcher()
				.Do(b =>
				{
					Bulbs.Add(new BulbViewModel(b, _controller));
					IsInitialized = true;
				})
				.Subscribe()
				.DisposeWith(Disposable);
		}

		private void GetLightStates()
		{

		}

		public ICommand RefreshCommand
		{
			get
			{
				return new CommandBase(() =>
				{
					Bulbs.Clear();
					_controller.RunInitialDiscovery();
				})
				.CanExecute(() => { return true; });
			}
		}

		public ICommand LightStateCommand
		{
			get
			{
				return new CommandBase(() =>
				{
					_controller.GetLightStates();
				})
				.CanExecute(() => { return IsInitialized; }) ;
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
