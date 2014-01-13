using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Provides parsing functionality for named values
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// Parses the string into value's type
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <returns>The parsed object</returns>
		object Parse(string str);
	}

	/// <summary>
	/// Provides parsing functionality for named values
	/// </summary>
	public interface IParser<out T> : IParser
	{
		/// <summary>
		/// Parses the string into value's type
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <returns>The parsed object</returns>
		T Parse(string str);
	}

	public class DefaultTypeParse<T> : IParser<T>
	{
		protected static readonly IReadOnlyDictionary<Type, Func<string, object>> Conversions = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(object), str => str },
			{ typeof(string), str => str },
			{ typeof(bool), str => bool.Parse(str) },
			{ typeof(byte), str => byte.Parse(str) },
			{ typeof(sbyte), str => sbyte.Parse(str) },
			{ typeof(char), str => char.Parse(str) },
			{ typeof(decimal), str => decimal.Parse(str) },
			{ typeof(double), str => double.Parse(str) },
			{ typeof(float), str => float.Parse(str) },
			{ typeof(int), str => int.Parse(str) },
			{ typeof(uint), str => uint.Parse(str) },
			{ typeof(long), str => long.Parse(str) },
			{ typeof(ulong), str => ulong.Parse(str) },
			{ typeof(short), str => short.Parse(str) },
			{ typeof(ushort), str => ushort.Parse(str) },
			{ typeof(Uri), str => new Uri(str) },
			{ typeof(DateTime), str => DateTime.Parse(str) },
			{ typeof(Guid), str => new Guid(str) },
			{ typeof(Type), str => Type.GetType(str) },
		};

		public virtual T Parse(string str)
		{
			if (typeof(T).IsEnum)
			{
				return (T)Enum.Parse(typeof(T), str);
			}
			else if (Conversions.ContainsKey(typeof(T)))
			{
				return (T)Conversions[typeof(T)](str);
			}
			else
			{
				throw new InvalidOperationException("Unsupported type for parsing");
			}
		}

		object IParser.Parse(string str)
		{
			return Parse(str);
		}
	}

}
