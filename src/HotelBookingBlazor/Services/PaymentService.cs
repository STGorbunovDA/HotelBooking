using HotelBookingBlazor.Data;
using HotelBookingBlazor.Data.Entities;
using HotelBookingBlazor.Models;
using HotelBookingBlazor.Models.Public;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace HotelBookingBlazor.Services
{
    public interface IPaymentService
    {
        Task<string> GeneratePaymentUrl(PaymentModel model, string userId, string domain);
        Task<MethodResult<string?>> ConfirmPaymentAsync(string paymentIdStr, long bookingId, string checkoutSessionId);
        Task<MethodResult> CancelPaymentAsync(string paymentIdStr, long bookingId, string checkoutSessionId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string StripePaymentInitiated = "initiated ";
        private const string StripePaymentSuccess = "paid";
        private const string StripePaymentFail = "unpaid";
        private const string StripePaymentPending = "Pending";
        public PaymentService(IDbContextFactory<ApplicationDbContext> contextFactory,
                              UserManager<ApplicationUser> userManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
        }

        public async Task<string> GeneratePaymentUrl(PaymentModel model, string userId, string domain)
        {
            using var context = _contextFactory.CreateDbContext();

            var paymentEntity = new Payment
            {
                BookingId = model.BookingId,
                CreatedAt = DateTime.Now,
                Status = StripePaymentInitiated,
                CheckoutSessionId = StripePaymentPending
            };
            await context.Payments.AddAsync(paymentEntity);
            await context.SaveChangesAsync();

            var totalAmount = model.NoOfDays * model.Price * 100;

            var lineItem = new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "rub",
                    UnitAmountDecimal = totalAmount,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = model.RoomTypeName,
                        Description = $"Бронирование номера: {model.RoomTypeName}, на {model.NoOfDays} {(model.NoOfDays > 1 ? "дней" : "день")}, за стоимость: {model.Price:c}/ночь"
                    }
                }
            };

            var sessionCreateOptions = new SessionCreateOptions
            {
                LineItems = [lineItem],
                Mode = "payment",
                SuccessUrl = $"{domain}/bookings/{model.BookingId}" + "/success?session-id={CHECKOUT_SESSION_ID}&payment-id=" + paymentEntity.Id,
                CancelUrl = $"{domain}/bookings/{model.BookingId}/cancel?session-id={{CHECKOUT_SESSION_ID}}&payment-id={paymentEntity.Id}"
            };


            var sessionService = new SessionService();

            var session = await sessionService.CreateAsync(sessionCreateOptions);

            paymentEntity.CheckoutSessionId = session.Id;
            await context.SaveChangesAsync();

            return session.Url;

        }

        public async Task<MethodResult<string?>> ConfirmPaymentAsync(string paymentIdStr, long bookingId, string checkoutSessionId)
        {
            using var context = _contextFactory.CreateDbContext();
            var paymentEntity = await context.Payments
                                             .AsTracking()
                                             .FirstOrDefaultAsync(p => p.Id == Guid.Parse(paymentIdStr)
                                                            && p.CheckoutSessionId == checkoutSessionId);

            if (paymentEntity is null)
            {
                return new MethodResult<string?>(false, "Invalid payment id", default);
            }

            var booking = await context.Bookings
                                           .AsTracking()
                                           .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking is null)
            {
                return new MethodResult<string?>(false, "Invalid booking id", default);
            }


            if (paymentEntity.Status != StripePaymentInitiated)
            {
                // У нас уже есть этот статус оплаты
                //  нам не нужно ничего делать с базой данных
            }
            else
            {
                var sessionService = new SessionService();
                var checkoutSession = await sessionService.GetAsync(checkoutSessionId);
                if (checkoutSession is null)
                {
                    return new MethodResult<string?>(false, "Invalid checkout session", default);
                }


                paymentEntity.Status = checkoutSession.PaymentStatus;
                paymentEntity.AdditionalInfo = $"Name: {checkoutSession.CustomerDetails.Name} " +
                                               $"Email: {checkoutSession.CustomerDetails.Email}";

                
                booking.Status = checkoutSession.PaymentStatus == StripePaymentSuccess
                                    ? Constants.BookingStatus.PaymentSuccess
                                    : Constants.BookingStatus.PaymentCancelled;

                await context.SaveChangesAsync();
            }
            var guestName = await _userManager.Users
                                              .Where(u => u.Id == booking.GuestId)
                                              .Select(u => u.FullName)
                                              .FirstOrDefaultAsync();
            return new MethodResult<string?>(true, null, guestName);
        }
    
        public async Task<MethodResult> CancelPaymentAsync(string paymentIdStr, long bookingId, string checkoutSessionId)
        {
            using var context = _contextFactory.CreateDbContext();
            var paymentEntity = await context.Payments
                                             .AsTracking()
                                             .FirstOrDefaultAsync(p => p.Id == Guid.Parse(paymentIdStr)
                                                            && p.CheckoutSessionId == checkoutSessionId);

            if (paymentEntity is null)
            {
                return new MethodResult(false, "Invalid payment id");
            }

            var booking = await context.Bookings
                                           .AsTracking()
                                           .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking is null)
            {
                return new MethodResult(false, "Invalid booking id");
            }


            if(paymentEntity.Status == StripePaymentInitiated)
            {
                var sessionService = new SessionService();
                var checkoutSession = await sessionService.GetAsync(checkoutSessionId);
                if (checkoutSession is null)
                {
                    return new MethodResult(false, "Invalid checkout session");
                }


                paymentEntity.Status = "cancelled";
                paymentEntity.AdditionalInfo = "Payment cancelled by Guest";


                booking.Status = Constants.BookingStatus.PaymentCancelled;

                await context.SaveChangesAsync();
            }
            var guestName = await _userManager.Users
                                              .Where(u => u.Id == booking.GuestId)
                                              .Select(u => u.FullName)
                                              .FirstOrDefaultAsync();
            return true;
        }
    }
}
