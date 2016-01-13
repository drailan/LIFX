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
    class BulbViewModel : ViewModelBase
    {
        private LifxBulb _bulb;
        private LifxController _controller;

        public LifxBulb Bulb
        {
            get { return _bulb; }
            set { _bulb = value; }
        }

        public BulbViewModel(LifxBulb b, LifxController controller)
        {
            Bulb = b;
            _controller = controller;

            _bulb.PropertyChanged += _bulb_PropertyChanged;
        }

        private void _bulb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        public string Group
        {
            get { return _bulb.Group; }
            set
            {
                if (_bulb.Group != value)
                {
                    _bulb.Group = value;
                    NotifyPropertyChanged("Group");
                }
            }
        }

        public string Label
        {
            get { return _bulb.Label; }
            set {
                if (_bulb.Label != value) {
                    _bulb.Label = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }

        public float Hue
        {
            get { return _bulb.Hue; }
            set {
                if (_bulb.Hue != value)
                {
                    _bulb.Hue = value;
                    NotifyPropertyChanged("Hue");
                }
            }
        }

        public float Saturation
        {
            get { return _bulb.Saturation; }
            set
            {
                if (_bulb.Saturation != value)
                {
                    _bulb.Saturation = value;
                    NotifyPropertyChanged("Saturation");
                }
            }
        }
        public float Brightness
        {
            get { return _bulb.Brightness; }
            set
            {
                if (_bulb.Brightness != value)
                {
                    _bulb.Brightness = value;
                    NotifyPropertyChanged("Brightness");
                }
            }
        }

        public int Kelvin
        {
            get { return (int) _bulb.Kelvin; }
            set {
                if (_bulb.Kelvin != value)
                {
                    _bulb.Kelvin = (ushort) value;
                    NotifyPropertyChanged("Kelvin");
                }
            }
        }

        public ushort Dim
        {
            get { return _bulb.Dim; }
            set
            {
                if (_bulb.Dim != value)
                {
                    _bulb.Dim = value;
                    NotifyPropertyChanged("Dim");
                }
            }
        }
        public ushort Power
        {
            get { return _bulb.Power; }
            set
            {
                if (_bulb.Power != value)
                {
                    _bulb.Power = value;
                    NotifyPropertyChanged("Power");
                }
            }
        }

        public ICommand PowerCommand
        {
            get
            {
                return new CommandBase(() => {
                    var power = (ushort) ((Power > 0) ? 0 : 1);
                    var newPower = _controller.SetPower(power, Label);
                    Power = power;
                    //newPower.Wait();
                    //Power = newPower.Result;
                });
            }
        }
    }
}
