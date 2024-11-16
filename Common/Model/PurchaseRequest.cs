using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Common.Model
{
	/// <summary>
	/// Model class representing single title purchase request.
	/// </summary>
	[DataContract]
	public class PurchaseRequest
	{
		/// <summary>
		/// Initializes new instance of <see cref="PurchaseRequest"/>
		/// </summary>
		public PurchaseRequest()
		{
		}

		/// <summary>
		/// Gets or sets name of user who is purchasing the book.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(User))]
		public string User { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets title which is being purchased.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(Title))]
		public string Title { get; set; } = string.Empty;
	}
}
