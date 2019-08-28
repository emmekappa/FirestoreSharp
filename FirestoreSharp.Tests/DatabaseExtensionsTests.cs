using FluentAssertions;
using Xunit;

namespace FirestoreSharp.Tests
{
    public class DatabaseExtensionsTests  
    {
        [Fact]
        public void Should_respect_path()
        {
            FirestoreDbFactory.Create().Collection<SampleData>("expectedPath")
                .CollectionReference.Path.Should().EndWith("expectedPath");
        }
        
        [Fact]
        public void Should_have_a_default_path()
        {
            FirestoreDbFactory.Create().Collection<SampleData>()
                .CollectionReference.Path.Should().EndWith(nameof(SampleData));
        }
    }
}