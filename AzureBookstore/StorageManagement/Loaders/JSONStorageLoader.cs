using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StorageManagement
{
	/// <summary>
	/// Storage loader for <typeparamref name="T"/> type of data stored inside JSON file.
	/// </summary>
	/// <typeparam name="T">Type of objects stored inside storage.</typeparam>
	public sealed class JSONStorageLoader<T>
		: IStorageLoader<T> where T : class
	{
		private string storageDataFileName;
		private IEnumerable<T> loadedData;

		/// <summary>
		/// Initializes new instance of <see cref="JSONStorageLoader"/>.
		/// </summary>
		/// <param name="storageDataFile">Name of file containing data.</param>
		/// <remarks>
		/// <paramref name="storageDataFile"/> contains file name, not its full name.
		/// </remarks>
		public JSONStorageLoader(string storageDataFile)
		{
			this.storageDataFileName = storageDataFile;
		}

		/// <inheritdoc/>
		public IEnumerable<T> LoadedData => loadedData;

		/// <inheritdoc/>
		public void Load()
		{
			if (!TryReadStorageFile(storageDataFileName, out string jsonContent)
				|| !TryDeserializeData(jsonContent, out loadedData))
			{
				loadedData = Enumerable.Empty<T>();
			};
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			loadedData = Enumerable.Empty<T>();
		}

		/// <summary>
		/// Tries to read storage from JSON file.
		/// </summary>
		/// <param name="fullFileName">Full name of JSON file to load.</param>
		/// <param name="jsonContent">Loaded JSON content.</param>
		/// <returns><c>True</c> if storage is successfully loaded; otherwise returns <c>false</c>.</returns>
		private bool TryReadStorageFile(string fullFileName, out string jsonContent)
		{
			jsonContent = string.Empty;

			if (!File.Exists(fullFileName))
			{
				return false;
			}

			try
			{
				jsonContent = File.ReadAllText(fullFileName);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Tries to deserialize data from <paramref name="dataJSON"/>.
		/// </summary>
		/// <param name="dataJSON">JSON file to deserialize.</param>
		/// <param name="deserializedData">Resulting deserialized data objects.</param>
		/// <returns><c>True</c> if data successfully deserialized; otherwise returns <c>false</c>.</returns>
		private bool TryDeserializeData(string dataJSON, out IEnumerable<T> deserializedData)
		{
			try
			{
				JsonReader jsonReader = new JsonTextReader(new StringReader(dataJSON));
				JsonSerializer jsonSerializer = new JsonSerializer();

				deserializedData = jsonSerializer.Deserialize<List<T>>(jsonReader);
				return true;
			}
			catch
			{
				deserializedData = Enumerable.Empty<T>();
				return false;
			}
		}
	}
}
