using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Tests
{
    public class OrderDiscountTests
    {
        [Fact]
        public void CreateOrder_NoDiscount_WhenTotalIsLessOrEqual500_AndFewProducts()
        {
            var order = new Order
            {
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct { ProductId = 1, Quantity = 1 },
                    new OrderProduct { ProductId = 2, Quantity = 1 }
                }
            };

            var productRepoMock = new Mock<IProductRepository>();
            productRepoMock.Setup(repo => repo.GetById(1)).Returns(new Product { Id = 1, Precio = 200 });
            productRepoMock.Setup(repo => repo.GetById(2)).Returns(new Product { Id = 2, Precio = 200 });

            var orderRepoMock = new Mock<IOrderRepository>();

            var service = new OrderService(orderRepoMock.Object, productRepoMock.Object);
            service.CreateOrder(order);
            Assert.Equal(400, order.Total);
        }

        [Fact]
        public void CreateOrder_Applies10PercentDiscount_WhenTotalGreaterThan500_AndFewProducts()
        {
            var order = new Order
            {
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct { ProductId = 1, Quantity = 1 },
                    new OrderProduct { ProductId = 2, Quantity = 1 }
                }
            };

            var productRepoMock = new Mock<IProductRepository>();
            productRepoMock.Setup(repo => repo.GetById(1)).Returns(new Product { Id = 1, Precio = 300 });
            productRepoMock.Setup(repo => repo.GetById(2)).Returns(new Product { Id = 2, Precio = 300 });

            var orderRepoMock = new Mock<IOrderRepository>();

            var service = new OrderService(orderRepoMock.Object, productRepoMock.Object);

            service.CreateOrder(order);
            Assert.Equal(540, order.Total);
        }

        [Fact]
        public void CreateOrder_Applies5PercentDiscount_WhenTotalIsLessOrEqual500_ButManyDistinctProducts()
        {
            var order = new Order
            {
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct { ProductId = 1, Quantity = 1 },
                    new OrderProduct { ProductId = 2, Quantity = 1 },
                    new OrderProduct { ProductId = 3, Quantity = 1 },
                    new OrderProduct { ProductId = 4, Quantity = 1 },
                    new OrderProduct { ProductId = 5, Quantity = 1 },
                    new OrderProduct { ProductId = 6, Quantity = 1 }
                }
            };

            var productRepoMock = new Mock<IProductRepository>();
            productRepoMock.Setup(repo => repo.GetById(1)).Returns(new Product { Id = 1, Precio = 50 });
            productRepoMock.Setup(repo => repo.GetById(2)).Returns(new Product { Id = 2, Precio = 50 });
            productRepoMock.Setup(repo => repo.GetById(3)).Returns(new Product { Id = 3, Precio = 50 });
            productRepoMock.Setup(repo => repo.GetById(4)).Returns(new Product { Id = 4, Precio = 50 });
            productRepoMock.Setup(repo => repo.GetById(5)).Returns(new Product { Id = 5, Precio = 50 });
            productRepoMock.Setup(repo => repo.GetById(6)).Returns(new Product { Id = 6, Precio = 150 });

            var orderRepoMock = new Mock<IOrderRepository>();

            var service = new OrderService(orderRepoMock.Object, productRepoMock.Object);

            service.CreateOrder(order);

            Assert.Equal(380, order.Total);
        }

        [Fact]
        public void CreateOrder_AppliesCombinedDiscount_WhenTotalGreaterThan500_AndManyDistinctProducts()
        {
            var order = new Order
            {
                OrderProducts = new List<OrderProduct>
                {
                    new OrderProduct { ProductId = 1, Quantity = 1 },
                    new OrderProduct { ProductId = 2, Quantity = 1 },
                    new OrderProduct { ProductId = 3, Quantity = 1 },
                    new OrderProduct { ProductId = 4, Quantity = 1 },
                    new OrderProduct { ProductId = 5, Quantity = 1 },
                    new OrderProduct { ProductId = 6, Quantity = 1 }
                }
            };

            var productRepoMock = new Mock<IProductRepository>();
            productRepoMock.Setup(repo => repo.GetById(1)).Returns(new Product { Id = 1, Precio = 100 });
            productRepoMock.Setup(repo => repo.GetById(2)).Returns(new Product { Id = 2, Precio = 100 });
            productRepoMock.Setup(repo => repo.GetById(3)).Returns(new Product { Id = 3, Precio = 100 });
            productRepoMock.Setup(repo => repo.GetById(4)).Returns(new Product { Id = 4, Precio = 100 });
            productRepoMock.Setup(repo => repo.GetById(5)).Returns(new Product { Id = 5, Precio = 100 });
            productRepoMock.Setup(repo => repo.GetById(6)).Returns(new Product { Id = 6, Precio = 200 });
            // Total = 100+100+100+100+100+200 = 700

            var orderRepoMock = new Mock<IOrderRepository>();

            var service = new OrderService(orderRepoMock.Object, productRepoMock.Object);

            service.CreateOrder(order);
            // Primer descuento: 700 * 0.10 = 70  --> nuevo subtotal = 700 - 70 = 630.
            // Segundo descuento: 630 * 0.05 = 31.5.
            // Descuento total = 70 + 31.5 = 101.5.
            // Total final = 700 - 101.5 = 598.5.
            Assert.Equal(598.5m, order.Total);
        }
    }
}
