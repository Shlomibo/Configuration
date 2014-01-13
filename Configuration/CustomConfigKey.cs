using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Provides config key with any value names, and data
	/// </summary>
	public class CustomConfigKey : IConfigKey
	{
		#region Fields

		private string name; 
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the key
		/// </summary>
		public string Name 
		{
			get { return this.name; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Name");
				}

				this.name = value;
			}
		}

		/// <summary>
		/// Gets or sets the value with the given name
		/// </summary>
		/// <param name="valueName">The name of the value to retrieve</param>
		/// <returns>Returns the value data</returns>
		public object this[string valueName]
		{
			get { return this.Values[valueName].Value; }
			set { this.Values[valueName].Value = value; }
		}

		IReadOnlyDictionary<string, INamedValue> IConfigKey.Values
		{
			get { return this.Values; }
		}

		/// <summary>
		/// Gets a dictionary of named values
		/// </summary>
		public Dictionary<string, INamedValue> Values { get; private set; }

		/// <summary>
		/// Gets the count of values in the key
		/// </summary>
		public int Count
		{
			get { return this.Values.Count; }
		}

		public string DefaultValueName { get; set; }
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of CustomConfigKey
		/// </summary>
		/// <param name="name">The name of the key</param>
		public CustomConfigKey(string name)
		{
			this.Name = name;
			this.Values = new Dictionary<string, INamedValue>();
		}
		#endregion

		#region Methods

		/// <summary>
		/// Checks if value with the given name exists in the key
		/// </summary>
		/// <param name="value">The name of the value to check.</param>
		/// <returns>true if value with the given name exists; otherwise false.</returns>
		public bool ContainsValue(string value)
		{
			return this.Values.ContainsKey(value);
		}

		/// <summary>
		/// Safely retrieves the value's data
		/// </summary>
		/// <param name="value">The value's name</param>
		/// <param name="valueData">Out: the data under the value</param>
		/// <returns>true if the data successfuly retrieved; otherwise false.</returns>
		public bool TryGetValue(string value, out object valueData)
		{
			valueData = null;
			INamedValue namedValue;
			bool didSucceed = this.Values.TryGetValue(value, out namedValue);

			if (didSucceed)
			{
				valueData = namedValue.Value;
			}

			return didSucceed;
		}

		/// <summary>
		/// Safely retrieves the value's data
		/// </summary>
		/// <typeparam name="T">The value's type</typeparam>
		/// <param name="value">The value's name</param>
		/// <param name="valueData">Out: the data under the value</param>
		/// <returns>true if the data successfuly retrieved; otherwise false.</returns>
		public bool TryGetValue<T>(string value, out T valueData)
		{
			valueData = default(T);
			object valueObj;
			bool didSucceed = TryGetValue(value, out valueObj);

			if (didSucceed)
			{
				try
				{
					valueData = (T)valueObj;
				}
				catch 
				{
					didSucceed = false;
				}
			}

			return didSucceed;
		}
		
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>A IEnumerator&lt;T&gt; that can be used to iterate through the collection.</returns>
		public IEnumerator<INamedValue> GetEnumerator()
		{
			return this.Values.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds named value to the key.
		/// </summary>
		/// <typeparam name="T">The type of the value's data</typeparam>
		/// <param name="valueName">The name of the value</param>
		/// <param name="valueData">The data of the value</param>
		/// <param name="isNameVisible">
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </param>
		public void Add<T>(string valueName, T valueData, bool isNameVisible = true)
		{
			this.Values.Add(valueName, new NamedValue<T>(valueName, valueData, isNameVisible));
		}

		/// <summary>
		/// Removes named value from the key
		/// </summary>
		/// <param name="valueName">The name of the value to remove</param>
		/// <returns>true if the value was removed from the key; otherwise false.</returns>
		public bool Remove(string valueName)
		{
			return this.Values.Remove(valueName);
		}

		/// <summary>
		/// Add new named value to the key, if no value with that name exists; otherwise updates the value's data.
		/// </summary>
		/// <typeparam name="T">The value's data type</typeparam>
		/// <param name="valueName">The name of the value to add, or update</param>
		/// <param name="valueData">The data of the value to add, or update</param>
		/// <param name="throwOnTypeChange">
		/// If true, and the type of the new value is different than that of the old value, 
		/// an exception would be thrown.
		/// </param>
		/// <param name="isNameVisible">
		/// Gets or or sets value that indicates if the name should be visible in the configuration storage
		/// </param>
		public void AddOrUpdate<T>(string valueName, T valueData, bool throwOnTypeChange, bool isNameVisible = true)
		{
			if (valueName == null)
			{
				throw new ArgumentNullException("valueName");
			}

			if (!this.Values.ContainsKey(valueName))
			{
				Add(valueName, valueData);
			}
			else if (throwOnTypeChange && 
				(this.Values[valueName].Value.GetType() != valueData.GetType()))
			{
				throw new InvalidOperationException("New value is different type from old value");
			}
			else
			{
				this.Values[valueName] = new NamedValue<T>(valueName, valueData, isNameVisible);
			}
		}
		#endregion
	}
}
