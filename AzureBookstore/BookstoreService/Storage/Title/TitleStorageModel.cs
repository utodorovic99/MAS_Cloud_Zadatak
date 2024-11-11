using System.Runtime.Serialization;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Class representing storage data model for bookstore title.
	/// </summary>
	[DataContract]
	internal class TitleStorageModel
	{
		//Note: Even tho this class is not exposed via WCF it must be decorated with attributes.
		// reason for this is that it is used in reliable dictionary which is replicated through network.

		/// <summary>
		/// Initializes new instance of <see cref="TitleStorageModel"/>.
		/// </summary>
		public TitleStorageModel()
		{
			Name = string.Empty;
			Copies = 0;
		}

		/// <summary>
		/// Initializes new instance of <see cref="TitleStorageModel"/>.
		/// </summary>
		/// <param name="name">Gets or sets <strong>unique</strong> bookstore title name used as its identifier.</param>
		/// <param name="copies">Gets or sets number of copies currently available.</param>
		public TitleStorageModel(string name, uint copies)
		{
			Name = name;
			Copies = copies;
		}

		/// <summary>
		/// Gets or sets <strong>unique</strong> bookstore title name used as its identifier.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets number of copies currently available.
		/// </summary>
		[DataMember]
		public uint Copies { get; set; }
	}
}
