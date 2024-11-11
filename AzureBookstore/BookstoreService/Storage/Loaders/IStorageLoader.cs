using System;
using System.Collections.Generic;

namespace BookstoreService.Storage
{
	/// <summary>
	/// Interface for storage loaders.
	/// </summary>
	/// <typeparam name="T">Type of object which are loaded.</typeparam>
	internal interface IStorageLoader<T> : IDisposable
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