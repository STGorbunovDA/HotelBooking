using HotelBookingBlazor.Models.Public;
using Stripe.Checkout;

namespace HotelBookingBlazor.Services
{
    public interface IPaymentService
    {
        Task<string> GeneratePaymentUrl(PaymentModel model, string userId, string domain);
    }

    public class PaymentService : IPaymentService
    {
        public async Task<string> GeneratePaymentUrl(PaymentModel model, string userId, string domain)
        {
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
                SuccessUrl = domain + "/booking-success?session-id={CHECKOUT_SESSION_ID}",
                CancelUrl = $"{domain}/booking-cancel"
            };


            var sessionService = new SessionService();

            var session = await sessionService.CreateAsync(sessionCreateOptions);
            return session.Url;

        }
    }
}
