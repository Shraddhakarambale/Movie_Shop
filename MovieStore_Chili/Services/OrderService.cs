﻿using Microsoft.EntityFrameworkCore;
using MovieStore_Chili.Data;
using MovieStore_Chili.Models.Database;
using System.Linq;

namespace MovieStore_Chili.Services
{

    public class OrderService : IOrderService
    {
        private readonly MovieDbContext _db;
        private readonly IConfiguration _configuration;
        public int Id { get; private set; }

        public OrderService(MovieDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public List<Order> GetAllOrder()
        {
            var orders = _db.Orders.ToList();
            return orders;
        }

        public List<Order> GetOrderInDetail()
        {

             return _db.Orders
            .Include(o => o.OrderRows)
            .ThenInclude(or => or.Movie)
            .Include(o => o.Customer)
            .ToList();
        }

            public Order GetOrderById(int id)
        {
            var order = _db.Orders.Include(o => o.Customer)
                                  .Include(o => o.OrderRows)
                                  .ThenInclude(or => or.Movie)
                                  .Single(o => o.Id == id);
            return order;

        }
        public void AddOrder(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
        }
        public void UpdateOrder(Order order)
        {
            _db.Orders.Update(order);
            _db.SaveChanges();

        }
        public void DeleteOrder(int orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                _db.Orders.Remove(order);
                _db.SaveChanges();
            }
        }
        public List<Order> GetCustomerOrders(string email)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Email == email);

            if (customer == null)
            {
                return null;
            }
            var customerOrders = _db.Orders
                 .Where(o => o.CustomerId == customer.Id)
                 .Include(o => o.OrderRows)
                     .ThenInclude(or => or.Movie)
                     .Include(o =>o.Customer)
                 .ToList();

            return customerOrders;
        }
        public async Task<Customer> FindCustomerWithMostExpensiveOrder()
        {
            var customers =await _db.Customers.ToListAsync();
            var customerWithMostExpensiveOrder =customers.OrderByDescending(c => c.Orders.Sum(o => o.OrderRows.Sum(or => or.Price))).FirstOrDefault();
            return customerWithMostExpensiveOrder;
        }
       
    }
}
