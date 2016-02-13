using LIFXGui.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

// Taken from Indeed & adapted

namespace LIFXGui.ViewModels
{
	class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		private readonly IDictionary<string, object> repository = new Dictionary<string, object>();

		public CompositeDisposable HardDisposable { get; protected internal set; }

		public CompositeDisposable Disposable { get; protected internal set; }

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

		public ViewModelBase()
		{
			HardDisposable = new CompositeDisposable();
			Disposable = new CompositeDisposable();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool hardDispose)
		{
			SoftDispose();

			if (!hardDispose) { return; }

			HardDispose();
		}

		private void SoftDispose()
		{
			Scheduler.Schedule(DispatcherScheduler.Current, () => { Disposable.Dispose(); });
		}

		private void HardDispose()
		{
			repository.Clear();
			Scheduler.Schedule(DispatcherScheduler.Current, () => { HardDisposable.Dispose(); });
			GC.Collect();
		}

		protected TType GetValue<TType>(Expression<Func<TType>> key, Func<TType> factory = null)
		{
			return GetValue(key.GetPropertyName(), factory);
		}

		protected TType GetValue<TType>(string key, Func<TType> factory = null)
		{
			if (!repository.ContainsKey(key))
			{
				repository[key] = factory != null ? factory() : default(TType);
				NotifyPropertyChanged(key);
			}
			return (TType)repository[key];
		}

		protected void SetValue<TType>(Expression<Func<TType>> key, TType value)
		{
			SetValue(key.GetPropertyName(), value);
		}

		protected void SetValue<TType>(string key, TType value)
		{
			repository[key] = value;
			NotifyPropertyChanged(key);
			Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = key });
		}
	}
}
