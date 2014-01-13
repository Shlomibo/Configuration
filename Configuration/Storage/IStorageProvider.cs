using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Storage
{
	/// <summary>
	/// Provides common interface to save configuration into storage
	/// </summary>
	public interface IStorageProvider : IDisposable
	{
		#region Properties

		/// <summary>
		/// Gets value indicates if the object is disposed.
		/// </summary>
		public bool IsDisposed { get; }

		/// <summary>
		/// Gets value indicates if the storage isn't writable.
		/// </summary>
		public bool IsReadOnly { get; }
		#endregion

		#region Methods

		/// <summary>
		/// Loads configuration from storage.
		/// </summary>
		/// <returns>New configuration object that repersents the saved configuration</returns>
		Configuration Load();

		/// <summary>
		/// Loads the selected configuration from storage.
		/// </summary>
		/// <param name="confToLoad">
		/// Confguration object which contains the keys and values to load from storage
		/// </param>
		/// <returns>The loaded configuration</returns>
		Configuration Load(Configuration confToLoad);

		/// <summary>
		/// Update the storage with the keys in the configuration
		/// </summary>
		/// <param name="configuration">Configuration which hold the keys to update</param>
		void Update(Configuration configuration);

		/// <summary>
		/// Update the storage with the keys in the configuration
		/// </summary>
		/// <param name="configuration">Configuration which hold the keys to update</param>
		/// <param name="addMissingKeys">true if new keys should be added to the storage</param>
		void Update(Configuration configuration, bool addMissingKeys);

		/// <summary>
		/// Save the entire configuration to the storage.
		/// </summary>
		/// <param name="configuration">The configuration to save</param>
		void Save(Configuration configuration); 
		#endregion
	}
}
