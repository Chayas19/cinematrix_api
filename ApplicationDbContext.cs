using CineMatrix_API.Models;
using Microsoft.EntityFrameworkCore;


namespace CineMatrix_API
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<OTPVerification> OTP { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Refreshtoken> RefreshTokens { get; set; }
        public DbSet<MovieActors> MovieActors { get; set; }
        public DbSet<MovieGenres> MovieGenres { get; set; }

      
        public DbSet<MovieLanguage> MoviesLanguages { get; set; }
        public DbSet<SupportTicket> ContactHelp { get; set; }

        // public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }

        public DbSet<MoviePoster> MoviePosters { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscribe>()
               .HasOne(s => s.User)              
               .WithMany(u => u.Subscribes)     
               .HasForeignKey(s => s.UserId)   
               .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Actor>()
                .Property(m => m.Picture)
                .HasColumnName("Picture");


            modelBuilder.Entity<Movie>()
                .Property(m => m.PosterData)
                .HasColumnName("PosterData");

            modelBuilder.Entity<MovieActors>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<MovieGenres>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieLanguage>()
                .HasKey(ml => new { ml.MovieId, ml.LanguageId });

            modelBuilder.Entity<WatchHistory>();


            modelBuilder.Entity<WatchHistory>()
            .HasOne(wh => wh.User)
            .WithMany(u => u.WatchHistories)
            .HasForeignKey(wh => wh.UserId);

            modelBuilder.Entity<WatchHistory>()
                .HasOne(wh => wh.Movie)
                .WithMany(m => m.WatchHistories)
                .HasForeignKey(wh => wh.MovieId);


            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId);


            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);


            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)"); 


            modelBuilder.Entity<OTPVerification>()
               .HasOne(o => o.User)
               .WithMany(u => u.OtpCodes)
               .HasForeignKey(o => o.UserId);


            modelBuilder.Entity<UserRoles>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });


        }
    }

}