namespace FirestoreSharp.Tests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FluentAssertions.Primitives;
    using Google.Cloud.Firestore;

    public class FirestoreTypedCollectionTestBase<T> where T : new()
    {
        protected readonly FirestoreDb Db;

        public FirestoreTypedCollectionTestBase()
        {
            Db = FirestoreDbFactory.Create();
        }


        protected TypedCollectionWrapper<T> CreateCollection(string path = null)
        {
            return Db.Collection<T>(path);
        }

        protected TypedCollectionWrapper<T> CreateCollectionAtRandomPath()
        {
            return Db.Collection<T>(Guid.NewGuid().ToString());
        }

        protected async Task<ObjectAssertions> ShouldPersistedMember<TMember>(T entity,
            Expression<Func<T, TMember>> selector,
            TMember value)
        {
            var collection = CreateCollectionAtRandomPath();
            selector.SetValue(entity, value);

            await collection.AddAsync(entity);
            var snapshot = await collection.GetSnapshotAsync();


            var results = snapshot.QuerySnapshot.Documents.Select(x => x.ConvertTo<T>()).ToList();
            results.Should().HaveCount(1);

            return selector.GetValue(results.Single()).Should();
        }

        protected async Task ShouldPersistsMember<TMember>(T entity, Expression<Func<T, TMember>> selector,
            TMember value,
            string because = "", object[] becauseArgs = null)
        {
            (await ShouldPersistedMember(entity, selector, value)).Be(value, because, becauseArgs);
        }

        protected async Task ShouldPersistsMember<TMember>(Expression<Func<T, TMember>> selector, TMember value,
            string because = "", object[] becauseArgs = null)
        {
            await ShouldPersistsMember(new T(), selector, value);
        }

        protected static async Task ClearCollection(CollectionReference collectionReference)
        {
            foreach (var documentSnapshot in (await collectionReference.GetSnapshotAsync()).Documents)
                await documentSnapshot.Reference.DeleteAsync();
        }
    }
}