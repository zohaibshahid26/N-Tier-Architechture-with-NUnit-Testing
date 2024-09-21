using DTOs;
using Dapper;
using System.Data.SqlClient;

namespace OrderProcessingDAL
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(string connectionString) : base(connectionString)
        {

        }

        public override void Add(Order order)
        {

            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                // Insert the Order
                dbConnection.Execute(
                    "INSERT INTO Orders (CustomerID, OrderDate, TotalAmount) VALUES (@CustomerID, @OrderDate, @TotalAmount)",
                    new { order.CustomerID, order.OrderDate, order.TotalAmount });

                // set the last inserted row id in order.Id
                order.Id = dbConnection.QuerySingle<int>("Select IDENT_CURRENT('Orders')");
            }



        }
        public IEnumerable<OrderItem> GetOrderItems(int orderId)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                return dbConnection.Query<OrderItem>("SELECT * FROM OrderItems WHERE OrderID = @OrderId", new { OrderId = orderId });
            }
        }

    }

    
}
                
           
