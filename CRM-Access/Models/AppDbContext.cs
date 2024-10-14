using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM_Access.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

       //     modelBuilder.Entity<Review>()
       //.HasOne(r => r.ParentReview)
       //.WithMany(r => r.Reviews)
       //.HasForeignKey(r => r.ParentReviewId)
       //.OnDelete(DeleteBehavior.Restrict); // To avoid cycles or multiple cascade paths

       //     // Relationship with AppUser
       //     modelBuilder.Entity<Review>()
       //         .HasOne(r => r.AppUser)
       //         .WithMany(u => u.Reviews)
       //         .HasForeignKey(r => r.AppUserId)
       //         .OnDelete(DeleteBehavior.Cascade);

     

            modelBuilder.Entity<AppRole>().HasData(
               new AppRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                 new AppRole { Id = 2, Name = "User", NormalizedName = "USER" }
             );
            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(
              new AppUser
              {
                  Id = 1,
                  UserName = "ilkin.admin",
                  Name = "Ilkin",
                  Surname = "Novruzov",
                  CreatedAt=DateTime.Now,
                  ImageUrl ="Image",
                  PasswordHash = hasher.HashPassword(null, "Admin.1234"),
                  Email = "inovruzov2004@gmail.com",
                  EmailConfirmed = true,
                  NormalizedUserName = "ILKIN.ADMIN",
                  NormalizedEmail = "INOVRUZOV2004@GMAIL.COM",
                  LockoutEnabled = true,
                  SecurityStamp = Guid.NewGuid().ToString()
              }
              );

            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
       new IdentityUserRole<int> { UserId = 1, RoleId = 1 });


            //modelBuilder.Entity<Review>().Navigation(r => r.AppUser).AutoInclude();

            base.OnModelCreating(modelBuilder);
        }
    }
}
