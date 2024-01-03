using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DAL.Entities;

namespace DAL.DatabaseContext
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<BookingPlace> BookingPlaces { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryLanguage> CategoryLanguages { get; set; } = null!;
        public virtual DbSet<CelebrateImage> CelebrateImages { get; set; } = null!;
        public virtual DbSet<ConfigLanguage> ConfigLanguages { get; set; } = null!;
        public virtual DbSet<Conversation> Conversations { get; set; } = null!;
        public virtual DbSet<FcmToken> FcmTokens { get; set; } = null!;
        public virtual DbSet<FeedBack> FeedBacks { get; set; } = null!;
        public virtual DbSet<ItemDescription> ItemDescriptions { get; set; } = null!;
        public virtual DbSet<Itinerary> Itineraries { get; set; } = null!;
        public virtual DbSet<ItineraryDescription> ItineraryDescriptions { get; set; } = null!;
        public virtual DbSet<ItineraryPlace> ItineraryPlaces { get; set; } = null!;
        public virtual DbSet<Journey> Journeys { get; set; } = null!;
        public virtual DbSet<MarkPlace> MarkPlaces { get; set; } = null!;
        public virtual DbSet<Nationality> Nationalities { get; set; } = null!;
        public virtual DbSet<Place> Places { get; set; } = null!;
        public virtual DbSet<PlaceCategory> PlaceCategories { get; set; } = null!;
        public virtual DbSet<PlaceDescription> PlaceDescriptions { get; set; } = null!;
        public virtual DbSet<PlaceImage> PlaceImages { get; set; } = null!;
        public virtual DbSet<PlaceItem> PlaceItems { get; set; } = null!;
        public virtual DbSet<PlaceTime> PlaceTimes { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<TransactionDetail> TransactionDetails { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Email, "UC_Email")
                    .IsUnique();

                entity.HasIndex(e => e.Phone, "UQ_Phone")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(300);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.NationalCode).HasMaxLength(10);

                entity.Property(e => e.Password).HasMaxLength(150);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.ConfigLanguage)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.ConfigLanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_ConfigLanguage");

                entity.HasOne(d => d.NationalCodeNavigation)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.NationalCode)
                    .HasConstraintName("FK_Account_Nationality");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Account");
            });

            modelBuilder.Entity<BookingPlace>(entity =>
            {
                entity.ToTable("BookingPlace");

                entity.Property(e => e.ExpiredTime).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingPlaces)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingPlace_Booking");

                entity.HasOne(d => d.Journey)
                    .WithMany(p => p.BookingPlaces)
                    .HasForeignKey(d => d.JourneyId)
                    .HasConstraintName("FK_BookingPlace_Journey");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.BookingPlaces)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingPlace_Place");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CategoryLanguage>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.ToTable("CategoryLanguage");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.NameLanguage).HasMaxLength(150);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryLanguages)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryLanguage_Category");
            });

            modelBuilder.Entity<CelebrateImage>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0); 
                entity.ToTable("CelebrateImage");

                entity.Property(e => e.ImageUrl).IsUnicode(false);

                entity.HasOne(d => d.BookingDetail)
                    .WithMany(p => p.CelebrateImages)
                    .HasForeignKey(d => d.BookingDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CelebrateImage_BookingPlace");
            });

            modelBuilder.Entity<ConfigLanguage>(entity =>
            {
                entity.ToTable("ConfigLanguage");

                entity.HasIndex(e => e.LanguageCode, "UQ_ConfigLanguage")
                    .IsUnique();

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.FileLink)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Icon).IsUnicode(false);

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.HasKey(e => new { e.AccountOneId, e.AccountTwoId })
                    .HasName("PK_Conversation_1");

                entity.ToTable("Conversation");

                entity.HasIndex(e => e.SessionId, "IX_Conversation_1")
                    .IsUnique();

                entity.Property(e => e.AccountOneId).HasColumnName("Account_One_Id");

                entity.Property(e => e.AccountTwoId).HasColumnName("Account_Two_Id");

                entity.Property(e => e.ChannelId).IsUnicode(false);

                entity.Property(e => e.SessionId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.AccountOne)
                    .WithMany(p => p.ConversationAccountOnes)
                    .HasForeignKey(d => d.AccountOneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Conversation_Account");

                entity.HasOne(d => d.AccountTwo)
                    .WithMany(p => p.ConversationAccountTwos)
                    .HasForeignKey(d => d.AccountTwoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Conversation_Account1");
            });

            modelBuilder.Entity<FcmToken>(entity =>
            {
                entity.ToTable("FcmToken");

                entity.Property(e => e.Token).IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.FcmTokens)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FcmToken_Account");
            });

            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.ToTable("FeedBack");

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.FeedBacks)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedBack_Account");

                entity.HasOne(d => d.Itinerary)
                    .WithMany(p => p.FeedBacks)
                    .HasForeignKey(d => d.ItineraryId)
                    .HasConstraintName("FK_FeedBack_Tour1");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.FeedBacks)
                    .HasForeignKey(d => d.PlaceId)
                    .HasConstraintName("FK_FeedBack_Place1");
            });

            modelBuilder.Entity<ItemDescription>(entity =>
            {
                entity.ToTable("ItemDescription");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.NameItem).HasMaxLength(50);

                entity.HasOne(d => d.PlaceItem)
                    .WithMany(p => p.ItemDescriptions)
                    .HasForeignKey(d => d.PlaceItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ItemDescription_PlaceItem");
            });

            modelBuilder.Entity<Itinerary>(entity =>
            {
                entity.ToTable("Itinerary");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.CreateBy)
                    .WithMany(p => p.Itineraries)
                    .HasForeignKey(d => d.CreateById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tour_Account");
            });

            modelBuilder.Entity<ItineraryDescription>(entity =>
            {
                entity.ToTable("ItineraryDescription");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourDescriptions)
                    .HasForeignKey(d => d.ItineraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourDescription_Tour");
            });

            modelBuilder.Entity<ItineraryPlace>(entity =>
            {
                entity.HasKey(e => new { e.PlaceId, e.ItineraryId })
                    .HasName("PK_TourDetail");

                entity.ToTable("ItineraryPlace");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.TourDetails)
                    .HasForeignKey(d => d.ItineraryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourDetail_Tour");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.TourDetails)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TourDetail_Place");
            });

            modelBuilder.Entity<Journey>(entity =>
            {
                entity.ToTable("Journey");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarkPlace>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.PlaceId });

                entity.ToTable("MarkPlace");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MarkPlaces)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkPlace_Account");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.MarkPlaces)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MarkPlace_Place");
            });

            modelBuilder.Entity<Nationality>(entity =>
            {
                entity.HasKey(e => e.NationalCode);

                entity.ToTable("Nationality");

                entity.Property(e => e.NationalCode).HasMaxLength(10);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.LanguageName).HasMaxLength(100);

                entity.Property(e => e.NationalName).HasMaxLength(100);

                entity.Property(e => e.PhoneCode).HasMaxLength(10);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);

                entity.ToTable("Place");

                entity.Property(e => e.Address).HasMaxLength(150);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.EntryTicket).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GooglePlaceId)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("GooglePlaceID");

                entity.Property(e => e.Latitude).HasColumnType("decimal(8, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<PlaceCategory>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.HasKey(e => new { e.PlaceId, e.CategoryId });

                entity.ToTable("PlaceCategory");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.PlaceCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceCategory_Category");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PlaceCategories)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceCategory_Place");
            });

            modelBuilder.Entity<PlaceDescription>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.ToTable("PlaceDescription");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.LanguageCode)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.VoiceFile).IsUnicode(false);

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PlaceDescriptions)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceDescription_Place");
            });

            modelBuilder.Entity<PlaceImage>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.ToTable("PlaceImage");

                entity.Property(e => e.Url).IsUnicode(false);

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PlaceImages)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceImage_Place");
            });

            modelBuilder.Entity<PlaceItem>(entity =>
            {
                entity.ToTable("PlaceItem");

                entity.Property(e => e.BeaconId).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PlaceItems)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceItem_Place");
            });

            modelBuilder.Entity<PlaceTime>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.ToTable("PlaceTime");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.PlaceTimes)
                    .HasForeignKey(d => d.PlaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlaceTime_Place");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasQueryFilter(b => b.Status != 0);
                entity.ToTable("Role");

                entity.Property(e => e.RoleName).HasMaxLength(150);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(800);

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_Account");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Booking");
            });

            modelBuilder.Entity<TransactionDetail>(entity =>
            {
                entity.ToTable("TransactionDetail");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionDetails)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TransactionDetail_Transaction");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
