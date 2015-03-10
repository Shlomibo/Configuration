using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// A named value that contains some data under some name
	/// </summary>
	public interface INamedValue
	{
		/// <summary>
		/// Gets the name of the value
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the value's data
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// Gets list of values
		/// </summary>
		IList Values { get; }

		/// <summary>
		/// Get value indicates if the value has been set
		/// </summary>
		bool IsValueSet { get; }

		/// <summary>
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </summary>
		bool IsNameVisible { get; set; }

		/// <summary>
		/// Parses the string
		/// </summary>
		/// <param name="valueString">String representation of the value</param>
		void ParseString(string valueString);

		/// <summary>
		/// Adds the parsed string to the values list
		/// </summary>
		/// <param name="valueString"></param>
		void AddParsedString(string valueString);

		/// <summary>
		/// Gets the type of the string parser 
		/// </summary>
		Type Parser { get; }

		/// <summary>
		/// Clears the value
		/// </summary>
		void ResetValue();
	}

	/// <summary>
	/// A named value that contains some data under some name, with specific type
	/// </summary>
	/// <typeparam name="T">The type of the data</typeparam>
	public interface INamedValue<T> : INamedValue
	{
		/// <summary>
		/// Gets the value's data
		/// </summary>
		new T Value { get; set; }

		/// <summary>
		/// Gets list of values
		/// </summary>
		new IList<T> Values { get; }
	}

	/// <summary>
	/// A named value that contains some data under some name, with specific type
	/// </summary>
	/// <typeparam name="T">The type of the data</typeparam>
	public sealed class NamedValue<T> : INamedValue<T>
	{
		#region Fields

		private List<T> values = new List<T>(1);
		#endregion

		#region Properties

		/// <summary>
		/// Gets the value's data
		/// </summary>
		public T Value
		{
			get { return this.Values[0]; }
			set
			{
				this.Values[0] = value;
				this.IsValueSet = true;
			}
		}

		/// <summary>
		/// Gets the name of the value
		/// </summary>
		public string Name { get; }

		object INamedValue.Value
		{
			get { return this.Value; }
			set { this.Value = (T)value; }
		}

		/// <summary>
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </summary>
		public bool IsNameVisible { get; set; }

		/// <summary>
		/// Gets the type of the string parser 
		/// </summary>
		public Type Parser { get; }

		private IParser ParserObj { get; }

		IList INamedValue.Values =>
			this.values;

		/// <summary>
		/// Gets list of values
		/// </summary>
		public IList<T> Values =>
			this.values;

		/// <summary>
		/// Get value indicates if the value has been set
		/// </summary>
		public bool IsValueSet { get; private set; }
		#endregion

		#region Ctor

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		/// <param name="parser">The string parser's type</param>
		public NamedValue(string name, Type parser = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			this.Name = name;
			this.Value = default(T);

			this.Parser = parser ?? typeof(DefaultTypeParser<T>);
			this.ParserObj = (IParser)Activator.CreateInstance(this.Parser);
		}

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		/// <param name="valueData">The value's data</param>
		/// <param name="parser">The string parser's type</param>
		public NamedValue(string name, T valueData, Type parser = null)
			: this(name, parser)
		{
			this.Value = valueData;
		}

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		/// <param name="isNameVisible">
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </param>
		/// <param name="parser">The string parser's type</param>
		public NamedValue(string name, bool isNameVisible, Type parser = null)
			: this(name, parser)
		{
			this.IsNameVisible = isNameVisible;
		}

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		/// <param name="valueData">The value's data</param>
		/// <param name="isNameVisible">
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </param>
		/// <param name="parser">The string parser's type</param>
		public NamedValue(string name, T valueData, bool isNameVisible, Type parser = null)
			: this(name, valueData, parser)
		{
			this.IsNameVisible = isNameVisible;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Parses the string
		/// </summary>
		/// <param name="valueString">String representation of the value</param>
		public void ParseString(string valueString) =>
			this.Value = (T)this.ParserObj.Parse(valueString);

		/// <summary>
		/// Adds the parsed string to the values list
		/// </summary>
		/// <param name="valueString"></param>
		public void AddParsedString(string valueString)
		{
			T parsed = (T)this.ParserObj.Parse(valueString);

			if (!this.IsValueSet)
			{
				this.Value = parsed;
			}
			else
			{
				this.Values.Add(parsed);
			}
		}

		/// <summary>
		/// Clears the value
		/// </summary>
		public void ResetValue()
		{
			if (this.values.Count > 1)
			{
				this.values.RemoveRange(1, this.values.Count - 1);
			}

			this.Value = default(T);
			this.IsValueSet = false;
		}
		#endregion

		#region Operators

		/// <summary>
		/// Cast from NamedValue&lt;T&gt; to T
		/// </summary>
		/// <param name="namedValue"></param>
		/// <returns></returns>
		public static explicit operator T(NamedValue<T> namedValue) =>
			namedValue.Value;
		#endregion
	}
}
