namespace FirestoreSharp.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;

    public class TypedCollectionWrapperLinqTests : FirestoreTypedCollectionTestBase<SampleData>
    {
        [Fact]
        public async Task Contains()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {IntArray1 = new[] {1, 2, 3}});
            await collection.AddAsync(new SampleData {IntArray1 = new[] {11, 22, 33}});
            await collection.AddAsync(new SampleData {IntArray1 = new[] {44, 55, 66, 1, 2, 3}});
            await collection.AddAsync(new SampleData {IntArray1 = new[] {32, 31, 30, 1, 2, 3}});

            var query = collection.Where(x => x.IntArray1.Contains(3));

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task Equal()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "boh", Int1 = 2});
            await collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "mah", Int1 = 3});
            await collection.AddAsync(new SampleData {String1 = "xxx1", String2WithCustomName = "111", Int1 = 1});
            await collection.AddAsync(new SampleData {String1 = "xxx2", String2WithCustomName = "222", Int1 = 1});

            var query = collection.Where(x => x.String1 == "asd");

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Equal_with_custom_path()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData
                {String1 = "asd", String2WithCustomName = "boh", Int2WithCustomName = 2});
            await collection.AddAsync(new SampleData
                {String1 = "asd", String2WithCustomName = "mah", Int2WithCustomName = 3});
            await collection.AddAsync(new SampleData
                {String1 = "xxx1", String2WithCustomName = "111", Int2WithCustomName = 1});
            await collection.AddAsync(new SampleData
                {String1 = "xxx2", String2WithCustomName = "222", Int2WithCustomName = 1});

            var query = collection.Where(x => x.String2WithCustomName == "111");

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task Equal_with_custom_path_and_MethodCall()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData
                {String1 = "asd", String2WithCustomName = "boh", Int2WithCustomName = 2});
            await collection.AddAsync(new SampleData
                {String1 = "asd", String2WithCustomName = "mah", Int2WithCustomName = 3});
            await collection.AddAsync(new SampleData
                {String1 = "xxx1", String2WithCustomName = "111", Int2WithCustomName = 1});
            await collection.AddAsync(new SampleData
                {String1 = "xxx2", String2WithCustomName = "222", Int2WithCustomName = 1});

            var query = collection.Where(x => x.String2WithCustomName.Equals("111"));

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GreaterThan_with_custom_path()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {Int2WithCustomName = 1});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 2});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 3});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 4});

            var query = collection.Where(x => x.Int2WithCustomName > 2);

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GreaterThanOrEqual_with_custom_path()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {Int2WithCustomName = 1});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 2});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 3});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 4});

            var query = collection.Where(x => x.Int2WithCustomName >= 2);

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task LessThan_with_custom_path()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {Int2WithCustomName = 1});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 2});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 3});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 4});

            var query = collection.Where(x => x.Int2WithCustomName < 2);

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task LessThanOrEqual_with_custom_path()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {Int2WithCustomName = 1});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 2});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 3});
            await collection.AddAsync(new SampleData {Int2WithCustomName = 4});

            var query = collection.Where(x => x.Int2WithCustomName <= 2);

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(2);
        }


        [Fact]
        public async Task Multiple_filters()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {String2WithCustomName = "antani", IntArray1 = new[] {1, 2, 3}});
            await collection.AddAsync(new SampleData {String2WithCustomName = "none", IntArray1 = new[] {11, 22, 33}});
            await collection.AddAsync(new SampleData
                {String2WithCustomName = "antani", IntArray1 = new[] {44, 55, 66, 1, 2, 3}});
            await collection.AddAsync(new SampleData
                {String2WithCustomName = "none", IntArray1 = new[] {32, 31, 30, 1, 2, 3}});

            var query = collection.Where(x => x.IntArray1.Contains(1) && x.String2WithCustomName.Equals("antani"));

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Multiple_filters_and_equality_operator()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {String2WithCustomName = "antani", IntArray1 = new[] {1, 2, 3}});
            await collection.AddAsync(new SampleData {String2WithCustomName = "none", IntArray1 = new[] {11, 22, 33}});
            await collection.AddAsync(new SampleData
                {String2WithCustomName = "antani", IntArray1 = new[] {44, 55, 66, 1, 2, 3}});
            await collection.AddAsync(new SampleData
                {String2WithCustomName = "none", IntArray1 = new[] {32, 31, 30, 1, 2, 3}});

            var query = collection.Where(x => x.IntArray1.Contains(1) && x.String2WithCustomName == "antani");

            var result = await query.GetSnapshotAsync();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Or_should_throw_NotSupportedException()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "boh", Int1 = 2});
            await collection.AddAsync(new SampleData {String1 = "asd", String2WithCustomName = "mah", Int1 = 3});
            await collection.AddAsync(new SampleData {String1 = "xxx1", String2WithCustomName = "111", Int1 = 1});
            await collection.AddAsync(new SampleData {String1 = "xxx2", String2WithCustomName = "222", Int1 = 1});

            Assert.Throws<NotSupportedException>(() =>
                collection.Where(x => x.String1 == "asd" || x.String2WithCustomName == "boh"));
        }

        [Fact]
        public async Task OrderBy()
        {
            var collection = CreateCollectionAtRandomPath();

            await collection.AddAsync(new SampleData {String2WithCustomName = "antani", Int1 = 1});
            await collection.AddAsync(new SampleData {String2WithCustomName = "no", Int1 = 2});
            await collection.AddAsync(new SampleData {String2WithCustomName = "antani", Int1 = 3});
            await collection.AddAsync(new SampleData {String2WithCustomName = "no", Int1 = 4});

            var query = collection.Where(x => x.String2WithCustomName == "antani")
                .OrderBy(x => x.Int1);

            var result = await query.GetSnapshotAsync();

            result.Should().HaveCount(2);
        }
    }
}