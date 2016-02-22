using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace LIFXGui.Extensions
{
	public static class INotifyPropertyChangedExtensions
	{
		public static IObservable<Unit> ObserveProperty<TSource, TValue>(this TSource source, Expression<Func<TValue>> expression)
			where TSource : INotifyPropertyChanged
		{
			var me = expression.Body as MemberExpression;

			if (me == null)
			{
				throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
			}

			return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					handler => handler.Invoke,
					h => source.PropertyChanged += h,
					h => source.PropertyChanged -= h)
				.Where(e => e.EventArgs.PropertyName == me.Member.Name)
				.Select(_ => Unit.Default);
		}
	}
}
