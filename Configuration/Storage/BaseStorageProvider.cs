using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Storage
{
	public abstract class BaseStorageProvider : IStorageProvider
	{
		public bool IsDisposed { get; private set; }

		public bool IsReadOnly { get; protected set; }

		public abstract Configuration Load();

		public abstract Configuration Load(Configuration confToLoad);

		public void Update(Configuration configuration)
		{
			Update(configuration, false);
		}

		public abstract void Update(Configuration configuration, bool addMissingKeys);

		public abstract void Save(Configuration configuration);

		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				Dispose(true);
				this.IsDisposed = true; 
			}
		}

		protected void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException("INIStorageProvider");
			}
		}

		protected abstract void Dispose(bool isDisposing);
	}
}
