using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// Provides dictionary access to type's properties, that have marked with ConfigValueAttribute
	/// </summary>
	public class PropertyCollection : IDictionary<string, PropertyInfo>
	{
		#region Fields

		private Dictionary<string, PropertyInfo> byPropName;
		private Dictionary<string, PropertyInfo> byConfigName;
		#endregion

		#region Properties

		/// <summary>
		/// The type which contains the properties
		/// </summary>
		public Type ParentType { get; private set; }

		private Dictionary<string, PropertyInfo> ByPropertyName
		{
			get
			{
				if (this.byPropName == null)
				{
					BuildPropList();
				}

				return this.byPropName;
			}
		}

		private Dictionary<string, PropertyInfo> ByConfigName
		{
			get
			{
				if (this.byConfigName == null)
				{
					BuildPropList();
				}

				return this.byConfigName;
			}
		}

		ICollection<string> IDictionary<string, PropertyInfo>.Keys
		{
			get { return this.PropertyNames; }
		}

		/// <summary>
		/// Gets collection of the properties' names
		/// </summary>
		public ICollection<string> PropertyNames
		{
			get { return this.ByPropertyName.Keys; }
		}

		/// <summary>
		/// Gets collection of the configuration's value names
		/// </summary>
		public ICollection<string> ConfigNames
		{
			get { return this.ByConfigName.Keys; }
		}

		/// <summary>
		/// Gets collection of the property objects, which have been marked with ConfigValueAttribute
		/// </summary>
		public ICollection<PropertyInfo> Values
		{
			get { return this.ByPropertyName.Values; }
		}

		/// <summary>
		/// Gets the property object, with the given name, or configuration name
		/// </summary>
		/// <param name="key">The property, or configuration name</param>
		/// <returns>The property with the given name</returns>
		public PropertyInfo this[string key]
		{
			get
			{
				PropertyInfo value;

				if (!this.ByPropertyName.TryGetValue(key, out value) &&
					!this.ByConfigName.TryGetValue(key, out value))
				{
					throw new KeyNotFoundException();
				}

				return value;
			}
		}

		PropertyInfo IDictionary<string, PropertyInfo>.this[string key]
		{
			get { return this[key]; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets the count of configuration properties
		/// </summary>
		public int Count
		{
			get { return this.ByPropertyName.Count; }
		}

		bool ICollection<KeyValuePair<string, PropertyInfo>>.IsReadOnly
		{
			get { return true; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates PropertyCollection for the given type.
		/// </summary>
		/// <param name="parentType">The type for which properties whould be accessed</param>
		public PropertyCollection(Type parentType)
		{
			this.ParentType = parentType;
		}
		#endregion

		#region Methods

		private void BuildPropList()
		{
			IEnumerable<PropertyInfo> props = this.ParentType.GetProperties(
				BindingFlags.Instance | BindingFlags.Public);

			var propData = from prop in props
						   let attributeTypes = prop.CustomAttributes.Select(attData => attData.AttributeType)
						   where attributeTypes.Contains(typeof(ConfigValueAttribute))
						   select new
						   {
							   Property = prop,
							   Attribute = ConfigValueAttribute.GetAttribute(prop),
						   };

			byPropName = propData.ToDictionary(
				prop => prop.Property.Name,
				prop => prop.Property);

			byConfigName = propData.ToDictionary(
				prop => prop.Attribute.ConfigName ?? prop.Property.Name,
				prop => prop.Property);
		}

		void IDictionary<string, PropertyInfo>.Add(string key, PropertyInfo value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Checks if property with the given name or configuration name exists
		/// </summary>
		/// <param name="key">Property or configuration name</param>
		/// <returns>true if such property esixts; otherwise false.</returns>
		public bool ContainsKey(string key)
		{
			return this.ByPropertyName.ContainsKey(key) ||
				this.ByConfigName.ContainsKey(key);
		}

		bool IDictionary<string, PropertyInfo>.Remove(string key)
		{
			return false;
		}

		/// <summary>
		/// Tries to get property with the given property or configuration name
		/// </summary>
		/// <param name="key">Property or configuration name</param>
		/// <param name="value">Out: the property with the given name</param>
		/// <returns>True if such property exists, and retrieved; otherwise false.</returns>
		public bool TryGetValue(string key, out PropertyInfo value)
		{
			return this.ByPropertyName.TryGetValue(key, out value) ||
				this.ByConfigName.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<string, PropertyInfo>>.Add(KeyValuePair<string, PropertyInfo> item)
		{
			throw new NotSupportedException();
		}

		void ICollection<KeyValuePair<string, PropertyInfo>>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<KeyValuePair<string, PropertyInfo>>.Contains(KeyValuePair<string, PropertyInfo> item)
		{
			return this.ContainsKey(item.Key) &&
				(this[item.Key] == item.Value);
		}

		void ICollection<KeyValuePair<string, PropertyInfo>>.CopyTo(
			KeyValuePair<string, PropertyInfo>[] array,
			int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (array.Length - arrayIndex < this.Count)
			{
				throw new ArgumentException("array is too small");
			}

			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}

			var items = Enumerable.Zip(
				Enumerable.Range(0, this.Count),
				this.PropertyNames,
				(index, key) => new
				{
					Index = index,
					Item = new KeyValuePair<string, PropertyInfo>(key, this[key]),
				});

			foreach (var item in items)
			{
				array[item.Index + arrayIndex] = item.Item;
			}
		}

		bool ICollection<KeyValuePair<string, PropertyInfo>>.Remove(KeyValuePair<string, PropertyInfo> item)
		{
			return false;
		}

		IEnumerator<KeyValuePair<string, PropertyInfo>>
			IEnumerable<KeyValuePair<string, PropertyInfo>>.GetEnumerator()
		{
			return this.ByPropertyName.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<KeyValuePair<string, PropertyInfo>>).GetEnumerator();
		}
		#endregion
	}
}
