using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Controllers;
using System;

namespace OrderManagement.Tests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly OrderService _orderService;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object);
            _orderController = new OrderController(_orderService);
        }

        [Fact]
        public void GetOrders_ReturnsOkResult_WithOrdersList()
        {
            var orders = new List<Order>
            {
                new Order { Id = 1, Cliente = "John Doe", Total = 100 },
                new Order { Id = 2, Cliente = "Jane Doe", Total = 200 }
            };

            int totalCount = 33;
            int pageNumber = 1;
            int pageSize = 2;

            _orderRepositoryMock.Setup(repo => repo.GetOrders(It.IsAny<int>(), It.IsAny<int>())).Returns(orders);
            _orderRepositoryMock.Setup(repo => repo.GetTotalOrdersCount()).Returns(totalCount);

            var actionResult = _orderController.GetOrders(pageNumber, pageSize);

            Assert.IsType<OkObjectResult>(actionResult);
            var result = actionResult as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var json = JsonConvert.SerializeObject(result.Value);
            var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            Assert.NotNull(responseDict);
            Assert.True(responseDict.ContainsKey("TotalCount"));
            Assert.True(responseDict.ContainsKey("PageNumber"));
            Assert.True(responseDict.ContainsKey("PageSize"));
            Assert.True(responseDict.ContainsKey("Orders"));
            Assert.Equal(totalCount, Convert.ToInt32(responseDict["TotalCount"]));
            Assert.Equal(pageNumber, Convert.ToInt32(responseDict["PageNumber"]));
            Assert.Equal(pageSize, Convert.ToInt32(responseDict["PageSize"]));

            var ordersListJson = responseDict["Orders"].ToString();
            var ordersList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ordersListJson);
            Assert.NotNull(ordersList);
            Assert.Equal(2, ordersList.Count);

            Assert.Equal("John Doe", ordersList[0]["Cliente"].ToString());
            Assert.Equal("Jane Doe", ordersList[1]["Cliente"].ToString());
        }

        [Fact]
        public void GetOrderById_ExistingId_ReturnsOkResult()
        {
            var order = new Order { Id = 1, Cliente = "John Doe", Total = 100 };
            _orderRepositoryMock.Setup(repo => repo.GetById(1)).Returns(order);

            var result = _orderController.GetOrderById(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(order, result.Value);
        }

        [Fact]
        public void GetOrderById_NonExistingId_ReturnsNotFound()
        {
            _orderRepositoryMock.Setup(repo => repo.GetById(999)).Returns((Order)null);

            var result = _orderController.GetOrderById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CreateOrder_ValidOrder_ReturnsCreatedResult()
        {
            var order = new Order { Id = 1, Cliente = "John Doe", Total = 100 };
            _orderRepositoryMock.Setup(repo => repo.Add(order));

            var result = _orderController.CreateOrder(order) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("GetOrderById", result.ActionName);
        }

        [Fact]
        public void UpdateOrder_ValidOrder_ReturnsNoContent()
        {
            var order = new Order { Id = 1, Cliente = "John Doe", Total = 100 };
            _orderRepositoryMock.Setup(repo => repo.Update(order));

            var result = _orderController.UpdateOrder(1, order);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteOrder_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var order = new Order { Id = 1, Cliente = "John Doe", Total = 100 };

            _orderRepositoryMock.Setup(repo => repo.GetById(1)).Returns(order);
            _orderRepositoryMock.Setup(repo => repo.Delete(1));

            // Act
            var result = _orderController.DeleteOrder(1);

            // Debug
            Console.WriteLine($"Tipo de resultado: {result?.GetType().Name ?? "null"}");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteOrder_NonExistingId_ReturnsNotFound()
        {
            _orderRepositoryMock.Setup(repo => repo.GetById(999)).Returns((Order)null);  

            var result = _orderController.DeleteOrder(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
