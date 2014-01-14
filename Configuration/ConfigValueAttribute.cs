using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Marks properties as configuration named values.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class ConfigValueAttribute : Attribute
	{
		#region Fields

		private Type accomodator;
		private Type typeParser; 
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

		/// <summary>
		/// Gets or sets the string parser's type
		/// </summary>
		public Type TypeParser
		{
			get { return this.typeParser; }
			set
			{
				if (typeof(IParser).IsAssignableFrom(value))
				{
					throw new ArgumentException("Type parser must implement IParser");
				}

				this.typeParser = value;
			}
		}

		/// <summary>
		/// Gets or sets value, that indicates if the name should be stored with the value.
		/// </summary>
		public bool IsNameVisible { get; set; }
		#endregion

		#region Ctor

		/// <summary>
		/// Marks the property as named value
		/// </summary>
		public ConfigValueAttribute()
		{
			this.IsNameVisible = true;
		}

		/// <summary>
		/// Marks the property as named value, with the given name
		/// </summary>
		/// <param name="configName">The name for the named value, for the property</param>
		public ConfigValueAttribute(string configName)
			: this()
		{
			this.ConfigName = configName;
		}

		/// <summary>
		/// Marks the property as named value, with the given name, and null value substitute
		/// </summary>
		/// <param name="configName">The name for the named value, for the property</param>
		/// <param name="nullValue">
		/// A value that would be placed in case of null
		/// This value needs to be of the configuration type, if it's different that the property type.
		/// </param>
		public ConfigValueAttribute(string configName, object nullValue)
			: this(configName)
		{
			this.NullValue = nullValue;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Gets the attribute that placed for the given proeprty
		/// </summary>
		/// <param name="property">The property to extraxt the attribute from.</param>
		/// <returns>The attribute that stored for that property.</returns>
		public static ConfigValueAttribute GetAttribute(PropertyInfo property)
		{
			return (ConfigValueAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigValueAttribute));
		}
		#endregion
	}
}
