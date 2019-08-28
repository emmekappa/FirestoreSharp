namespace FirestoreSharp
{
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Cloud.Firestore;

    public class TypedDocumentReference<TEntity>
    {
        public TypedDocumentReference(DocumentReference documentReference)
        {
            DocumentReference = documentReference;
        }

        public DocumentReference DocumentReference { get; }

        public async Task<TypedDocumentSnapshot<TEntity>> GetSnapshotAsync()
        {
            return new TypedDocumentSnapshot<TEntity>(await DocumentReference.GetSnapshotAsync());
        }

        public async Task DeleteAsync()
        {
            await DocumentReference.DeleteAsync();
        }

        public async Task SetAsync(TEntity entity, SetOptions options = null,
            CancellationToken cancellationToken = default)
        {
            await DocumentReference.SetAsync(entity, options, cancellationToken);
        }
    }
}