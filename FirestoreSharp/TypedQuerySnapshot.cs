using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public class TypedQuerySnapshot<T> : IReadOnlyList<TypedDocumentSnapshot<T>>, IEquatable<TypedQuerySnapshot<T>>
    {
        public TypedQuerySnapshot(QuerySnapshot querySnapshot)
        {
            QuerySnapshot = querySnapshot;
        }

        public QuerySnapshot QuerySnapshot { get; }

        public IEnumerable<TypedDocumentSnapshot<T>> Documents =>
            QuerySnapshot.Documents.Select(x => new TypedDocumentSnapshot<T>(x));

        public IEnumerator<TypedDocumentSnapshot<T>> GetEnumerator()
        {
            return Documents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => QuerySnapshot.Documents.Count;

        public bool Equals(TypedQuerySnapshot<T> other) => QuerySnapshot.Equals(other.QuerySnapshot);

        public override bool Equals(object obj) => Equals(obj as TypedQuerySnapshot<T>);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((QuerySnapshot != null ? QuerySnapshot.GetHashCode() : 0) * 397) ^ Count;
            }
        }

        public TypedDocumentSnapshot<T> this[int index] => new TypedDocumentSnapshot<T>(QuerySnapshot[index]);
    }
}