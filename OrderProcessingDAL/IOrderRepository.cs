using DTOs;
namespace OrderProcessingDAL
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        IEnumerable<OrderItem> GetOrderItems(int orderId);
    }
}
