namespace FirestoreSharp
{
    using System;
    using System.Linq.Expressions;
    using Google.Cloud.Firestore;

    public class TypedUpdate<T>
    {
        private readonly Transaction _transaction;
        private readonly TypedDocumentReference<T> _typedDocumentReference;

        public TypedUpdate(Transaction transaction, TypedDocumentReference<T> typedDocumentReference)
        {
            _transaction = transaction;
            _typedDocumentReference = typedDocumentReference;
        }

        public void With<TMember>(Expression<Func<T, TMember>> selector, TMember value)
        {
            _transaction.Update(_typedDocumentReference.DocumentReference, FieldPathResolver<T>.Resolve(selector),
                value);
        }
    }
}