using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagement.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderManagementDbContext _context;

        public OrderRepository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetOrders(int pageNumber, int pageSize)
        {
            return _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetTotalOrdersCount()
        {
            return _context.Orders.Count();
        }

        public Order GetById(int id)
        {
            return _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            var existingOrder = _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefault(o => o.Id == order.Id);

            if (existingOrder == null) return;

            existingOrder.Cliente = order.Cliente;
            existingOrder.FechaCreacion = order.FechaCreacion;
            existingOrder.Total = order.Total;

            _context.OrderProducts.RemoveRange(existingOrder.OrderProducts);
            foreach (var op in order.OrderProducts)
            {
                existingOrder.OrderProducts.Add(new OrderProduct
                {
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                });
            }

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                _context.OrderProducts.RemoveRange(order.OrderProducts);
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
