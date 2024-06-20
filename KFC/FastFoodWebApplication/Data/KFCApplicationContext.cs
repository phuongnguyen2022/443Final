using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KFCApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace KFCApplication.Data
{
    public class KFCApplicationContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public KFCApplicationContext(DbContextOptions<KFCApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<DishType> DishType { get; set; } = default!;

        public DbSet<Dish> Dish { get; set; }

        public DbSet<Profile> Profile { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OrderDetail>().HasNoKey();
            builder.Entity<UserVoucher>().HasNoKey();
            /* builder.Entity<User>().ToTable("User");*/


            builder.Entity<Dish>().ToTable("Dish");
            builder.Entity<DishType>().ToTable("DishType");
            builder.Entity<Profile>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<Profile>(p => p.UserId);
            builder.Entity<Cart>().ToTable("Cart");
            builder.Entity<Order>().ToTable("Order");
            builder.Entity<OrderDetail>().ToTable("OrderDetail");
            builder.Entity<Voucher>().ToTable("Voucher");
            builder.Entity<UserVoucher>().ToTable("UserVoucher");

        }
        public DbSet<KFCApplication.Models.Voucher> Voucher { get; set; }
        public DbSet<KFCApplication.Models.UserVoucher> UserVoucher { get; set; }



    }
}
