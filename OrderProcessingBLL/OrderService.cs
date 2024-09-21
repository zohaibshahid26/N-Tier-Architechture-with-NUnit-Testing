using DTOs;
using OrderProcessingDAL;
namespace OrderProcessingBLL
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;

        public OrderService(IOrderRepository orderRepository, IGenericRepository<Product> productRepository, IGenericRepository<Customer> customerRepository, IGenericRepository<OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _orderItemRepository = orderItemRepository;
        }

        public void CreateOrder(Order orderDto, List<OrderItem> orderItemDtos)
        {
            var customer = _customerRepository.GetById(orderDto.CustomerID);
            if (customer == null)
            {
                throw new Exception("Customer does not exist.");
            }

            foreach (var itemDto in orderItemDtos)
            {
                var product = _productRepository.GetById(itemDto.ProductID);
                if (product == null || product.Quantity < itemDto.Quantity)
                {
                    throw new Exception("Product does not exist or insufficient stock.");
                }
                itemDto.Price = product.Price;
                product.Quantity -= itemDto.Quantity;
                _productRepository.Update(product);
            }


            orderDto.TotalAmount = orderItemDtos.Sum(item => item.Quantity * item.Price);
            _orderRepository.Add(orderDto);

            foreach (var itemDto in orderItemDtos)
            {
                itemDto.OrderID = orderDto.Id;
                _orderItemRepository.Add(itemDto);
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            var orders = _orderRepository.GetAll();
            if (!orders.Any())
            {
                throw new Exception("No orders found.");
            }
            return orders;
        }

        public Order? GetOrderById(int id)
        {
            return _orderRepository.GetById(id);
        }

        public void UpdateOrder(Order orderDto, List<OrderItem> orderItemDtos)
        {
            var order = _orderRepository.GetById(orderDto.Id);
            if (order == null)
            {
                throw new Exception("Order does not exist.");
            }
           foreach (var itemDto in orderItemDtos)
           {
                var product = _productRepository.GetById(itemDto.ProductID);
                if (product == null || product.Quantity < itemDto.Quantity)
                {
                    throw new Exception("Product does not exist or insufficient stock.");
                }
                itemDto.Price = product.Price;
                product.Quantity -= itemDto.Quantity;
                _productRepository.Update(product);
           }
            orderDto.TotalAmount = orderItemDtos.Sum(item => item.Quantity * item.Price);
            _orderRepository.Update(orderDto);

            var existingOrderItems = _orderRepository.GetOrderItems(orderDto.Id);
            foreach (var item in existingOrderItems)
            {
                _orderItemRepository.Delete(item.Id);
            }

            foreach (var itemDto in orderItemDtos)
            {
                itemDto.OrderID = orderDto.Id;
                _orderItemRepository.Add(itemDto);
            }
        }

        public void DeleteOrder(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                throw new Exception("Order does not exist.");
            }
            _orderRepository.Delete(order.Id);
            
            var orderItems = _orderRepository.GetOrderItems(id);
            foreach (var item in orderItems)
            {
                var product = _productRepository.GetById(item.ProductID);
                product!.Quantity += item.Quantity;
                _productRepository.Update(product);
            }  
        }

        public IEnumerable<Order> SearchOrders(string search, string searchCriteria)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new Exception("Search term cannot be empty.");
            }
            var allOrders = _orderRepository.GetAll();
            
            switch(searchCriteria)
            {
                case "CustomerName":
                    return allOrders.Where(order => _customerRepository.GetById(order.CustomerID)!.Name.Contains(search));
                case "OrderID":
                    return allOrders.Where(order => order.Id.ToString().Contains(search));
                case "DateRange":
                    var dates = search.Split(',');
                    return allOrders.Where(order => order.OrderDate >= DateTime.Parse(dates[0]) && order.OrderDate <= DateTime.Parse(dates[1]));
                default:
                    throw new Exception("Invalid search criteria.");
            }
        }


        public IEnumerable<Customer> GetCustomers()
        {
            var customers = _customerRepository.GetAll();
            if (!customers.Any())
            {
                throw new Exception("No customers found.");
            }
            return customers;
        }

        public IEnumerable<Product> GetProducts()
        {
            var products = _productRepository.GetAll();
            if (!products.Any())
            {
                throw new Exception("No products found.");
            }
            return products;

        }

        public Customer? GetCustomerById(int id)
        {
            return _customerRepository.GetById(id);
        }
    }
}