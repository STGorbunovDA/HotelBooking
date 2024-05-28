using HotelBookingBlazor.Data;
using HotelBookingBlazor.Data.Entities;
using HotelBookingBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services
{
    public interface ISubscriberService
    {
        Task<PagedResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize);
        Task<MethodResult> SubscribeAsync(string email);
    }

    public class SubscriberService : ISubscriberService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SubscriberService(IDbContextFactory<ApplicationDbContext> contextFactory) =>
            _contextFactory = contextFactory;

        public async Task<MethodResult> SubscribeAsync(string email)
        {
            using var context = _contextFactory.CreateDbContext();
            var subscriber = new Subscriber
            {
                Email = email,
                SubscriberOn = DateTime.Now
            };
            await context.Subscribers.AddAsync(subscriber);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Subscriber>> GetSubscribersAsync(int startIndex, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var totalCount = await context.Subscribers.CountAsync();
            var records = await context.Subscribers
                                       .OrderByDescending(e => e.SubscriberOn)
                                       .Skip(startIndex)
                                       .Take(pageSize)
                                       .ToArrayAsync();

            return new PagedResult<Subscriber>(totalCount, records);
        }
    }
}
