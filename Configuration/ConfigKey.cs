using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Configuration key base class
	/// </summary>
	public abstract class ConfigKey : IConfigKey
	{
		#region Consts

		private const string CAT_WARNING = "Warning";
		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the key
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the name of the default value.
		/// </summary>
		public virtual string DefaultValueName { get { return null; } }

		/// <summary>
		/// Gets the properties that have been marked with ConfigKeyAttribute
		/// </summary>
		protected PropertyCollection Properties { get; private set; }

		/// <summary>
		/// Gets or sets the value with the given name
		/// </summary>
		/// <param name="valueName">The name of the value to retrieve</param>
		/// <returns>Returns the value data</returns>
		public object this[string valueName]
		{
			get { return GetConfigValue(this.Properties[valueName]); }
			set { SetConfigValue(this.Properties[valueName], value); }
		}

		/// <summary>
		/// Gets a dictionary of named values in the key
		/// </summary>
		public IReadOnlyDictionary<string, INamedValue> Values
		{
			get
			{
				var rowValues = from prop in this.Properties.Values
								let attribute = ConfigValueAttribute.GetAttribute(prop)
								let name = attribute.ConfigName ?? prop.Name
								let accomodator = attribute.Accomodator != null
									? attribute.AccomodatorParam == null
										? (IConfigValueAccomodator)Activator.CreateInstance(attribute.Accomodator)
										: (IConfigValueAccomodator)Activator.CreateInstance(
											attribute.Accomodator, 
											attribute.AccomodatorParam)
									: null
								let outType = attribute.ConfigType ?? prop.PropertyType
								let namedValueType = typeof(NamedValue<>).MakeGenericType(outType)
								let value = accomodator != null
									? accomodator.Accomodate(prop.GetValue(this))
									: prop.GetValue(this)
								select (INamedValue)Activator.CreateInstance(namedValueType, 
											name, 
											value, 
											attribute.IsNameVisible,
											attribute.TypeParser);

				return rowValues.ToDictionary(
					value => value.Name,
					value => value);

			}
		}

		/// <summary>
		/// Gets the count of named values in the key
		/// </summary>
		public int Count
		{
			get { return this.Properties.Count; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Initialize the config key
		/// </summary>
		protected ConfigKey()
		{
			this.Properties = new PropertyCollection(GetType());
		}
		#endregion

		#region Methods

		/// <summary>
		/// Set the configuration value. IE, the value in configuration, not the property
		/// </summary>
		/// <param name="property">The property to set</param>
		/// <param name="value">The value in configuration</param>
		protected void SetConfigValue(PropertyInfo property, object value)
		{
			ConfigValueAttribute attribute = ConfigValueAttribute.GetAttribute(property);

			if (attribute.Accomodator != null)
			{
				var accomodator = (IConfigValueAccomodator)Activator.CreateInstance(attribute.Accomodator);
				value = accomodator.AccomodateBack(value);
			}

			property.SetValue(this, value);
		}

		/// <summary>
		/// Gets the configuratino value from the property
		/// </summary>
		/// <param name="property">The property of the value</param>
		/// <returns>The configuration value for that property</returns>
		protected object GetConfigValue(PropertyInfo property)
		{
			ConfigValueAttribute attribute = ConfigValueAttribute.GetAttribute(property);
			object value = property.GetValue(this);

			if ((value == null) && (attribute.NullValue != null))
			{
				value = attribute.NullValue;
			}

			if (attribute.Accomodator != null)
			{
				var accomodator = (IConfigValueAccomodator)Activator.CreateInstance(attribute.Accomodator);
				value = accomodator.Accomodate(value);
			}

			return value;
		}

		/// <summary>
		/// Checks if value with the given name exists in the key
		/// </summary>
		/// <param name="value">The name of the value to check.</param>
		/// <returns>true if value with the given name exists; otherwise false.</returns>
		public bool ContainsValue(string value)
		{
			return this.Properties.ContainsKey(value);
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
			PropertyInfo prop;
			bool didSucceed = this.Properties.TryGetValue(value, out prop);

			if (didSucceed)
			{
				try
				{
					valueData = GetConfigValue(prop);
				}
				catch 
				{
					didSucceed = false;
				}
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
		#endregion
	}
}
