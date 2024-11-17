using System.Threading;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Generator of transaction identifiers.
	/// </summary>
	/// <remarks>
	/// Generated identifiers can repeat but in acceptable interval.
	/// </remarks>
	internal static class PurchaseIdGenerator
	{
		private static int purchaseId = int.MinValue;

		/// <summary>
		/// Generates new purchase identifier.
		/// </summary>
		/// <returns>New purchase identifier.</returns>
		public static uint Generate()
		{
			unsafe //Prevents overflow exception when ulong limit is reached. At that moment counting loop will restart.
			{
				return (uint)Interlocked.Increment(ref purchaseId); //Provide thread safety with atomic operation.
			}
		}
	}
}
