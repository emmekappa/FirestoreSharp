namespace FirestoreSharp.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class FirestorePathResolverTests
    {
        [Fact]
        public void Resolve_should_throw_when_missing_FirestoreProperty()
        {
            Assert.Throws<InvalidOperationException>(() =>
                FieldPathResolver<SampleData>.Resolve(x => x.StringWithoutFirestoreProperty));
        }

        [Fact]
        public void Resolve_should_throw_when_nested_and_missing_FirestoreProperty()
        {
            Assert.Throws<InvalidOperationException>(() =>
                FieldPathResolver<SampleData>.Resolve(x => x.NestedData1.Int1).Should().Be("NestedData1.Int1"));
        }

        [Fact]
        public void Resolve_when_attribute()
        {
            FieldPathResolver<SampleData>.Resolve(x => x.Int2WithCustomName).Should().Be("__int_second");
        }

        [Fact]
        public void Resolve_when_nested_with_attrbiute()
        {
            FieldPathResolver<SampleData>.Resolve(x => x.NestedData2CustomName.Int2WithCustomName).Should()
                .Be("custom_nested_path.__int_second");
        }

        [Fact]
        public void Resolve_when_nested_with_mix_attributes()
        {
            FieldPathResolver<SampleData>.Resolve(x => x.NestedData1.Int2WithCustomName).Should()
                .Be("NestedData1.__int_second");
        }

        [Fact]
        public void Resolve_when_no_attribute()
        {
            FieldPathResolver<SampleData>.Resolve(x => x.Int1).Should().Be("Int1");
            FieldPathResolver<SampleData>.Resolve(x => x.String1).Should().Be("String1");
        }
    }
}