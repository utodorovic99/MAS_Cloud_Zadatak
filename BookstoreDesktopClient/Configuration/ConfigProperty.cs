namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Attribute for decoration of properties loaded from configuration.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	internal sealed class ConfigProperty : Attribute
	{
		/// <summary>
		/// Gets or sets identifier used in App.config.
		/// </summary>
		public string Identifier { get; set; }

		/// <summary>
		/// Gets or set type of data.
		/// </summary>
		public Type DataType { get; set; }
	}
}
