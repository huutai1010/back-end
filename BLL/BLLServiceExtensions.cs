using BLL.Interfaces;
using BLL.Services;

using Common.AppConfiguration;


using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace BLL
{
    public static class BLLServiceExtensions
    {
        public static IServiceCollection AddBLLServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<IItineraryService, ItineraryService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<INationalityService, NationalityService>();
            services.AddScoped<IJourneyService, JourneyService>();
            services.AddScoped<IChartService, ChartService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICurrencyService, CurrencyService>();

            services.Configure<ExchangeRatesApiSettings>(configuration.GetSection("ExchangeRatesApiSettings"));
            return services;
        }
    }
}
