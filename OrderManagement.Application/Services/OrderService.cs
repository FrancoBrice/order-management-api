using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        private decimal CalculateDiscount(Order order)
        {
            decimal discount = 0;
            decimal subtotal = order.Total;

            // Descuento del 10% si el subtotal es mayor a $500
            if (subtotal > 500)
            {
                discount += subtotal * 0.10m;
                subtotal -= discount;

                // Descuento adicional del 5% si hay más de 5 productos distintos
                if (CountDistinctProductIds(order.OrderProducts) > 5)
                {
                    discount += subtotal * 0.05m;
                }
            }
            return discount;
        }

        private int CountDistinctProductIds(List<OrderProduct> orderProducts)
        {
            var productIds = new List<int>();
            foreach (var op in orderProducts)
            {
                if (!productIds.Contains(op.ProductId))
                {
                    productIds.Add(op.ProductId);
                }
            }
            return productIds.Count;
        }


        public (IEnumerable<Order> orders, int totalCount) GetOrders(int pageNumber, int pageSize)
        {
            var orders = _orderRepository.GetOrders(pageNumber, pageSize);
            var totalCount = _orderRepository.GetTotalOrdersCount();
            return (orders, totalCount);
        }

        public Order GetOrderById(int id)
        {
            return _orderRepository.GetById(id);
        }

        public void CreateOrder(Order order)
        {
            // Calcular el total de la orden sumando el precio * cantidad de cada producto
            decimal total = order.OrderProducts
                .Sum(op => _productRepository.GetById(op.ProductId)?.Precio * op.Quantity ?? 0);

            order.Total = total;
            // Aplicar el descuento calculado
            decimal discount = CalculateDiscount(order);
            order.Total -= discount;

            _orderRepository.Add(order);
        }

        public void UpdateOrder(Order order)
        {
            // Recalcular el total y el descuento en caso de modificación
            decimal total = order.OrderProducts
                .Sum(op => _productRepository.GetById(op.ProductId)?.Precio * op.Quantity ?? 0);

            order.Total = total;
            decimal discount = CalculateDiscount(order);
            order.Total -= discount;

            _orderRepository.Update(order);
        }

        public bool DeleteOrder(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null) return false;

            _orderRepository.Delete(id);
            return true;
        }
    }
}
