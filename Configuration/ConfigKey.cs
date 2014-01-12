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
	public abstract class ConfigKey : IConfigKey
	{
		#region Consts

		private const string CAT_WARNING = "Warning";
		#endregion

		#region Properties

		public abstract string Name { get; }

		protected PropertyCollection Properties { get; private set; }

		public object this[string valueName]
		{
			get { return GetConfigValue(this.Properties[valueName]); }
			set { SetConfigValue(this.Properties[valueName], value); }
		}

		public IReadOnlyDictionary<string, INamedValue> Values
		{
			get
			{
				var rowValues = from prop in this.Properties.Values
								let attribute = ConfigValueAttribute.GetAttribute(prop)
								let name = attribute.ConfigName ?? prop.Name
								let accomodator = attribute.Accomodator != null
									? (IConfigValueAccomodator)Activator.CreateInstance(attribute.Accomodator)
									: null
								let outType = attribute.ConfigType ?? prop.PropertyType
								let namedValueType = typeof(NamedValue<>).MakeGenericType(outType)
								let value = accomodator != null
									? accomodator.Accomodate(prop.GetValue(this))
									: prop.GetValue(this)
								select (INamedValue)Activator.CreateInstance(namedValueType, name, value);

				return rowValues.ToDictionary(
					value => value.Name,
					value => value);

			}
		}

		public int Count
		{
			get { return this.Properties.Count; }
		}
		#endregion

		#region Ctor

		protected ConfigKey()
		{
			this.Properties = new PropertyCollection(GetType());
		}
		#endregion

		#region Methods

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

		public bool ContainsValue(string value)
		{
			return this.Properties.ContainsKey(value);
		}

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
