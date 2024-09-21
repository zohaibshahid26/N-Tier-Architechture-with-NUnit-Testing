using DTOs;
using OrderProcessingBLL;

namespace OrderProcessing
{
    public class Application
    {
        private readonly IOrderService _orderService;
        public Application(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("Order Processing System");
                Console.WriteLine("====================================");
                Console.WriteLine("1. List Orders");
                Console.WriteLine("2. Add Order");
                Console.WriteLine("3. Update Order");
                Console.WriteLine("4. Delete Order");
                Console.WriteLine("5. Search Orders");
                Console.WriteLine("0. Exit");
                Console.WriteLine("====================================");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        ListOrders();
                        break;
                    case "2":
                        Console.Clear();
                        AddOrder();
                        break;
                    case "3":
                        Console.Clear();
                        UpdateOrder();
                        break;
                    case "4":
                        Console.Clear();
                        DeleteOrder();
                        break;
                    case "5":
                        Console.Clear();
                        SearchOrders();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void ListOrders()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("All Orders:\n");
                var orders = _orderService.GetAllOrders();

                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.Id}");
                    Console.WriteLine($"Customer ID: {order.CustomerID}");
                    Console.WriteLine($"Customer Name: {_orderService.GetCustomerById(order.CustomerID)?.Name}");
                    Console.WriteLine($"Customer Email: {_orderService.GetCustomerById(order.CustomerID)?.Email}");
                    Console.WriteLine($"Order Date: {order.OrderDate}");
                    Console.WriteLine($"Total Amount: {order.TotalAmount}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateOrder()
        {
            try
            {
                Console.Clear();
                ListOrders();
                var orderId = ReadInt("Enter Order ID:");
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }

                var customers = _orderService.GetCustomers();
                Console.WriteLine("Customers:\n");
                foreach (var customer in customers)
                {
                    Console.WriteLine($"Customer ID: {customer.Id}, Name: {customer.Name}");
                }

                var customerId = ReadInt("Enter Customer ID:");
                var orderItems = new List<OrderItem>();
                while (true)
                {
                    var products = _orderService.GetProducts();
                    Console.WriteLine("Products:\n");
                    foreach (var product in products)
                    {
                        Console.WriteLine($"Product ID: {product.Id}, Name: {product.Name}, Quantity: {product.Quantity}, Price: {product.Price}");
                    }

                    var productId = ReadInt("Enter Product ID:");
                    var quantity = ReadInt("Enter Quantity:");
                    var price = ReadDecimal("Enter Price:");

                    orderItems.Add(new OrderItem
                    {
                        ProductID = productId,
                        Quantity = quantity,
                        Price = price
                    });

                    Console.WriteLine("Add another product? (Y/N)");
                    var choice = Console.ReadLine();
                    if (choice != "Y")
                    {
                        break;
                    }
                }

                _orderService.UpdateOrder(order, orderItems);
                Console.WriteLine("Order updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DeleteOrder()
        {
            try
            {
                Console.Clear();
                ListOrders();
                var orderId = ReadInt("Enter Order ID:");
                _orderService.DeleteOrder(orderId);
                Console.WriteLine("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SearchOrders()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("Select search criteria:");
                Console.WriteLine("1. Order ID");
                Console.WriteLine("2. Customer Name");
                Console.WriteLine("3. Date Range");
                string? searchCriteria = Console.ReadLine();
                while (searchCriteria != "1" && searchCriteria != "2" && searchCriteria != "3")
                {
                    Console.WriteLine("Invalid choice. Please enter a valid choice.");
                    searchCriteria = Console.ReadLine();
                }

                switch (searchCriteria)
                {
                    case "1":
                        var orderId = ReadInt("Enter Order ID:");
                        var order = _orderService.SearchOrders(orderId.ToString(), "OrderID").FirstOrDefault();
                        if (order == null)
                        {
                            Console.WriteLine("Order not found.");
                            return;
                        }
                        Console.WriteLine($"Order ID: {order.Id}");
                        Console.WriteLine($"Customer ID: {order.CustomerID}");
                        Console.WriteLine($"Customer Name: {_orderService.GetCustomerById(order.CustomerID)?.Name}");
                        Console.WriteLine($"Customer Email: {_orderService.GetCustomerById(order.CustomerID)?.Email}");
                        Console.WriteLine($"Order Date: {order.OrderDate}");
                        Console.WriteLine($"Total Amount: {order.TotalAmount}\n");
                        break;
                    case "2":
                        Console.WriteLine("Enter Customer Name:");
                        var customerName = Console.ReadLine();
                        // validate customer name
                        while (string.IsNullOrEmpty(customerName))
                        {
                            Console.WriteLine("Customer name cannot be empty.");
                            customerName = Console.ReadLine();
                        }
                        var orders = _orderService.SearchOrders(customerName, "CustomerName");
                        if (orders.Count() == 0)
                        {
                            Console.WriteLine("No orders found.");
                            return;
                        }
                        foreach (var odr in orders)
                        {
                            Console.WriteLine($"Order ID: {odr.Id}");
                            Console.WriteLine($"Customer ID: {odr.CustomerID}");
                            Console.WriteLine($"Customer Name: {_orderService.GetCustomerById(odr.CustomerID)?.Name}");
                            Console.WriteLine($"Customer Email: {_orderService.GetCustomerById(odr.CustomerID)?.Email}");
                            Console.WriteLine($"Order Date: {odr.OrderDate}");
                            Console.WriteLine($"Total Amount: {odr.TotalAmount}\n");
                        }
                        break;
                    case "3":
                        Console.WriteLine("Enter start date (mm/dd/yyyy):");
                        var startDate = Console.ReadLine();
                        Console.WriteLine("Enter end date (mm/dd/yyyy):");
                        var endDate = Console.ReadLine();
                        var ordersByDate = _orderService.SearchOrders($"{startDate},{endDate}", "DateRange");
                        if (ordersByDate.Count() == 0)
                        {
                            Console.WriteLine("No orders found.");
                            return;
                        }
                        foreach (var odr in ordersByDate)
                        {
                            Console.WriteLine($"Order ID: {odr.Id}");
                            Console.WriteLine($"Customer ID: {odr.CustomerID}");
                            Console.WriteLine($"Customer Name: {_orderService.GetCustomerById(odr.CustomerID)?.Name}");
                            Console.WriteLine($"Customer Email: {_orderService.GetCustomerById(odr.CustomerID)?.Email}");
                            Console.WriteLine($"Order Date: {odr.OrderDate}");
                            Console.WriteLine($"Total Amount: {odr.TotalAmount}\n");
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddOrder()
        {
            try
            {
                Console.Clear();
                var customers = _orderService.GetCustomers();
                Console.WriteLine("Customers:\n");
                foreach (var customer in customers)
                {
                    Console.WriteLine($"Customer ID: {customer.Id}, Name: {customer.Name}");
                }

                var customerId = ReadInt("Enter Customer ID:");
                var orderItems = new List<OrderItem>();
                while (true)
                {
                    var products = _orderService.GetProducts();
                    Console.WriteLine("Products:\n");
                    foreach (var product in products)
                    {
                        Console.WriteLine($"Product ID: {product.Id}, Name: {product.Name}, Quantity: {product.Quantity}, Price: {product.Price}");
                    }

                    var productId = ReadInt("Enter Product ID:");
                    var quantity = ReadInt("Enter Quantity:");

                    orderItems.Add(new OrderItem
                    {
                        ProductID = productId,
                        Quantity = quantity
                    });

                    Console.WriteLine("Add another product? (Y/N)");
                    var choice = Console.ReadLine();
                    if (choice != "Y")
                    {
                        break;
                    }
                }

                var order = new Order
                {
                    CustomerID = customerId,
                    OrderDate = DateTime.Now
                };

                _orderService.CreateOrder(order, orderItems);
                Console.WriteLine("Order created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private int ReadInt(string prompt, string errorMessage = "Invalid input. Please enter a valid number.")
        {
            Console.WriteLine(prompt);
            int value;
            while (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.WriteLine(errorMessage);
            }
            return value;
        }

        private decimal ReadDecimal(string prompt, string errorMessage = "Invalid input. Please enter a valid number.")
        {
            Console.WriteLine(prompt);
            decimal value;
            while (!decimal.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.WriteLine(errorMessage);
            }
            return value;
        }
    }
}