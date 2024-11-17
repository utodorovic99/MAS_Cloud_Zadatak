using BookstoreServiceContract.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BookstoreServiceContract.Model
{
	/// <summary>
	/// Book purchase enlistment result.
	/// </summary>
	[DataContract]
	public class BookstoreEnlistPurchaseResult
	{
		/// <summary>
		/// Gets or sets status of enlistment.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(Status))]
		public BookstoreEnlistPurchaseStatus Status { get; set; }

		/// <summary>
		/// Gets or sets purchase id if enlistment succeeded.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(PurchaseId))]
		public uint PurchaseId { get; set; }
	}
}
