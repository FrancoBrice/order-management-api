using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Application.Services
{
    public class OrderService
    {
        private readonly OrderManagementDbContext _context;

        public OrderService(OrderManagementDbContext context)
        {
            _context = context;
        }

        private decimal CalculateDiscount(Order order)
        {
            decimal discount = 0;
            decimal subtotal = order.Total;

            if (subtotal > 500)
            {
                discount += subtotal * 0.10m;
                subtotal -= discount;
            }

            if (order.OrderProducts.Select(op => op.ProductId).Distinct().Count() > 5)
            {
                discount += subtotal * 0.05m;
            }

            return discount;
        }

        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders.ToList();
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.Id == id);
        }

        public void CreateOrder(Order order)
        {
            decimal total = order.OrderProducts
                .Sum(op => _context.Products
                .Where(p => p.Id == op.ProductId)
                .Select(p => p.Precio * op.Quantity)
                .FirstOrDefault());

            order.Total = total;
            decimal discount = CalculateDiscount(order);
            order.Total -= discount;

            foreach (var orderProduct in order.OrderProducts)
            {
                orderProduct.Order = null;
                orderProduct.Product = null;
            }

            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            decimal discount = CalculateDiscount(order);
            order.Total -= discount;

            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void DeleteOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
