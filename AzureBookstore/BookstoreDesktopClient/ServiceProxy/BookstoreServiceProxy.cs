using BookstoreDesktopClient.Resources;
using BookstoreDesktopClient.ViewModel;
using BookstoreServiceContracts.Model;
using CommunicationsSDK.HTTPExtensions;
using PurchaseDataModel;
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
		private const int MaxResponseTimeoutMs = 5000;
		private readonly HttpClient bokstoreServiceHttpClient;

		private event PurchaseResponseReceived purchaseReceivedEvent;

		/// <summary>
		/// Initializes new instance of <see cref="BookstoreServiceProxy"/>
		/// </summary>
		public BookstoreServiceProxy()
		{
			bokstoreServiceHttpClient = new HttpClient()
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

			string requestName = "Title/PurchaseTitle";
			Task<HttpResponseMessage> requestTask = bokstoreServiceHttpClient.PostAsync(requestName, purchaseRequestHttpContent, cancellationToken);

			requestTask.ContinueWith(async t =>
			{
				if (!requestTask.Result.IsSuccessStatusCode)
				{
					PublishFailureResponseFor(purchaseRequest.Title);
					return;
				}

				var receivedResponse = await requestTask.Result.Content.ReadFromJsonAsync<PurchaseResponse>(cancellationToken);
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

			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		/// <inheritdoc/>
		public Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			CancellationToken cancellationToken = new CancellationTokenSource(MaxResponseTimeoutMs).Token;
			return GetAllTitles(cancellationToken);
		}

		/// <inheritdoc/>
		public Task<IEnumerable<BookstoreTitle>> GetAllTitles(CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(() =>
			{
				string requestName = "Title/GetAll/";
				Task<HttpResponseMessage> httpResponseTask = bokstoreServiceHttpClient.GetAsync(requestName, cancellationToken);

				httpResponseTask.Wait(cancellationToken);//Token is timed so it will not stuck forever

				HttpResponseMessage httpResponseMessage = httpResponseTask.Result;
				if (!httpResponseMessage.IsSuccessStatusCode)
				{
					return Enumerable.Empty<BookstoreTitle>();
				}

				Task<IEnumerable<BookstoreTitle>> conversionTask = httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<BookstoreTitle>>(cancellationToken);
				conversionTask.Wait(cancellationToken);

				return conversionTask.Result;

			}, cancellationToken)
			.ContinueWith(t =>
			{
				t.Exception?.Handle(ex =>
				{
					return true;
				});

				return Enumerable.Empty<BookstoreTitle>(); //Empty collection is good enough indicator that something failed.
			}, TaskContinuationOptions.OnlyOnFaulted);
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
			bokstoreServiceHttpClient.Dispose();
		}
	}
}
