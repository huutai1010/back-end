using Common.AppConfiguration;

using DAL.DatabaseContext;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace DAL
{
    public static class DALServiceExtensions
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(databaseSettings.ConnectionString);
            });
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IPlaceRepository, PlaceRepository>();
            services.AddScoped<IItineraryRepository, ItineraryRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IChartRepository, ChartRepository>();
            services.AddScoped<IFcmTokenRepository, FcmTokenRepository>();
            services.AddScoped<IMarkPlaceRepository, MarkPlaceRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingPlaceRepository, BookingPlaceRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<INationalityRepository, NationalityRepository>();
            services.AddScoped<ITransactionDetailRepository, TransactionDetailRepository>();
            services.AddScoped<IPlaceItemRepository, PlaceItemRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
