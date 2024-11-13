using System;
using System.Collections.Generic;

namespace StorageManagement
{
	/// <summary>
	/// Interface for storage loader.
	/// </summary>
	/// <typeparam name="T">Type of objects which are loaded.</typeparam>
	public interface IStorageLoader<T> : IDisposable
	{
		/// <summary>
		/// Gets loaded data.
		/// </summary>
		IEnumerable<T> LoadedData { get; }

		/// <summary>
		/// Loads data from storage.
		/// </summary>
		void Load();
	}
}