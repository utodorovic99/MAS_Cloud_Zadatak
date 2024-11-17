using System.Runtime.Serialization;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Class representing storage data of bookstore title purchase.
	/// </summary>
	[DataContract]
	internal class PurchaseStorageModel
	{
		//Note: Even tho this class is not exposed via WCF it must be decorated with attributes.
		// reason for this is that it is used in reliable dictionary which is replicated through network.

		/// <summary>
		/// Initializes new instance of <see cref="PurchaseStorageModel"/>.
		/// </summary>
		public PurchaseStorageModel()
		{
		}

		/// <summary>
		/// Gets or sets <strong>unique</strong> bookstore title name used as its identifier.
		/// </summary>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets unique purchase id.
		/// </summary>
		[DataMember]
		public uint PurchaseId { get; set; }
	}
}
