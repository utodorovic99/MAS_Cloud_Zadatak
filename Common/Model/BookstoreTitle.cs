﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Common.Model
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
		public BookstoreTitle()
		{
		}

		/// <summary>
		/// Gets or sets <strong>unique</strong> bookstore title name used as its identifier.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(Name))]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets price of title copy.
		/// </summary>
		[DataMember]
		[JsonPropertyName(nameof(Price))]
		public float Price { get; set; } = float.MaxValue;
	}
}
