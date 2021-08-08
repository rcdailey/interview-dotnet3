using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GroceryStoreAPI.Data
{
    public class GroceryStoreJsonDataContext : IGroceryStoreJsonDataContext
    {
        private const string DatabaseFilename = "database.json";
        private readonly List<Customer> _customers = new();
        private readonly IFileSystem _fileSystem;
        private readonly JsonSerializerSettings _serializerSettings;

        public GroceryStoreJsonDataContext(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public int NextAvailablePrimaryKey { get; set; } = 1;

        public IList<Customer> Customers => _customers;

        public void LoadJson()
        {
            // If the file does not exist, create it.
            if (!_fileSystem.File.Exists(DatabaseFilename))
            {
                SaveJson();
            }
            else
            {
                var serializer = new JsonSerializer();
                using var stream = _fileSystem.File.OpenText(DatabaseFilename);
                using var reader = new JsonTextReader(stream);
                serializer.Populate(reader, this);
            }

            // Initial data does not include this property, so it is nullable.
            NextAvailablePrimaryKey = _customers
                .DefaultIfEmpty(new Customer(""))
                .Max(x => x.Id) + 1;
        }

        public void SaveJson()
        {
            var serializer = JsonSerializer.Create(_serializerSettings);
            using var stream = _fileSystem.File.CreateText(DatabaseFilename);
            using var writer = new JsonTextWriter(stream);
            serializer.Serialize(writer, this);
        }
    }
}
