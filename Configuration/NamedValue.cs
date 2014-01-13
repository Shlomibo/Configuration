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

		IList Values { get; }

		/// <summary>
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </summary>
		bool IsNameVisible { get; set; }

		/// <summary>
		/// Parses the string
		/// </summary>
		/// <param name="valueString">String representation of the value</param>
		void ParseString(string valueString);

		void AddParsedString(string valueString);

		Type Parser { get; }
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

		new IList<T> Values { get; }
	}

	/// <summary>
	/// A named value that contains some data under some name, with specific type
	/// </summary>
	/// <typeparam name="T">The type of the data</typeparam>
	public class NamedValue<T> : INamedValue<T>
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
			set { this.Values[0] = value; }
		}

		/// <summary>
		/// Gets the name of the value
		/// </summary>
		public string Name { get; private set; }

		object INamedValue.Value
		{
			get { return this.Value; }
			set { this.Value = (T)value; }
		}

		/// <summary>
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </summary>
		public bool IsNameVisible { get; set; }

		public Type Parser { get; private set; }

		private IParser ParserObj { get; set; }

		IList INamedValue.Values
		{
			get { return this.values; }
		}

		public IList<T> Values
		{
			get { return this.values; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		public NamedValue(string name, Type parser = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			this.Name = name;
			this.Value = default(T);

			this.Parser = parser ?? typeof(DefaultTypeParse<T>);
			this.ParserObj = (IParser)Activator.CreateInstance(this.Parser);
		}

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		/// <param name="valueData">The value's data</param>
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
		public NamedValue(string name, T valueData, bool isNameVisible, Type parser = null)
			: this(name, valueData, parser)
		{
			this.IsNameVisible = isNameVisible;
		}
		#endregion

		#region Methods

		public void ParseString(string valueString)
		{
			this.Value = (T)this.ParserObj.Parse(valueString);
		}

		public void AddParsedString(string valueString)
		{
			this.Values[this.Values.Count - 1] = (T)this.ParserObj.Parse(valueString);
			this.Values.Add(default(T));
		}
		#endregion

		#region Operators

		/// <summary>
		/// Cast from NamedValue&lt;T&gt; to T
		/// </summary>
		/// <param name="namedValue"></param>
		/// <returns></returns>
		public static explicit operator T(NamedValue<T> namedValue)
		{
			return namedValue.Value;
		}
		#endregion
	}
}
