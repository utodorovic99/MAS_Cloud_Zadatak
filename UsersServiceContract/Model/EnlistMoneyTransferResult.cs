using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UsersServiceContract.Enums
{
	/// <summary>
	/// Money transfer enlistment result.
	/// </summary>
	[DataContract]
	public class EnlistMoneyTransferResult
	{
		/// <summary>
		/// Gets or sets status of enlistment.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(Status))]
		public EnlistMoneyTransferStatus Status { get; set; }

		/// <summary>
		/// Gets or sets transaction id if enlistment succeeded.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(TransactionId))]
		public uint TransactionId { get; set; }
	}
}
