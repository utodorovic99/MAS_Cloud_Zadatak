using System.Runtime.Serialization;

namespace UsersService.Storage.Users
{
	/// <summary>
	/// Class representing user data in the model.
	/// </summary>
	[DataContract]
	internal class UserStorageModel
	{
		//Note: Even tho this class is not exposed via WCF it must be decorated with attributes.
		// reason for this is that it is used in reliable dictionary which is replicated through network.

		/// <summary>
		/// Gets or sets username.
		/// </summary>
		[DataMember]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets saldo.
		/// </summary>
		[DataMember]
		public float Saldo { get; set; }
	}
}
