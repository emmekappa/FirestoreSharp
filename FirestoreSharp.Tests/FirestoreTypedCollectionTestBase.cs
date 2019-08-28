using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Primitives;
using Google.Cloud.Firestore;

namespace FirestoreSharp.Tests
{
    public class FirestoreTypedCollectionTestBase<T> where T : new()
    {
        protected readonly FirestoreDb Db;
        private FirestoreSharp.TypedCollectionWrapper<T> _collection;
        protected FirestoreSharp.TypedCollectionWrapper<T> Collection => _collection ?? (_collection = CreateCollection());

        public FirestoreTypedCollectionTestBase()
        {
            Db = FirestoreDbFactory.Create();
        }

        protected FirestoreSharp.TypedCollectionWrapper<T> CreateCollection(string path = null)
        {
            return DatabaseExtensions.Collection<T>(Db, path);
        }
        
        protected FirestoreSharp.TypedCollectionWrapper<T> CreateCollectionAtRandomPath()
        {
            return DatabaseExtensions.Collection<T>(Db, Guid.NewGuid().ToString());
        }

        protected async Task<ObjectAssertions> ShouldPersistedMember<TMember>(T entity,
            Expression<Func<T, TMember>> selector,
            TMember value)
        {
            var collection = DatabaseExtensions.Collection<T>(Db, Guid.NewGuid().ToString());
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

        protected async Task ClearCollection()
        {
            var collection = CreateCollection();
            foreach (var documentSnapshot in (await collection.GetSnapshotAsync()).Documents)
                await documentSnapshot.Reference.DeleteAsync();
        }
    }
}