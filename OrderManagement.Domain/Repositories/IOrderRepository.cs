using OrderManagement.Domain.Entities;
using System.Collections.Generic;

namespace OrderManagement.Domain.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders(int pageNumber, int pageSize);
        int GetTotalOrdersCount();
        Order GetById(int id);
        void Add(Order order);
        void Update(Order order);
        void Delete(int id);
    }
}
