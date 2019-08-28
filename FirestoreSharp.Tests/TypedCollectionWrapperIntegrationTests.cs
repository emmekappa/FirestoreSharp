using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace FirestoreSharp.Tests
{
    public class TypedCollectionWrapperIntegrationTests : FirestoreTypedCollectionTestBase<SampleData>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TypedCollectionWrapperIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public async Task Delete()
        {
            await ClearCollection();

            await Collection.GetDocument("doc1").SetAsync(new SampleData {String1 = "HELLO!"});
            await Collection.GetDocument("doc1").DeleteAsync();
            (await Collection.GetDocument("doc1").GetSnapshotAsync()).Exists.Should().BeFalse();
        }

        [Fact]
        public async Task Set()
        {
            await ClearCollection();

            await Collection.GetDocument("doc1").SetAsync(new SampleData {String1 = "HELLO!"});

            (await Collection.GetDocument("doc1").GetSnapshotAsync()).Convert()
                .String1.Should().Be("HELLO!");
        }


        [Fact]
        public async Task Transaction_with_DocumentReference()
        {
            await ClearCollection();

            await Collection.GetDocument("doc1").SetAsync(new SampleData
                {String1 = "asd", String2WithCustomName = "boh", Int1 = 2});

            await Collection.GetDocument("doc2").SetAsync(new SampleData
                {String1 = "xxx", String2WithCustomName = "111", Int1 = 1});

            await Db.RunTransactionAsync(async transaction =>
            {
                var currentSnapshot =
                    await transaction.GetSnapshotAsync<SampleData>(Collection.GetDocument("doc1").DocumentReference);


                transaction.Update(currentSnapshot.Reference).With(x => x.String2WithCustomName, "updatedValue");

                transaction.Update(currentSnapshot.Reference)
                    .With(x => x.Int1, currentSnapshot.GetValue(x => x.Int1) + 1);
            });

            var doc1 = (await Collection.GetDocument("doc1").GetSnapshotAsync()).Convert();
            doc1.Int1.Should().Be(3);
            doc1.String2WithCustomName.Should().Be("updatedValue");
        }


        [Fact(Skip = "transactional queries not implemented (on emulator?)")]
        public async Task Transaction_with_Query()
        {
            await ClearCollection();

            await Collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "boh", Int1 = 2});
            await Collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "mah", Int1 = 3});
            await Collection.AddAsync(new SampleData {String1 = "xxx", String2WithCustomName = "111", Int1 = 1});
            await Collection.AddAsync(new SampleData {String1 = "xxx", String2WithCustomName = "222", Int1 = 1});

            await Db.RunTransactionAsync(async transaction =>
            {
                var currentSnapshot =
                    await transaction.GetSnapshotAsync<SampleData>(Collection.WhereEqualTo(x => x.String1, "asd"));


                foreach (var documentSnapshot in currentSnapshot.Documents)
                {
                    transaction.Update(documentSnapshot.Reference).With(x => x.String2WithCustomName, "updatedValue");

                    transaction.Update(documentSnapshot.Reference)
                        .With(x => x.Int1, documentSnapshot.GetValue(x => x.Int1) + 1);
                }
            });
        }

        [Fact]
        public async Task WhereArrayContains_should_filter()
        {
            await ClearCollection();

            await Collection.AddAsync(new SampleData {IntArray1 = new[] {33, 11, 22}});
            await Collection.AddAsync(new SampleData {IntArray1 = new[] {10, 10, 20}});
            var query = Collection.WhereArrayContains(x => x.IntArray1, 10);

            var snapshot = await query.GetSnapshotAsync();
            Assert.Equal(1, snapshot.Documents.Count);
        }

        [Fact]
        public async Task WhereEqualTo_should_filter()
        {
            await ClearCollection();

            await Collection.AddAsync(new SampleData {Int2WithCustomName = 33});
            var query = Collection.WhereEqualTo(x => x.Int2WithCustomName, 33);

            var snapshot = await query.GetSnapshotAsync();
            Assert.Equal(1, snapshot.Documents.Count);
        }

        [Fact]
        public async Task WhereEqualTo_should_filter_when_FirebaseProperty_is_not_defined()
        {
            foreach (var documentSnapshot in (await Collection.GetSnapshotAsync()).Documents)
                await documentSnapshot.Reference.DeleteAsync();

            await Collection.AddAsync(new SampleData {Int1 = 33});
            var query = Collection.WhereEqualTo(x => x.Int1, 33);

            var snapshot = await query.GetSnapshotAsync();
            Assert.Equal(1, snapshot.Documents.Count);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task ListDocumentsAsync(int count)
        {
            await ClearCollection();

            for (var i = 0; i < count; i++)
                await Collection.AddAsync(new SampleData {Int2WithCustomName = i});

            var stopWatch = Stopwatch.StartNew();
            (await Collection.ListDocumentsAsync().Count())
                .Should().Be(count);
            stopWatch.Stop();
            _testOutputHelper.WriteLine($"ListDocumentsAsync executed in {stopWatch.ElapsedMilliseconds}ms");
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetSnapshotAsync(int count)
        {
            await ClearCollection();

            for (var i = 0; i < count; i++)
                await Collection.AddAsync(new SampleData {Int2WithCustomName = i});
            
            var stopWatch = Stopwatch.StartNew();
            var snapshot = await Collection.GetSnapshotAsync();
            snapshot.Should().HaveCount(count);
            stopWatch.Stop();
            _testOutputHelper.WriteLine($"GetSnapshotAsync executed in {stopWatch.ElapsedMilliseconds}ms");
        }
    }
}