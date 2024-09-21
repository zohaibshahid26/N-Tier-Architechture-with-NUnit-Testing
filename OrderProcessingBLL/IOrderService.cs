using DTOs;
namespace OrderProcessingBLL
{
    public interface IOrderService
    {
        void CreateOrder(Order order, List<OrderItem> orderItems);
        IEnumerable<Order> GetAllOrders();
        Order ? GetOrderById(int id);
        void UpdateOrder(Order order, List<OrderItem> orderItems);
        void DeleteOrder(int id);
        IEnumerable<Order> SearchOrders(string search, string searchCriteria);
        IEnumerable<Customer> GetCustomers();
        IEnumerable<Product> GetProducts();
        Customer? GetCustomerById(int id);

    }
}