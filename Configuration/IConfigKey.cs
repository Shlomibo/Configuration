using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Interface for configuration key
	/// </summary>
	public interface IConfigKey : IEnumerable<INamedValue>
	{
		#region Properties

		/// <summary>
		/// Gets the name of the key
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets or sets the value with the given name
		/// </summary>
		/// <param name="namedValue">The name of the value to retrieve</param>
		/// <returns>Returns the value data</returns>
		object this[string namedValue] { get; set; }

		/// <summary>
		/// Gets the values in the key
		/// </summary>
		IReadOnlyDictionary<string, INamedValue> Values { get; }

		/// <summary>
		/// Gets the count of values in the key
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets the name of the default value.
		/// </summary>
		string DefaultValueName { get; }
		#endregion

		#region Methods

		/// <summary>
		/// Checks if value with the given name exists in the key
		/// </summary>
		/// <param name="value">The name of the value to check.</param>
		/// <returns>true if value with the given name exists; otherwise false.</returns>
		bool ContainsValue(string value);
		
		/// <summary>
		/// Safely retrieves the value's data
		/// </summary>
		/// <param name="value">The value's name</param>
		/// <param name="valueData">Out: the data under the value</param>
		/// <returns>true if the data successfuly retrieved; otherwise false.</returns>
		bool TryGetValue(string value, out object valueData);

		/// <summary>
		/// Safely retrieves the value's data
		/// </summary>
		/// <typeparam name="T">The value's type</typeparam>
		/// <param name="value">The value's name</param>
		/// <param name="valueData">Out: the data under the value</param>
		/// <returns>true if the data successfuly retrieved; otherwise false.</returns>
		bool TryGetValue<T>(string value, out T valueData);
		#endregion
	}
}
