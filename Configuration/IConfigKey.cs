using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public interface IConfigKey : IEnumerable<KeyValuePair<string, string>>
	{
		#region Properties

		string Name { get; }
		string this[string value] { get; }
		IEnumerable<string> Values { get; }
		IEnumerable<string> ValuesData { get; }
		int Count { get; }
		#endregion

		#region Methods

		bool ContainsValue(string value);
		bool TryGetValue(string value, out string valueData);
		#endregion
	}
}
