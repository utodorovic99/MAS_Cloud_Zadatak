using Common.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Common.Model
{
	/// <summary>
	/// Class representing purchase response.
	/// </summary>
	[DataContract]
	public class PurchaseResponse
	{
		/// <summary>
		/// Initializes new instance of <see cref="PurchaseResponse"/>.
		/// </summary>
		public PurchaseResponse()
		{
		}

		/// <summary>
		/// Gets or sets status of book purchase.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(Status))]
		public PurchaseResponseStatus Status { get; set; } = PurchaseResponseStatus.Fail;
	}
}
