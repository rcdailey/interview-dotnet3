#pragma warning disable CA1801

namespace GroceryStoreAPI.Data
{
    public record Customer(string Name, int Id = default);
}
