using System;
using System.Linq.Expressions;

namespace LIFXGui.Extensions
{
	public static class ExpressionExtensions
	{
		public static string GetPropertyName<T>(this Expression<Func<T>> expression)
		{
			var me = expression.Body as MemberExpression;

			if (me == null)
			{
				throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
			}

			return me.Member.Name;
		}
	}
}
