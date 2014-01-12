using System;
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
	}

	/// <summary>
	/// A named value that contains some data under some name, with specific type
	/// </summary>
	/// <typeparam name="T">The type of the data</typeparam>
	public class NamedValue<T> : INamedValue<T>
	{
		#region Properties

		/// <summary>
		/// Gets the value's data
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Gets the name of the value
		/// </summary>
		public string Name { get; private set; }

		object INamedValue.Value
		{
			get { return this.Value; }
			set { this.Value = (T)value; }
		} 
		#endregion

		#region Ctor

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		public NamedValue(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			this.Name = name;
			this.Value = default(T);
		}

		/// <summary>
		/// Create new NamedValue
		/// </summary>
		/// <param name="name">The value's name</param>
		/// <param name="valueData">The value's data</param>
		public NamedValue(string name, T valueData)
			: this(name)
		{
			this.Value = valueData;
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
