using Moq;
using OrderProcessingBLL;
using OrderProcessingDAL;
using DTOs;

namespace OrderProcessingTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepoMock;
        private Mock<IGenericRepository<Product>> _productRepoMock;
        private Mock<IGenericRepository<Customer>> _customerRepoMock;
        private Mock<IGenericRepository<OrderItem>> _orderItemRepoMock;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _productRepoMock = new Mock<IGenericRepository<Product>>();
            _customerRepoMock = new Mock<IGenericRepository<Customer>>();
            _orderItemRepoMock = new Mock<IGenericRepository<OrderItem>>();
            _orderService = new OrderService(_orderRepoMock.Object, _productRepoMock.Object, _customerRepoMock.Object, _orderItemRepoMock.Object);
        }

        [Test]
        public void CreateOrder_WithInvalidCustomer_ThrowsException()
        {
            //Arrange
            var order = new Order { CustomerID = 2334 };
            var orderItems = new List<OrderItem>();

            //Act and Assert
            Assert.Throws<Exception>(() => _orderService.CreateOrder(order, orderItems), "Customer does not exist.");
        }

        [Test]
        public void UpdateOrder_WithNonExistentOrder_ThrowsException()
        {
            //Arrange
            var order = new Order { Id = 999 };
            var orderItems = new List<OrderItem>();


            //Act and Assert
            Assert.Throws<Exception>(() => _orderService.UpdateOrder(order, orderItems), "Order does not exist.");
        }

        [Test]
        public void DeleteOrder_WithNonExistentOrder_ThrowsException()
        {
            //Arrange
            int orderId = 999;

            //Act and Assert
            Assert.Throws<Exception>(() => _orderService.DeleteOrder(orderId), "Order does not exist.");
        }

        [Test]
        public void SearchOrders_WithEmptyCriteria_ThrowsException()
        {
            //Arrange
            string searchCriteria = "";

            //Act and Assert
            Assert.Throws<Exception>(() =>_orderService.SearchOrders(searchCriteria,""), "Search term cannot be empty.");
        }
    }
}