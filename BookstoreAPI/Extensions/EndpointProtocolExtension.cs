using System.Fabric.Description;

namespace BookstoreAPI.Extensions
{
	/// <summary>
	/// Extension class for <see cref="EndpointProtocol"/>.
	/// </summary>
	internal static class EndpointProtocolExtension
	{
		private const string HttpString = "http";
		private const string HttpsString = "https";

		/// <summary>
		/// Converts <paramref name="protocol"/> to string.
		/// </summary>
		/// <param name="protocol">Protocol to convert.</param>
		/// <returns>String representation for <paramref name="protocol"/>.</returns>
		public static string FormattedString(this EndpointProtocol protocol)
		{
			if (protocol.Equals(EndpointProtocol.Http))
			{
				return HttpString;
			}

			if (protocol.Equals(EndpointProtocol.Https))
			{
				return HttpsString;
			}

			return protocol.ToString();
		}
	}
}
