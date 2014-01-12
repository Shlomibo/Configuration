using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public class PropertyCollection : IDictionary<string, PropertyInfo>
	{
		#region Fields

		private Dictionary<string, PropertyInfo> byPropName;
		private Dictionary<string, PropertyInfo> byKeyName;
		#endregion

		#region Properties

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

		private Dictionary<string, PropertyInfo> ByKeyName
		{
			get
			{
				if (this.byKeyName == null)
				{
					BuildPropList();
				}

				return this.byKeyName;
			}
		}

		ICollection<string> IDictionary<string, PropertyInfo>.Keys
		{
			get { return this.PropertyNames; }
		}

		public ICollection<string> PropertyNames
		{
			get { return this.ByPropertyName.Keys; }
		}

		public ICollection<string> KeyNames
		{
			get { return this.ByKeyName.Keys; }
		}

		public ICollection<PropertyInfo> Values
		{
			get { return this.ByPropertyName.Values; }
		}

		public PropertyInfo this[string key]
		{
			get
			{
				PropertyInfo value;

				if (!this.ByPropertyName.TryGetValue(key, out value) &&
					!this.ByKeyName.TryGetValue(key, out value))
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
						   let attributeTypes = from attData in prop.CustomAttributes
												select attData.AttributeType
						   where attributeTypes.Contains(typeof(ConfigValueAttribute))
						   select new
						   {
							   Property = prop,
							   Attribute = ConfigValueAttribute.GetAttribute(prop),
						   };

			byPropName = propData.ToDictionary(
				prop => prop.Property.Name,
				prop => prop.Property);

			byKeyName = propData.ToDictionary(
				prop => prop.Attribute.ConfigName ?? prop.Property.Name,
				prop => prop.Property);
		}

		void IDictionary<string, PropertyInfo>.Add(string key, PropertyInfo value)
		{
			throw new NotSupportedException();
		}

		public bool ContainsKey(string key)
		{
			return this.ByPropertyName.ContainsKey(key) ||
				this.ByKeyName.ContainsKey(key);
		}

		bool IDictionary<string, PropertyInfo>.Remove(string key)
		{
			return false;
		}

		public bool TryGetValue(string key, out PropertyInfo value)
		{
			return this.ByPropertyName.TryGetValue(key, out value) ||
				this.ByKeyName.TryGetValue(key, out value);
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
