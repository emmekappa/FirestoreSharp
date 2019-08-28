using System;
using System.Linq.Expressions;
using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public class TypedDocumentSnapshot<T>
    {
        public TypedDocumentSnapshot(DocumentSnapshot documentSnapshot)
        {
            DocumentSnapshot = documentSnapshot;
        }

        public DocumentSnapshot DocumentSnapshot { get; }

        public bool Exists => DocumentSnapshot.Exists;
        public TypedDocumentReference<T> Reference => new TypedDocumentReference<T>(DocumentSnapshot.Reference);

        public T Convert()
        {
            return DocumentSnapshot.ConvertTo<T>();
        }

        public TMember GetValue<TMember>(Expression<Func<T, TMember>> selector)
        {
            return DocumentSnapshot.GetValue<TMember>(FieldPathResolver<T>.Resolve(selector));
        }
    }
}