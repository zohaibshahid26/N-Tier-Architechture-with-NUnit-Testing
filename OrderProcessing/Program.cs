using DTOs;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessing;
using OrderProcessingBLL;
using OrderProcessingDAL;

namespace PL
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var app = serviceProvider.GetService<Application>();
            app?.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=OrderProcessingSystem;Integrated Security=True;";
            services.AddTransient<IOrderRepository>(provider => new OrderRepository(connectionString));
            services.AddTransient<IGenericRepository<Product>>(provider => new GenericRepository<Product>(connectionString));
            services.AddTransient<IGenericRepository<OrderItem>>(provider => new GenericRepository<OrderItem>(connectionString));
            services.AddTransient<IGenericRepository<Customer>>(provider => new GenericRepository<Customer>(connectionString));
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<Application>();
        }
    }
}
