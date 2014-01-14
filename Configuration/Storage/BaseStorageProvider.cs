using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Storage
{
	/// <summary>
	/// Provide basic functionality for storage providers
	/// </summary>
	public abstract class BaseStorageProvider : IStorageProvider
	{
		#region Properties

		/// <summary>
		/// Gets value indicates if the object is disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Gets value indicates if the storage isn't writable.
		/// </summary>
		public bool IsReadOnly { get; protected set; } 
		#endregion

		#region Ctor

		/// <summary>
		/// Finalizes the object, and releases unmanaged resources.
		/// </summary>
		~BaseStorageProvider()
		{
			Dispose(false);
		}
		#endregion

		/// <summary>
		/// Loads configuration from storage.
		/// </summary>
		/// <returns>New configuration object that repersents the saved configuration</returns>
		public abstract Configuration Load();

		/// <summary>
		/// Loads the selected configuration from storage.
		/// </summary>
		/// <param name="confToLoad">
		/// Confguration object which contains the keys and values to load from storage
		/// </param>
		/// <returns>The loaded configuration</returns>
		public abstract Configuration Load(Configuration confToLoad);

		/// <summary>
		/// Update the storage with the keys in the configuration
		/// </summary>
		/// <param name="configuration">Configuration which hold the keys to update</param>
		public void Update(Configuration configuration)
		{
			Update(configuration, false);
		}

		/// <summary>
		/// Update the storage with the keys in the configuration
		/// </summary>
		/// <param name="configuration">Configuration which hold the keys to update</param>
		/// <param name="addMissingKeys">true if new keys should be added to the storage</param>
		public abstract void Update(Configuration configuration, bool addMissingKeys);

		/// <summary>
		/// Save the entire configuration to the storage.
		/// </summary>
		/// <param name="configuration">The configuration to save</param>
		public abstract void Save(Configuration configuration);

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				Dispose(true);
				this.IsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Throws an exception if this instance is disposed
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException("INIStorageProvider");
			}
		}

		/// <summary>
		/// Throw an exception if this instance is read only
		/// </summary>
		protected void ThrowIfReadOnly()
		{
			ThrowIfDisposed();

			if (this.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot change read only storage");
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="isDisposing">
		/// true to release both managed and unmanaged resources; false to release only unmanaged resources.
		/// </param>
		protected abstract void Dispose(bool isDisposing);
	}
}
