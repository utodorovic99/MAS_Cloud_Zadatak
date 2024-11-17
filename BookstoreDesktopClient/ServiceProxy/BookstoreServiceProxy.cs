using BookstoreDesktopClient.Resources;
using BookstoreDesktopClient.ViewModel;
using Common.Model;
using CommunicationsSDK.HTTPExtensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreDesktopClient.ServiceProxy
{
	/// <summary>
	/// Represents service proxy for bookstore service.
	/// </summary>
	internal sealed class BookstoreServiceProxy : IBookstoreServiceProxy
	{
		private const int MaxResponseTimeoutMs = 20000;
		private readonly HttpClient bookstoreServiceHttpClient;

		private event PurchaseResponseReceived purchaseReceivedEvent;

		/// <summary>
		/// Initializes new instance of <see cref="BookstoreServiceProxy"/>
		/// </summary>
		public BookstoreServiceProxy()
		{
			bookstoreServiceHttpClient = new HttpClient()
				.WithBaseAddress(App.Configuration.BookStoreServiceConfig.Uri)
				.WithRequestTimeout(MaxResponseTimeoutMs)
				.WithDefaultRequestHeaders();
		}

		/// <inheritdoc/>
		public event PurchaseResponseReceived PurchaseReceivedEvent
		{
			add
			{
				purchaseReceivedEvent += value;
			}

			remove
			{
				purchaseReceivedEvent -= value;
			}
		}

		/// <inheritdoc/>
		public void SendPurchaseRequest(PurchaseRequest purchaseRequest)
		{
			purchaseRequest.User = App.Configuration.UserConfig.Username;
			HttpContent purchaseRequestHttpContent = JsonContent.Create(purchaseRequest);
			CancellationToken cancellationToken = new CancellationTokenSource(MaxResponseTimeoutMs).Token;

			string requestName = "Title/PurchaseTitle/";
			Task<HttpResponseMessage> requestTask = bookstoreServiceHttpClient.PostAsync(requestName, purchaseRequestHttpContent, cancellationToken);

			requestTask.ContinueWith(async t =>
			{
				if (!requestTask.Result.IsSuccessStatusCode)
				{
					PublishFailureResponseFor(purchaseRequest.Title);
					return;
				}

				string responseBody = await t.Result.Content.ReadAsStringAsync();
				PurchaseResponse receivedResponse = JsonConvert.DeserializeObject<PurchaseResponse>(responseBody);

				PublishReceivedResponseFor(purchaseRequest.Title, receivedResponse);

			}, TaskContinuationOptions.OnlyOnRanToCompletion)
			.ContinueWith(t =>
			{
				t.Exception?.Handle(ex =>
				{
					return true;
				});

				if (t.IsCanceled)
				{
					PublishTimedOutResponseFor(purchaseRequest.Title);
				}
				else
				{
					PublishFailureResponseFor(purchaseRequest.Title);
				}
			}, TaskContinuationOptions.NotOnRanToCompletion);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			CancellationToken cancellationToken = new CancellationTokenSource(MaxResponseTimeoutMs).Token;

			string requestName = "Title/GetAll/";
			HttpResponseMessage httpResponseMessage = await bookstoreServiceHttpClient.GetAsync(requestName, cancellationToken);

			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				return Enumerable.Empty<BookstoreTitle>();
			}

			return await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<BookstoreTitle>>(cancellationToken);
		}

		/// <summary>
		/// Executes type of publication when response successfully received.
		/// </summary>
		/// <param name="title">Title of book whose purchase has been requested.</param>
		/// <param name="purchaseResponse">Received purchase response.</param>
		private void PublishReceivedResponseFor(string title, PurchaseResponse purchaseResponse)
		{
			purchaseReceivedEvent.Invoke(new PurchaseResponseWrapper(title, purchaseResponse));
		}

		/// <summary>
		/// Executes type of publication when request timed out.
		/// </summary>
		/// <param name="title">Title of book whose purchase has been requested.</param>
		private void PublishTimedOutResponseFor(string title)
		{
			purchaseReceivedEvent.Invoke(new PurchaseResponseWrapper()
			{
				Status = false,
				Message = BookstoreResources.BookPurschaseResult_REQUEST_TIMED_OUT,
				Title = title,
			});
		}

		/// <summary>
		/// Executes type of publication when failure occurred.
		/// </summary>
		/// <param name="title">Title of book whose purchase has been requested.</param>
		private void PublishFailureResponseFor(string title)
		{
			purchaseReceivedEvent.Invoke(new PurchaseResponseWrapper()
			{
				Status = false,
				Message = BookstoreResources.BookPurschaseResult_REQUEST_FAILED,
				Title = title,
			});
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			bookstoreServiceHttpClient.Dispose();
		}
	}
}
