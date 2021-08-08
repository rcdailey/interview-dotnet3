# Completed Interview Assignment

This is my personalized solution to the [Autobooks interview repository][1]. To reiterate, the
following requirements were mentioned:

- API listing all customers
- API retrieving a customer
- API adding a customer
- API updating a customer
- Unit tests
- Use .NET Core 3.1 or NET 5+

I did consider adding a delete endpoint, but since it was not explicitly specified, I did not do it.

[1]: https://github.com/autobooks/interview-dotnet3

## Features and Changes

- Upgraded to .NET 5.0
- Use generic `Host` instead of `WebHost`
- An API Controller implemented to serve the requirements listed for this assignment.
- Extensive unit and integration testing implemented
- Provided Postman collection that may be used to test the APIs after running `dotnet watch run`.

## Explanation of Nuget Package Choices

For unit testing, I am using:

- NUnit (for the unit test framework)
- NSubstitute (for mocking)
- AutoFixture (for easier "Arrange" in arrange-act-assert test structure)
- FluentAssertions (for more fluid and readable LINQ-style test assertions)

For general implementation in the main project:

- Autofac (my personal favorite DI container for implementing inversion of control)
- Newtsonsoft.Json (for JSON serialization)
- System.IO.Abstractions (for abstracting away the filesystem to greatly improve unit testing, since
  I can simulate file data without managing files in my tests)

## Design

For the foundation of the assignment, I'm using the following principles:

- CRUD for the API (minus the "D", of course!)
- Loosely follow the Unit of Work and Repository patterns for the persistence layer.

I chose to use UoW & Repo patterns with the main goals of:

- Anticipating that later we may want to replace JSON persistence with an actual database, such as
  with EFCore.
- Making unit testing easier since I can easily separate main business logic from the persistence
  concerns using a repository interface.

Repository pattern here doesn't show much value beyond this, especially since the data context only
is concerned with one type of data: A list of customers. But long term, you'll be able to add more
data points (such as a product listing in the grocery store) and you won't need to go back and touch
existing code too much (I tried to keep things as SOLID as possible, namely open-closed in this
case).

## Manual Acceptance Testing

My test environment is Windows 10 x64 using Jetbrains Rider 2021.2. I only tested development mode
using `dotnet watch run`, I did not do any publishing to IIS, etc.

Acceptance testing performed:

- Run all unit tests in Rider and verify they pass.
- Run all 4 requests in the provided Postman collection
  (`GroceryStoreAPITests.postman_collection.json`) and verify the results in the HTTP response as
  well as changes in the `database.json` file.
- Completely delete the `database.json` file and verify it is created on the next HTTP request.

> **Note**: The `GroceryStoreJsonDataContext` uses relative paths to find and use `database.json`.
> In my tests, it was able to find this file without any additional changes to my project settings.
> If in your testing it is not able to find the file, please accept my apologies and try moving the
> `database.json` around to the appropriate location so that it may find the file.
