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
		T Value { get; set; }
	}

	/// <summary>
	/// A named value that contains some data under some name, with specific type
	/// </summary>
	/// <typeparam name="T">The type of the data</typeparam>
	public class NamedValue<T> : INamedValue<T>
	{
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
	}

}
