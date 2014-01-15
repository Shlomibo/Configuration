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
	/// <typeparam name="T">The type to convert into</typeparam>
	public interface IParser<out T> : IParser
	{
		/// <summary>
		/// Parses the string into value's type
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <returns>The parsed object</returns>
		new T Parse(string str);
	}

	/// <summary>
	/// Privdes basic parser functionality
	/// </summary>
	/// <typeparam name="T">The type to convert into</typeparam>
	public abstract class ParserBase<T> : IParser<T>
	{
		/// <summary>
		/// Parses the string into value's type
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <returns>The parsed object</returns>
		public abstract T Parse(string str);

		object IParser.Parse(string str)
		{
			return Parse(str);
		}
	}


	/// <summary>
	/// The default type parser
	/// </summary>
	/// <typeparam name="T">The type to parse</typeparam>
	public class DefaultTypeParser<T> : IParser<T>
	{
		/// <summary>
		/// Provides conversions for the basic types
		/// </summary>
		protected static readonly IReadOnlyDictionary<Type, Func<string, object>> Conversions = 
			new Dictionary<Type, Func<string, object>>()
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

		/// <summary>
		/// Parses the string into value's type
		/// </summary>
		/// <param name="str">The string to parse</param>
		/// <returns>The parsed object</returns>
		public virtual T Parse(string str)
		{
			if (typeof(T).IsEnum)
			{
				return (T)Enum.Parse(typeof(T), str, true);
			}
			else if (typeof(T).IsGenericType && 
				(typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)) &&
				Conversions.ContainsKey(Nullable.GetUnderlyingType(typeof(T))))
			{
				return str != null
					? (T)Conversions[Nullable.GetUnderlyingType(typeof(T))](str)
					: default(T);
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
