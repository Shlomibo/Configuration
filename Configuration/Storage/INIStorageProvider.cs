using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.Storage
{
	public sealed class INIStorageProvider : IStorageProvider
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

		public bool IsDisposed { get; private set; }

		public bool IsReadOnly { get; private set; }

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

		#region Methods

		private void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException("INIStorageProvider");
			}
		}

		private Stream GetStream()
		{
			return this.IsReadOnly
				? File.OpenRead(this.Path)
				: File.Open(this.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		}

		public Configuration Load()
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

		public Configuration Load(Configuration confToLoad)
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

		public void Update(Configuration configuration)
		{
			Update(configuration, false);
		}

		public void Update(Configuration configuration, bool addMissingKeys)
		{
			ThrowIfDisposed();
			Reset();
			string tempPath = System.IO.Path.GetTempFileName();

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
						}
						else if (lastKey != null)
						{
							line = GetValue(lastKey, trimmed) ?? line;
						}
					}

					newConfig.WriteLine(line);
				}
			}

			this.writer = null;
			this.reader = null;
			this.iniStream.Dispose();
			File.Delete(this.Path);
			File.Move(tempPath, this.Path);
			this.iniStream = GetStream();
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
						(from val in namedVal.Values.Cast<object>().Take(namedVal.Values.Count - 1)
						 select val ?? "").ToArray());
				}
			}

			return valueStr;
		}

		public void Save(Configuration configuration)
		{
			
		}

		public void Dispose()
		{
			if (!this.IsDisposed)
			{
				if (this.iniStream != null)
				{
					this.iniStream.Flush();
					this.iniStream.Dispose();
				}

				this.IsDisposed = true;
			}
		}
		#endregion
	}
}
