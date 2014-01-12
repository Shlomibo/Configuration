using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class ConfigValueAttribute : Attribute
	{
		#region Fields

		private Type accomodator; 
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the string that whould be printed if the property value is null
		/// </summary>
		public object NullValue { get; set; }

		/// <summary>
		/// Gets or set the name of the key in the 
		/// </summary>
		public string ConfigName { get; set; }

		/// <summary>
		/// Gets or sets an accomodator for the values
		/// </summary>
		public Type Accomodator
		{
			get { return this.accomodator; }
			set
			{
				if ((value != null) && 
					!typeof(IConfigValueAccomodator).IsAssignableFrom(value))
				{
					throw new ArgumentException("Accomodator must be of type IConfigValueAccomodator", "Accomodator");
				}

				this.accomodator = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the value in configuration (is it's not the same as the property)
		/// </summary>
		public Type ConfigType { get; set; }
		#endregion

		#region Ctor

		public ConfigValueAttribute()
		{
		}

		public ConfigValueAttribute(string configName)
			: this()
		{
			this.ConfigName = configName;
		}

		public ConfigValueAttribute(string configName, object nullValue)
			: this(configName)
		{
			this.NullValue = nullValue;
		}
		#endregion

		#region Methods

		public static ConfigValueAttribute GetAttribute(PropertyInfo property)
		{
			return (ConfigValueAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigValueAttribute));
		}
		#endregion
	}
}
