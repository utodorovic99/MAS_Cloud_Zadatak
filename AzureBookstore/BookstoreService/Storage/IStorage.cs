using System;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Contract for title storage.
	/// </summary>
	internal interface IStorage : IDisposable
	{
		/// <summary>
		/// Initializes storage.
		/// </summary>
		void InitializeStorage();
	}
}