using System;
using System.IO;
using System.IO.Abstractions;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Json;
using GroceryStoreAPI.Data;
using GroceryStoreAPI.Tests.Utilities;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace GroceryStoreAPI.Tests.Data
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class GroceryStoreJsonDataContextTest
    {
        [Test, AutoMockData]
        public void Customers_are_loaded_and_names_are_parsed(
            [Frozen] IFileSystem fileSystem,
            [NoAutoProperties] GroceryStoreJsonDataContext context)
        {
            using var jsonStream = StreamUtils.MakeStreamReader(@"{
  'customers': [{
    'id': 1,
    'name': 'John'
  }, {
    'id': 2,
    'name': 'Jane'
  }]
}");
            fileSystem.File.OpenText(Arg.Any<string>()).Returns(jsonStream);
            fileSystem.File.Exists(Arg.Any<string>()).Returns(true);
            context.LoadJson();

            context.Customers.Should().BeEquivalentTo(
                new Customer("John", 1),
                new Customer("Jane", 2));
        }

        [Test, AutoMockData]
        public void Load_json_initializes_primary_key_counter(
            [Frozen] IFileSystem fileSystem,
            GroceryStoreJsonDataContext context)
        {
            // Newtonsoft.Json happens to parse single quotes in JSON data, even though those aren't normally valid,
            // which allows us to use them to make clean looking JSON strings in the unit test code.
            using var jsonStream = StreamUtils.MakeStreamReader(@"{
  'customers': [{
    'id': 4
  }, {
    'id': 8
  }]
}");
            fileSystem.File.OpenText(Arg.Any<string>()).Returns(jsonStream);
            fileSystem.File.Exists(Arg.Any<string>()).Returns(true);
            context.LoadJson();

            fileSystem.File.Received().OpenText(Arg.Is<string>(s => s.EndsWith("database.json")));
            context.NextAvailablePrimaryKey.Should().Be(9);
        }

        [Test, AutoMockData]
        public void Load_json_saves_if_file_doesnt_exist(
            [Frozen] IFileSystem fileSystem,
            GroceryStoreJsonDataContext context)
        {
            fileSystem.File.Exists(Arg.Any<string>()).Returns(false);
            fileSystem.File.CreateText(Arg.Any<string>()).Returns(_ => StreamUtils.MakeStreamWriter());
            context.LoadJson();

            fileSystem.File.Received().Exists(Arg.Is<string>(s => s.EndsWith("database.json")));
            fileSystem.File.Received().CreateText(Arg.Any<string>());
            fileSystem.File.DidNotReceive().OpenText(Arg.Any<string>());
        }

        [Test, AutoMockData]
        public void Primary_key_counter_is_initialized_to_default_without_load(
            [NoAutoProperties] GroceryStoreJsonDataContext context)
        {
            context.NextAvailablePrimaryKey.Should().Be(1);
        }

        [Test, AutoMockData]
        public void Save_json_works(
            [Frozen] IFileSystem fileSystem,
            [NoAutoProperties] GroceryStoreJsonDataContext context)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
            fileSystem.File.CreateText(Arg.Any<string>()).Returns(streamWriter);

            context.SaveJson();

            fileSystem.File.Received().CreateText(Arg.Is<string>(s => s.EndsWith("database.json")));

            var resultObject = JObject.Parse(StreamUtils.ReadStream(memoryStream));
            var expectedObject = JObject.FromObject(new
            {
                customers = Array.Empty<object>(),
                next_available_primary_key = 1
            });

            resultObject.Should().BeEquivalentTo(expectedObject);
        }
    }
}
