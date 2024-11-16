using System;

namespace StorageManagement
{
	/// <summary>
	/// Contract for title storage.
	/// </summary>
	public interface IStorage : IDisposable
	{
		/// <summary>
		/// Initializes storage.
		/// </summary>
		void InitializeStorage();
	}
}