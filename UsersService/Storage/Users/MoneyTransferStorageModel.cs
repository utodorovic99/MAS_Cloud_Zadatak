using System.Runtime.Serialization;

namespace UsersService.Storage.Users
{
	/// <summary>
	/// Class representing money transfer data in the model.
	/// </summary>
	[DataContract]
	internal class MoneyTransferStorageModel
	{
		//Note: Even tho this class is not exposed via WCF it must be decorated with attributes.
		// reason for this is that it is used in reliable dictionary which is replicated through network.

		[DataMember]
		public uint TransferId { get; set; }

		/// <summary>
		/// Gets or sets username.
		/// </summary>
		[DataMember]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets amount.
		/// </summary>
		[DataMember]
		public float Amount { get; set; }
	}
}
