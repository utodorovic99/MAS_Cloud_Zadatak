using BookstoreServiceContracts.Model;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreAPI.FabricClients
{
	internal sealed class BookstoreFabricClient
	{
		private FabricClient fabricClient;

		/// <summary>
		/// Initializes new instance of <see cref="BookstoreFabricClient"/>
		/// </summary>
		public BookstoreFabricClient()
		{
			fabricClient = new FabricClient();
		}

		public async Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			FabricClient fabricClient = new FabricClient();
			int numberOfPartitions = (await fabricClient.QueryManager.GetPartitionListAsync(Program.Configuration.BookstoreServiceUri)).Count;

			int partitionToHit = PartitionIdPseudoRandomizer.RandomPartitionId(numberOfPartitions);

			return Enumerable.Empty<BookstoreTitle>();
		}
	}
}
