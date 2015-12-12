using System;
using System.ComponentModel;
using System.Text;

namespace LIFXSeeSharp
{
	public class LifxBulb : INotifyPropertyChanged
	{
        private string _group;
        private string _label;
        private float _hue;
        private float _saturation;        
        private float _brightness;
        private ushort _kelvin;
        private ushort _dim;
        private ushort _power;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            var h = PropertyChanged;
            if (h != null)
            {
                h(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Group
        {
            get { return _group; }
            set
            {
                if (_group != value)
                {
                    _group = value;
                    NotifyPropertyChanged("Group");
                }
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }

        public float Hue
        {
            get { return _hue; }
            set
            {
                if (_hue != value)
                {
                    _hue = value;
                    NotifyPropertyChanged("Hue");
                }
            }
        }

        public float Saturation
        {
            get { return _saturation; }
            set
            {
                if (_saturation != value)
                {
                    _saturation = value;
                    NotifyPropertyChanged("Saturation");
                }
            }
        }
        public float Brightness
        {
            get { return _brightness; }
            set
            {
                if (_brightness != value)
                {
                    _brightness = value;
                    NotifyPropertyChanged("Brightness");
                }
            }
        }

        public ushort Kelvin
        {
            get { return _kelvin; }
            set
            {
                if (_kelvin != value)
                {
                    _kelvin = value;
                    NotifyPropertyChanged("Kelvin");
                }
            }
        }

        public ushort Dim
        {
            get { return _dim; }
            set
            {
                if (_dim != value)
                {
                    _dim = value;
                    NotifyPropertyChanged("Dim");
                }
            }
        }
        public ushort Power
        {
            get { return _power; }
            set
            {
                if (_power != value)
                {
                    _power = value;
                    NotifyPropertyChanged("Power");
                }
            }
        }

        public LifxBulb(string label = "")
		{
			Label = label;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Label: {0}\n\tHue: {1}\n\tSaturation: {2}\n\tBrightNess: {3}\n\tKelvin: {4}\n\tDim: {5}\n\tPower: {6}\n",
				Label, Hue, Saturation, Brightness, Kelvin, Dim, Power);
			return sb.ToString();
		}

        public override bool Equals(object obj)
        {
            var bulb = obj as LifxBulb;
            if (bulb != null)
            {
                return Label == bulb.Label;
            }

            return false;            
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }
    }
}