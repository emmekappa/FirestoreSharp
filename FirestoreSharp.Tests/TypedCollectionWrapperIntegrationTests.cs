namespace FirestoreSharp.Tests
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;
    using Xunit.Abstractions;

    public class TypedCollectionWrapperIntegrationTests : FirestoreTypedCollectionTestBase<SampleData>
    {
        public TypedCollectionWrapperIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private readonly ITestOutputHelper _testOutputHelper;

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task ListDocumentsAsync(int count)
        {
            var collection = CreateCollectionAtRandomPath();

            for (var i = 0; i < count; i++)
                await collection.AddAsync(new SampleData {Int2WithCustomName = i});

            var stopWatch = Stopwatch.StartNew();
            (await collection.ListDocumentsAsync().Count())
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
            var collection = CreateCollectionAtRandomPath();

            for (var i = 0; i < count; i++)
                await collection.AddAsync(new SampleData {Int2WithCustomName = i});

            var stopWatch = Stopwatch.StartNew();
            var snapshot = await collection.GetSnapshotAsync();
            snapshot.Should().HaveCount(count);
            stopWatch.Stop();
            _testOutputHelper.WriteLine($"GetSnapshotAsync executed in {stopWatch.ElapsedMilliseconds}ms");
        }

        [Fact]
        public async Task Delete()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.GetDocument("doc1").SetAsync(new SampleData {String1 = "HELLO!"});
            await collection.GetDocument("doc1").DeleteAsync();
            (await collection.GetDocument("doc1").GetSnapshotAsync()).Exists.Should().BeFalse();
        }

        [Fact]
        public async Task Set()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.GetDocument("doc1").SetAsync(new SampleData {String1 = "HELLO!"});

            (await collection.GetDocument("doc1").GetSnapshotAsync()).Convert()
                .String1.Should().Be("HELLO!");
        }


        [Fact]
        public async Task Transaction_with_DocumentReference()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.GetDocument("doc1").SetAsync(new SampleData
                {String1 = "asd", String2WithCustomName = "boh", Int1 = 2});

            await collection.GetDocument("doc2").SetAsync(new SampleData
                {String1 = "xxx", String2WithCustomName = "111", Int1 = 1});

            await Db.RunTransactionAsync(async transaction =>
            {
                var currentSnapshot =
                    await transaction.GetSnapshotAsync<SampleData>(collection.GetDocument("doc1").DocumentReference);


                transaction.Update(currentSnapshot.Reference).With(x => x.String2WithCustomName, "updatedValue");

                transaction.Update(currentSnapshot.Reference)
                    .With(x => x.Int1, currentSnapshot.GetValue(x => x.Int1) + 1);
            });

            var doc1 = (await collection.GetDocument("doc1").GetSnapshotAsync()).Convert();
            doc1.Int1.Should().Be(3);
            doc1.String2WithCustomName.Should().Be("updatedValue");
        }


        [Fact(Skip = "transactional queries not implemented (on emulator?)")]
        public async Task Transaction_with_Query()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "boh", Int1 = 2});
            await collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "mah", Int1 = 3});
            await collection.AddAsync(new SampleData {String1 = "xxx", String2WithCustomName = "111", Int1 = 1});
            await collection.AddAsync(new SampleData {String1 = "xxx", String2WithCustomName = "222", Int1 = 1});

            await Db.RunTransactionAsync(async transaction =>
            {
                var currentSnapshot =
                    await transaction.GetSnapshotAsync<SampleData>(collection.WhereEqualTo(x => x.String1, "asd"));


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
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {IntArray1 = new[] {33, 11, 22}});
            await collection.AddAsync(new SampleData {IntArray1 = new[] {10, 10, 20}});
            var query = collection.WhereArrayContains(x => x.IntArray1, 10);

            var snapshot = await query.GetSnapshotAsync();
            Assert.Equal(1, snapshot.Documents.Count);
        }

        [Fact]
        public async Task WhereEqualTo_should_filter()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {Int2WithCustomName = 33});
            var query = collection.WhereEqualTo(x => x.Int2WithCustomName, 33);

            var snapshot = await query.GetSnapshotAsync();
            Assert.Equal(1, snapshot.Documents.Count);
        }

        [Fact]
        public async Task WhereEqualTo_should_filter_when_FirebaseProperty_is_not_defined()
        {
            var collection = CreateCollectionAtRandomPath();

            foreach (var documentSnapshot in (await collection.GetSnapshotAsync()).Documents)
                await documentSnapshot.Reference.DeleteAsync();

            await collection.AddAsync(new SampleData {Int1 = 33});
            var query = collection.WhereEqualTo(x => x.Int1, 33);

            var snapshot = await query.GetSnapshotAsync();
            Assert.Equal(1, snapshot.Documents.Count);
        }
    }
}