using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	/// <summary>
	/// A configuration, built from keys, which contians named values
	/// </summary>
	public class Configuration : IList<IConfigKey>, IDictionary<string, IConfigKey>
	{
		#region Fields

		private List<IConfigKey> keyList;
		private Dictionary<string, IConfigKey> keyDictionary;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the key at the given index
		/// </summary>
		/// <param name="index">The index for the key</param>
		/// <returns>The key at the given index</returns>
		public IConfigKey this[int index]
		{
			get { return this.keyList[index]; }
			set
			{
				if ((this.keyList[index].Name != value.Name) && this.keyDictionary.ContainsKey(value.Name))
				{
					throw new ArgumentException("A key with the same name already exists");
				}
				else if (this.keyList[index].Name != value.Name)
				{
					this.keyDictionary.Remove(this.keyList[index].Name);
					this.keyDictionary.Add(value.Name, value);
				}

				this.keyList[index] = value;
			}
		}

		/// <summary>
		/// Gets the count of keys in the configuration.
		/// </summary>
		public int Count
		{
			get { return this.keyList.Count; }
		}

		bool ICollection<IConfigKey>.IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<KeyValuePair<string, IConfigKey>>.IsReadOnly
		{
			get { return (this as ICollection<IConfigKey>).IsReadOnly; }
		}

		ICollection<string> IDictionary<string, IConfigKey>.Keys
		{
			get { return this.KeyNames; }
		}

		/// <summary>
		/// Gets a collection of the key names in the configuration
		/// </summary>
		public ICollection<string> KeyNames
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a collection of the keys in the configuration
		/// </summary>
		public ICollection<IConfigKey> Keys
		{
			get { return this.keyDictionary.Values; }
		}

		ICollection<IConfigKey> IDictionary<string, IConfigKey>.Values
		{
			get { return this.Keys; }
		}

		/// <summary>
		/// Gets or sets the key with the given name
		/// </summary>
		/// <param name="keyName">The name of the key</param>
		/// <returns>The key with the given name</returns>
		public IConfigKey this[string keyName]
		{
			get { return this.keyDictionary[keyName]; }
			set
			{
				if (value.Name != keyName)
				{
					throw new ArgumentException("The new key must have the same name as the old");
				}

				int index = this.keyList.IndexOf(this.keyDictionary[keyName]);
				this.keyList[index] = value;

				this.keyDictionary[keyName] = value;
			}
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Initialize new instance of configuration
		/// </summary>
		public Configuration()
		{
			this.keyDictionary = new Dictionary<string, IConfigKey>();
			this.keyList = new List<IConfigKey>();
		}

		/// <summary>
		/// Initialize new instance of configuration, with collection of keys
		/// </summary>
		/// <param name="keys"></param>
		public Configuration(IEnumerable<IConfigKey> keys)
			: this()
		{
			foreach (IConfigKey key in keys)
			{
				Add(key);
			}
		}

		/// <summary>
		/// Initialize new instance of configuration, with the given keys
		/// </summary>
		public Configuration(params IConfigKey[] keys) : this(keys as IEnumerable<IConfigKey>) { }
		#endregion

		#region Methods

		/// <summary>
		/// Determines the index of a specific key in the configuration
		/// </summary>
		/// <param name="configKey">The key to locate in the configuration</param>
		/// <returns>The index of key if found in the configuration; otherwise, -1.</returns>
		public int IndexOf(IConfigKey configKey)
		{
			return this.keyList.IndexOf(configKey);
		}

		/// <summary>
		/// Inserts an key to the configuration at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which key should be inserted.</param>
		/// <param name="configKey">The key to insert into the configuration.</param>
		public void Insert(int index, IConfigKey configKey)
		{
			this.keyDictionary.Add(configKey.Name, configKey);
			this.keyList.Insert(index, configKey);
		}

		/// <summary>
		/// Removes the configuration key at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the key to remove.</param>
		public void RemoveAt(int index)
		{
			this.keyDictionary.Remove(this.keyList[index].Name);
			this.keyList.RemoveAt(index);
		}

		/// <summary>
		/// Adds a key to the configuration.
		/// </summary>
		/// <param name="configKey">The key to add to the configuration.</param>
		public void Add(IConfigKey configKey)
		{
			this.keyDictionary.Add(configKey.Name, configKey);
			this.keyList.Add(configKey);
		}

		/// <summary>
		/// Removes all keys from the configuration. 
		/// </summary>
		public void Clear()
		{
			this.keyList.Clear();
			this.keyDictionary.Clear();
		}

		/// <summary>
		/// Determines whether the configuration contains a specific key.
		/// </summary>
		/// <param name="configKey">The key to locate in the configuration.</param>
		/// <returns>true if key is found in the configuration; otherwise, false.</returns>
		public bool Contains(IConfigKey configKey)
		{
			return this.keyDictionary.ContainsKey(configKey.Name) &&
				this.keyDictionary[configKey.Name].Equals(configKey);
		}

		/// <summary>
		/// Copies the keys of the configuration to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional Array that is the destination of the keys copied from configuration. 
		/// The Array must have zero-based indexing.
		/// </param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(IConfigKey[] array, int arrayIndex)
		{
			this.keyList.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specific key from the configuration.
		/// </summary>
		/// <param name="configKey">The key to remove from the configuration.</param>
		/// <returns>
		/// true if key was successfully removed from the configuration; otherwise, false. 
		/// This method also returns false if key is not found in the original configuration.</returns>
		public bool Remove(IConfigKey configKey)
		{
			bool didRemoved = this.keyList.Remove(configKey);

			if (didRemoved)
			{
				this.keyDictionary.Remove(configKey.Name);
			}

			return didRemoved;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the configuration.
		/// </summary>
		/// <returns>A IEnumerator&lt;IConfigKey&gt; that can be used to iterate through the configuration.</returns>
		public IEnumerator<IConfigKey> GetEnumerator()
		{
			return this.keyList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IDictionary<string, IConfigKey>.Add(string key, IConfigKey value)
		{
			Add(value);
		}

		/// <summary>
		/// Determines whether the configuration contains a key with the specified key name.
		/// </summary>
		/// <param name="keyName">The key name to locate in the configuration.</param>
		/// <returns>true if the configuration contains a key with the key name; otherwise, false.</returns>
		public bool ContainsKey(string keyName)
		{
			return this.keyDictionary.ContainsKey(keyName);
		}

		/// <summary>
		/// Removes the key with the specified key name from the configuration.
		/// </summary>
		/// <param name="keyName">The key name of the key to remove.</param>
		/// <returns>
		/// true if the key is successfully removed; otherwise, false. 
		/// This method also returns false if key name was not found in the original configuration.
		/// </returns>
		public bool Remove(string keyName)
		{
			if (this.keyDictionary.ContainsKey(keyName))
			{
				this.keyList.Remove(this.keyDictionary[keyName]);
			}

			return this.keyDictionary.Remove(keyName);
		}

		bool IDictionary<string, IConfigKey>.TryGetValue(string keyName, out IConfigKey configKey)
		{
			return TryGetKey(keyName, out configKey);
		}

		/// <summary>
		/// Gets the key associated with the specified key name.
		/// </summary>
		/// <param name="keyName">The key name whose key to get.</param>
		/// <param name="configKey">
		/// When this method returns, the key associated with the specified key name, if the key name is found; otherwise, null. 
		/// This parameter is passed uninitialized.
		/// </param>
		/// <returns>true if the configuration contains a key with the specified key name; otherwise, false.</returns>
		public bool TryGetKey(string keyName, out IConfigKey configKey)
		{
			return this.keyDictionary.TryGetValue(keyName, out configKey);
		}

		void ICollection<KeyValuePair<string, IConfigKey>>.Add(KeyValuePair<string, IConfigKey> item)
		{
			Add(item.Value);
		}

		bool ICollection<KeyValuePair<string, IConfigKey>>.Contains(KeyValuePair<string, IConfigKey> item)
		{
			return Contains(item.Value);
		}

		void ICollection<KeyValuePair<string, IConfigKey>>.CopyTo(KeyValuePair<string, IConfigKey>[] array, int arrayIndex)
		{
			var items = Enumerable.Zip(
				Enumerable.Range(0, this.Count),
				this.keyDictionary,
				(index, item) => new
				{
					Index = index,
					Item = item,
				});

			foreach (var item in items)
			{
				array[arrayIndex + item.Index] = item.Item;
			}
		}

		bool ICollection<KeyValuePair<string, IConfigKey>>.Remove(KeyValuePair<string, IConfigKey> item)
		{
			return Remove(item.Value);
		}

		IEnumerator<KeyValuePair<string, IConfigKey>> IEnumerable<KeyValuePair<string, IConfigKey>>.GetEnumerator()
		{
			return this.keyDictionary.GetEnumerator();
		}
		#endregion
	}
}
