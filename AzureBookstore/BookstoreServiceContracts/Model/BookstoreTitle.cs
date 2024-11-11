using System.Runtime.Serialization;

namespace BookstoreServiceContracts.Model
{
	/// <summary>
	/// Represents single title from bookstore.
	/// </summary>
	[DataContract]
	public sealed class BookstoreTitle
	{
		/// <summary>
		/// Creates new instance of<see cref="BookstoreTitle"/>
		/// </summary>
		/// <param name="name">name of book title.</param>
		public BookstoreTitle(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Gets or sets <strong>unique</strong> bookstore title name used as its identifier.
		/// </summary>
		[DataMember]
		public string Name { get; set; }
	}
}
