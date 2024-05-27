using HotelBookingBlazor.Data;
using HotelBookingBlazor.Data.Entities;
using HotelBookingBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingBlazor.Services
{
    public interface IEnquiryService
    {
        Task<MethodResult> AddEnquiryAsync(EnquiryModel model);
        Task<PagedResult<Enquiry>> GetEnquiries(int startIndex, int pageSize);
    }

    public class EnquiryService : IEnquiryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public EnquiryService(IDbContextFactory<ApplicationDbContext> contextFactory) =>
            _contextFactory = contextFactory;

        public async Task<MethodResult> AddEnquiryAsync(EnquiryModel model)
        {
            using var context = _contextFactory.CreateDbContext();
            var enquiry = new Enquiry
            {
                Email = model.Email,
                Message = model.Message,
                Name = model.Name,
                Subject = model.Subject,
                EnquiredOn = DateTime.Now
            };
            await context.Enquiries.AddAsync(enquiry);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Enquiry>> GetEnquiries(int startIndex, int pageSize)
        {
            using var context = _contextFactory.CreateDbContext();
            var totalCount = await context.Enquiries.CountAsync();
            var records = await context.Enquiries
                                       .OrderByDescending(e => e.EnquiredOn)
                                       .Skip(startIndex)
                                       .Take(pageSize)
                                       .ToArrayAsync();

            return new PagedResult<Enquiry>(totalCount, records);
        }
    }
}
