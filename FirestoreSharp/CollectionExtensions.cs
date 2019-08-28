using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public static class TransactionExtensions
    {
        public static async Task<TypedDocumentSnapshot<T>> GetSnapshotAsync<T>(this Transaction transaction,
            DocumentReference documentReference,
            CancellationToken cancellationToken = default)
        {
            return new TypedDocumentSnapshot<T>(
                await transaction.GetSnapshotAsync(documentReference, cancellationToken));
        }

        public static async Task<TypedQuerySnapshot<T>> GetSnapshotAsync<T>(this Transaction transaction,
            Query query,
            CancellationToken cancellationToken = default)
        {
            return new TypedQuerySnapshot<T>(
                await transaction.GetSnapshotAsync(query, cancellationToken));
        }

        public static TypedUpdate<T> Update<T>(this Transaction transaction,
            TypedDocumentReference<T> typedDocumentReference)
        {
            return new TypedUpdate<T>(transaction, typedDocumentReference);
        }
    }
}