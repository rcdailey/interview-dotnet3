using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace GroceryStoreAPI.Tests.Utilities
{
    public static class MockingFixture
    {
        public static IFixture Create() => new Fixture().Customize(new AutoNSubstituteCustomization());
    }
}
