using System.Threading;

namespace UsersService.Storage.Users
{
	/// <summary>
	/// Generator of transaction identifiers.
	/// </summary>
	/// <remarks>
	/// Generated identifiers can repeat but in acceptable interval.
	/// </remarks>
	internal static class TransferIdGenerator
	{
		private static int transactionId = int.MinValue;

		/// <summary>
		/// Generates new transaction identifier.
		/// </summary>
		/// <returns>New transaction identifier.</returns>
		public static uint Generate()
		{
			unsafe //Prevents overflow exception when ulong limit is reached. At that moment counting loop will restart.
			{
				return (uint)Interlocked.Increment(ref transactionId); //Provide thread safety with atomic operation.
			}
		}
	}
}
