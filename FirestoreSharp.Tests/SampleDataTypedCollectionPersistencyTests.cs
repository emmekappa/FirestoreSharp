namespace FirestoreSharp.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class SampleDataTypedCollectionPersistencyTests : FirestoreTypedCollectionTestBase<SampleData>
    {
        public SampleData createSampleData()
        {
            return new SampleData
            {
                NestedData1 = new NestedData
                {
                    NestedNestedData1 = new NestedNestedData(),
                    RecursiveNestedData1 = new NestedData()
                }
            };
        }

        [Theory]
        [ClassData(typeof(StringFieldData))]
        public async Task ShouldPersists_string_field(string value)
        {
            await ShouldPersistsMember(x => x.String1, value);
            await ShouldPersistsMember(x => x.String2WithCustomName, value);
        }

        [Theory]
        [ClassData(typeof(StringFieldData))]
        public async Task ShouldPersists_nested_string_field(string value)
        {
            await ShouldPersistsMember(createSampleData(), x => x.NestedData1.NestedNestedData1.String1, value);
            await ShouldPersistsMember(createSampleData(), x => x.NestedData1.RecursiveNestedData1.String1, value);
        }


        [Theory]
        [InlineData(1.0)]
        [InlineData(1.22312132)]
        [InlineData(313123312.3132)]
        [InlineData((double) 10121)]
        public async Task ShouldPersists_double_field(double value)
        {
            await ShouldPersistsMember(x => x.Double1, value);
        }

        [Theory]
        [InlineData((float) 1.0)]
        [InlineData((float) 1.22312132)]
        [InlineData((float) 313123312.3132)]
        public async Task ShouldPersists_float_field(float value)
        {
            await ShouldPersistsMember(x => x.Float1, value);
        }

        [Fact]
        public async Task Should_skip_string_field_without_FirestoreProperty()
        {
            (await ShouldPersistedMember(createSampleData(),
                    x => x.StringWithoutFirestoreProperty, "something"))
                .Be(default(string));
        }

        [Fact]
        public async Task ShouldPersists_DateTimeOffset_field()
        {
            await ShouldPersistsMember(x => x.DateTimeOffset1, new DateTimeOffset());
            await ShouldPersistsMember(x => x.DateTimeOffset1, new DateTimeOffset());
        }
    }

    internal class StringFieldData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {"sample"};
            yield return new object[] {"             with             spaces\t\t \n\n\n newlines  . "};
            yield return new object[] {" @ # ^ è à è ò ì ≠ ` `"};
            yield return new object[] {" ❤ 🍑 🍆 "};
            yield return new object[] {"{\"json\": \"HELLO WORLD!\"}"};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}