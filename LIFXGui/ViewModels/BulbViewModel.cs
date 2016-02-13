using LIFXGui.Commands;
using LIFXSeeSharp;
using LIFXSeeSharp.Bulb;
using System.Windows.Input;

namespace LIFXGui.ViewModels
{
	class BulbViewModel : ViewModelBase
	{
		private LifxController _controller;

		public LifxBulb Bulb
		{
			get { return GetValue(() => Bulb); }
			set { SetValue(() => Bulb, value); }
		}

		public BulbViewModel(LifxBulb b, LifxController controller)
		{
			Bulb = b;
			_controller = controller;

			Bulb.PropertyChanged += Bulb_PropertyChanged;
		}

		protected override void Dispose(bool hardDispose)
		{
			Bulb.PropertyChanged -= Bulb_PropertyChanged;
			base.Dispose(hardDispose);
		}

		private void Bulb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}

		public ICommand PowerCommand
		{
			get
			{
				return new RelayCommand(() =>
				{

				});
			}
		}
	}
}
