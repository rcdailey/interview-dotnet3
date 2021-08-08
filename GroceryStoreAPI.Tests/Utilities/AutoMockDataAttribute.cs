using AutoFixture.NUnit3;

namespace GroceryStoreAPI.Tests.Utilities
{
    public sealed class AutoMockDataAttribute : AutoDataAttribute
    {
        public AutoMockDataAttribute()
            : base(MockingFixture.Create)
        {
        }
    }
}
