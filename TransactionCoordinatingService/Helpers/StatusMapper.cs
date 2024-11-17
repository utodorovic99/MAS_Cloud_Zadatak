using BookstoreServiceContract.Enums;
using Common.Enums;
using System.Collections.Generic;
using UsersServiceContract.Enums;

namespace TransactionCoordinatingService.Helpers
{
	/// <summary>
	/// Mapper for different status enumerations.
	/// </summary>
	internal static class StatusMapper
	{
		private static readonly Dictionary<EnlistMoneyTransferStatus, PurchaseResponseStatus> enlistMoneyTransferStatusToPurchaseResponse;
		private static readonly Dictionary<BookstoreEnlistPurchaseStatus, PurchaseResponseStatus> enlistPurchaseStatusToPurchaseResponse;

		/// <summary>
		/// Static initialization of <see cref="StatusMapper"/>.
		/// </summary>
		static StatusMapper()
		{
			enlistMoneyTransferStatusToPurchaseResponse = new Dictionary<EnlistMoneyTransferStatus, PurchaseResponseStatus>(3);
			enlistMoneyTransferStatusToPurchaseResponse[EnlistMoneyTransferStatus.Fail] = PurchaseResponseStatus.Fail;
			enlistMoneyTransferStatusToPurchaseResponse[EnlistMoneyTransferStatus.Success] = PurchaseResponseStatus.Success;
			enlistMoneyTransferStatusToPurchaseResponse[EnlistMoneyTransferStatus.InsuffientFunds] = PurchaseResponseStatus.InsufficientFunds;

			enlistPurchaseStatusToPurchaseResponse = new Dictionary<BookstoreEnlistPurchaseStatus, PurchaseResponseStatus>(3);
			enlistPurchaseStatusToPurchaseResponse[BookstoreEnlistPurchaseStatus.Fail] = PurchaseResponseStatus.Fail;
			enlistPurchaseStatusToPurchaseResponse[BookstoreEnlistPurchaseStatus.Success] = PurchaseResponseStatus.Success;
			enlistPurchaseStatusToPurchaseResponse[BookstoreEnlistPurchaseStatus.OutOfCopies] = PurchaseResponseStatus.OutOfCopies;
		}

		/// <summary>
		/// Gets <see cref="PurchaseResponseStatus"/> from <paramref name="enlistMoneyTransferStatus"/>.
		/// </summary>
		/// <param name="enlistMoneyTransferStatus">Input status.</param>
		/// <returns>Resulting status marching with input one.</returns>
		public static PurchaseResponseStatus GetPurchaseResponseStatus(EnlistMoneyTransferStatus enlistMoneyTransferStatus)
		{
			return enlistMoneyTransferStatusToPurchaseResponse[enlistMoneyTransferStatus];
		}

		/// <summary>
		/// Gets <see cref="PurchaseResponseStatus"/> from <paramref name="enlistPurchaseStatus"/>.
		/// </summary>
		/// <param name="enlistPurchaseStatus">Input status.</param>
		/// <returns>Resulting status marching with input one.</returns>
		public static PurchaseResponseStatus GetPurchaseResponseStatus(BookstoreEnlistPurchaseStatus enlistPurchaseStatus)
		{
			return enlistPurchaseStatusToPurchaseResponse[enlistPurchaseStatus];
		}
	}
}
