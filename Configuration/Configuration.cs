using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public class Configuration : IList<IConfigKey>, IDictionary<string, IConfigKey>
	{
		#region Fields

		private List<IConfigKey> keyList;
		private IDictionary<string, IConfigKey> keyDictionary;
		#endregion

		#region Properties

		public IConfigKey this[int index]
		{
			get { return this.keyList[index]; }
			set { this.keyList[index] = value; }
		}

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

		public ICollection<string> KeyNames
		{
			get { throw new NotImplementedException(); }
		}

		public ICollection<IConfigKey> Keys
		{
			get { return this.keyDictionary.Values; }
		}

		ICollection<IConfigKey> IDictionary<string, IConfigKey>.Values
		{
			get { return this.Keys; }
		}

		public IConfigKey this[string keyName]
		{
			get { return this.keyDictionary[keyName]; }
			set { this.keyDictionary[keyName] = value; }
		}
		#endregion

		#region Methods

		public int IndexOf(IConfigKey configKey)
		{
			return this.keyList.IndexOf(configKey);
		}

		public void Insert(int index, IConfigKey configKey)
		{
			this.keyDictionary.Add(configKey.Name, configKey);
			this.keyList.Insert(index, configKey);
		}

		public void RemoveAt(int index)
		{
			this.keyDictionary.Remove(this.keyList[index].Name);
			this.keyList.RemoveAt(index);
		}

		public void Add(IConfigKey configKey)
		{
			this.keyDictionary.Add(configKey.Name, configKey);
			this.keyList.Add(configKey);
		}

		public void Clear()
		{
			this.keyList.Clear();
			this.keyDictionary.Clear();
		}

		public bool Contains(IConfigKey configKey)
		{
			return this.keyDictionary.ContainsKey(configKey.Name) &&
				this.keyDictionary[configKey.Name].Equals(configKey);
		}

		public void CopyTo(IConfigKey[] array, int arrayIndex)
		{
			this.keyList.CopyTo(array, arrayIndex);
		}

		public bool Remove(IConfigKey configKey)
		{
			bool didRemoved = this.keyList.Remove(configKey);

			if (didRemoved)
			{
				this.keyDictionary.Remove(configKey.Name);
			}

			return didRemoved;
		}

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

		public bool ContainsKey(string keyName)
		{
			return this.keyDictionary.ContainsKey(keyName);
		}

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
