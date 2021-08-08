using System.IO.Abstractions;
using Autofac;
using GroceryStoreAPI.Data;

namespace GroceryStoreAPI
{
    public static class CompositionRoot
    {
        public static void Setup(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<CustomerJsonRepository>().As<ICustomerRepository>();

            builder.Register(c =>
                {
                    var context = new GroceryStoreJsonDataContext(c.Resolve<IFileSystem>());
                    context.LoadJson();
                    return context;
                })
                .As<IGroceryStoreJsonDataContext>()
                .InstancePerLifetimeScope();
        }
    }
}
