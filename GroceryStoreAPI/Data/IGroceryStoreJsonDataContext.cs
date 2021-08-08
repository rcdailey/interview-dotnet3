using System.Collections.Generic;

namespace GroceryStoreAPI.Data
{
    public interface IGroceryStoreJsonDataContext
    {
        IList<Customer> Customers { get; }
        int NextAvailablePrimaryKey { get; set; }
        void LoadJson();
        void SaveJson();
    }
}
