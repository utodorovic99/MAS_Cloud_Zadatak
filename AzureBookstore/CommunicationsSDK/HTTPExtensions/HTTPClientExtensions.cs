using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CommunicationsSDK.HTTPExtensions
{
	/// <summary>
	/// Extension class for <see cref="HttpClient"/>
	/// </summary>
	public static class HTTPClientExtensions
	{
		/// <summary>
		/// Initializes HTTP client to have base address of <paramref name="baseAddress"/>.
		/// </summary>
		/// <param name="httpClient">HTTP client whose base address is being set.</param>
		/// <param name="baseAddress">Address which is set as base address for <paramref name="httpClient"/>.</param>
		/// <returns>Same HTTP client instance with properly set value of base address.</returns>
		public static HttpClient WithBaseAddress(this HttpClient httpClient, Uri baseAddress)
		{
			httpClient.BaseAddress = baseAddress;

			return httpClient;
		}

		/// <summary>
		/// Initializes HTTP client to have request timeout of <paramref name="requestTimeoutInSeconds"/>.
		/// </summary>
		/// <param name="httpClient">HTTP client whose request timeout is being set.</param>
		/// <param name="requestTimeoutInSeconds">Request timeout in seconds.</param>
		/// <returns>Same HTTP client instance with properly set value of request timeout.</returns>
		public static HttpClient WithRequestTimeout(this HttpClient httpClient, int requestTimeoutInSeconds)
		{
			httpClient.Timeout = TimeSpan.FromMilliseconds(requestTimeoutInSeconds);

			return httpClient;
		}

		/// <summary>
		/// Initializes HTTP client to have default headers.
		/// </summary>
		/// <param name="httpClient">HTTP client whose headers are set.</param>
		/// <returns>Same HTTP client instance with properly set value of default headers.</returns>
		public static HttpClient WithDefaultRequestHeaders(this HttpClient httpClient)
		{
			httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return httpClient;
		}
	}
}
