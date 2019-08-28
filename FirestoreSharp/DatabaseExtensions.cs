using Google.Cloud.Firestore;

namespace FirestoreSharp
{
    public static class DatabaseExtensions
    {
        public static TypedCollectionWrapper<T> Collection<T>(this FirestoreDb db, string path = null)
        {
            if (!string.IsNullOrEmpty(path))
                return new TypedCollectionWrapper<T>(db.Collection(path));
            return new TypedCollectionWrapper<T>(db.Collection(typeof(T).Name));
        }
    }
}