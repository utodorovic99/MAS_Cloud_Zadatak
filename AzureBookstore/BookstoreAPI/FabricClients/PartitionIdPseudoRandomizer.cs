using System;

namespace BookstoreAPI.FabricClients
{
	/// <summary>
	/// Class implementing primitive load balancing randomly using partitions.
	/// </summary>
	internal static class PartitionIdPseudoRandomizer
	{
		private static readonly Random random = new Random();

		/// <summary>
		/// Creates pseudo random partition id between 0 and <paramref name="totalNumberOfPartitions"/>.
		/// </summary>
		/// <param name="totalNumberOfPartitions">Total number of partitions as range maximum.</param>
		/// <returns>Pseudo random partition id.</returns>
		public static int RandomPartitionId(int totalNumberOfPartitions)
		{
			//There is no such thing as random in software. This strategy is actually deterministic. :)
			return random.Next(maxValue: totalNumberOfPartitions);
		}
	}
}
