using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSPath = System.IO.Path;
using Utilities.Extansions;

namespace Configuration.Storage
{
	/// <summary>
	/// Provides storage of configuration into ini files.
	/// </summary>
	public sealed class INIStorageProvider : BaseStorageProvider
	{
		#region Consts

		private const string UNNAMED_NAME = "unnamed";

		private const string COMMENT_START = "#";

		private const string KEY_START = "[";
		private const string KEY_END = "]";

		private const char NAME_VAL_SEPARATOR = '=';
		#endregion

		#region Fields

		private Stream iniStream = null;
		private StreamReader reader;
		private StreamWriter writer;
		private int unnamedIndex = 0;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the path to the ini file.
		/// </summary>
		public string Path { get; private set; }

		private StreamReader Reader
		{
			get
			{
				ThrowIfDisposed();

				if (this.iniStream == null)
				{
					this.iniStream = GetStream();
				}

				if (this.reader == null)
				{
					this.reader = new StreamReader(this.iniStream);
				}

				return this.reader;
			}
		}

		private StreamWriter Writer
		{
			get
			{
				ThrowIfDisposed();

				if (this.IsReadOnly)
				{
					throw new NotSupportedException("Storage provider was openned in read only mode");
				}

				if (this.iniStream == null)
				{
					this.iniStream = GetStream();
				}

				if (this.writer == null)
				{
					this.writer = new StreamWriter(this.iniStream);
				}

				return this.writer;
			}
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new ini storage provider.
		/// </summary>
		/// <param name="path">The path to the ini file</param>
		/// <param name="isReadOnly">true if only read operations should be supported; otherwise false</param>
		public INIStorageProvider(string path, bool isReadOnly = false)
		{
			this.IsReadOnly = isReadOnly;

			string fileName = FSPath.GetFileName(path);

			if (path.Any(@char => FSPath.GetInvalidPathChars().Contains(@char)) ||
				string.IsNullOrWhiteSpace(fileName) ||
				fileName.Any(@char => FSPath.GetInvalidFileNameChars().Contains(@char)))
			{
				throw new ArgumentException("Invalid path", "path");
			}

			if (isReadOnly && !File.Exists(path))
			{
				throw new InvalidOperationException("Cannot create file, in read only mode");
			}

			this.Path = path;
		}
		#endregion

		#region Methods

		private Stream GetStream()
		{
			return this.IsReadOnly
				? File.OpenRead(this.Path)
				: File.Open(this.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		}

		/// <summary>
		/// Loads configuration from storage.
		/// </summary>
		/// <returns>New configuration object that repersents the saved configuration</returns>
		public override Configuration Load()
		{
			ThrowIfDisposed();
			Reset();

			Configuration conf = new Configuration();
			CustomConfigKey lastKey = null;

			while (!this.Reader.EndOfStream)
			{
				string line = this.Reader.ReadLine().Trim();

				if (!ShouldIgnore(line))
				{
					if (IsKey(line))
					{
						lastKey = new CustomConfigKey(GetKeyName(line));
						conf.Add(lastKey);
					}
					else if (lastKey != null)
					{
						ParseValue(line, lastKey);
					}
				}
			}

			return conf;
		}

		private bool IsKey(string line)
		{
			return line.StartsWith(KEY_START) && line.EndsWith(KEY_END);
		}

		private bool ShouldIgnore(string line)
		{
			return (line == "") || line.StartsWith(COMMENT_START);
		}

		private void ParseValue(string line, CustomConfigKey key)
		{
			string[] splittedVal = line.Split(NAME_VAL_SEPARATOR);

			if ((splittedVal.Length == 1) || (splittedVal[0].Trim() == ""))
			{
				key.Add(GetUnnamedName(), line, false);
			}
			else
			{
				key.Add(splittedVal[0], splittedVal[1]);
			}
		}

		private string GetUnnamedName()
		{
			return UNNAMED_NAME + this.unnamedIndex++;
		}

		private string GetKeyName(string line)
		{
			return line.Substring(1, line.Length - 2);
		}

		private void Reset()
		{
			if (this.iniStream != null)
			{
				this.iniStream.Seek(0, SeekOrigin.Begin);
			}
		}

		/// <summary>
		/// Loads the selected configuration from storage.
		/// </summary>
		/// <param name="confToLoad">
		/// Confguration object which contains the keys and values to load from storage
		/// </param>
		/// <returns>The loaded configuration</returns>
		public override Configuration Load(Configuration confToLoad)
		{
			ThrowIfDisposed();
			Reset();

			IConfigKey lastKey = null;

			while (!this.Reader.EndOfStream)
			{
				string line = this.Reader.ReadLine().Trim();

				if (!ShouldIgnore(line))
				{
					if (IsKey(line))
					{
						lastKey = confToLoad.ContainsKey(GetKeyName(line))
							? confToLoad[GetKeyName(line)]
							: null;
					}
					else if (lastKey != null)
					{
						ParseValue(line, lastKey);
					}
				}
			}

			return confToLoad;
		}

		private void ParseValue(string line, IConfigKey key)
		{
			string[] splittedVal = line.Split(NAME_VAL_SEPARATOR);

			if (((splittedVal.Length == 1) || (splittedVal[0].Trim() == "")) &&
				(key.DefaultValueName != null))
			{
				key.Values[key.DefaultValueName].AddParsedString(line);
			}
			else if (key.Values.ContainsKey(splittedVal[0]))
			{
				key.Values[splittedVal[0]].ParseString(splittedVal[1]);
			}
		}

		/// <summary>
		/// Update the storage with the keys in the configuration
		/// </summary>
		/// <param name="configuration">Configuration which hold the keys to update</param>
		/// <param name="addMissingKeys">true if new keys should be added to the storage</param>
		public override void Update(Configuration configuration, bool addMissingKeys)
		{
			ThrowIfReadOnly();
			Reset();
			string tempPath = FSPath.GetTempFileName();
			HashSet<string> existingKeys = new HashSet<string>();

			using (var newConfig = new StreamWriter(tempPath))
			{
				IConfigKey lastKey = null;

				while (!this.Reader.EndOfStream)
				{
					string line = this.Reader.ReadLine();
					string trimmed = line.Trim();

					if (!ShouldIgnore(trimmed))
					{
						if (IsKey(trimmed))
						{
							lastKey = configuration.ContainsKey(GetKeyName(trimmed))
								? configuration[GetKeyName(trimmed)]
								: null;
							existingKeys.Add(lastKey.Name);
						}
						else if (lastKey != null)
						{
							line = GetValue(lastKey, trimmed) ?? line;
						}
					}

					newConfig.WriteLine(line);
				}

				if (addMissingKeys)
				{
					IEnumerable<IConfigKey> missingKeys = configuration.Where((IConfigKey key) => 
																				  !existingKeys.Contains(key.Name));

					WriteKeys(missingKeys, newConfig);
				}
			}

			ReplaceFile(tempPath);
		}

		private void ReplaceFile(string replaceWith)
		{
			this.writer = null;
			this.reader = null;
			this.iniStream.Flush();
			this.iniStream.Dispose();

			if (FSPath.GetPathRoot(FSPath.GetFullPath(replaceWith)) == FSPath.GetPathRoot(FSPath.GetFullPath(this.Path)))
			{
				File.Replace(replaceWith, this.Path, null);
			}
			else
			{
				File.Delete(this.Path);
				File.Copy(replaceWith, this.Path);
				File.Delete(replaceWith);
			}

			this.iniStream = GetStream();
		}

		private void WriteKeys(IEnumerable<IConfigKey> keys, StreamWriter configStream)
		{
			foreach (IConfigKey key in keys)
			{
				configStream.WriteLine();
				configStream.WriteLine("[{0}]", key.Name);
				configStream.WriteLine();

				foreach (INamedValue value in key)
				{
					if (value.IsNameVisible)
					{
						configStream.WriteLine("{0}={1}", value.Name, value.Value ?? "");
					}
					else
					{
						foreach (object val in value.Values)
						{
							configStream.WriteLine(val);
						}
					}
				}
			}

			configStream.WriteLine();
		}

		private string GetValue(IConfigKey key, string line)
		{
			string[] splittedVal = line.Split(NAME_VAL_SEPARATOR);
			INamedValue namedVal = key.Values[key.DefaultValueName];
			string valueStr = null;

			if (((splittedVal.Length != 1) && (splittedVal[0].Trim() != "")) ||
				(key.DefaultValueName == null))
			{
				valueStr = string.Format("{0}={1}", namedVal.Name, namedVal.Value.ToString());
			}
			else if (key.Values.ContainsKey(splittedVal[0]))
			{
				if (namedVal.Values.Count == 1)
				{
					valueStr = namedVal.Value.ToString();
				}
				else
				{
					valueStr = string.Join(
						Environment.NewLine,
						namedVal.Values.Cast<object>()
									   .Select(val => val.NullableToString()).ToArray());
				}
			}

			return valueStr;
		}

		/// <summary>
		/// Save the entire configuration to the storage.
		/// </summary>
		/// <param name="configuration">The configuration to save</param>
		public override void Save(Configuration configuration)
		{
			ThrowIfReadOnly();
			Reset();
			string tempPath = FSPath.GetTempFileName();

			using (var newConfig = new StreamWriter(tempPath))
			{
				WriteKeys(configuration, newConfig);
			}

			ReplaceFile(tempPath);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="isDisposing">
		/// true to release both managed and unmanaged resources; false to release only unmanaged resources.
		/// </param>
		protected override void Dispose(bool isDisposing)
		{
			if (this.iniStream != null)
			{
				this.iniStream.Flush();
				this.iniStream.Dispose();
			}
		}
		#endregion
	}
}
