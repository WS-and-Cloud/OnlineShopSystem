namespace OnlineShop.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Microsoft.AspNet.Identity.EntityFramework;

    using OnlineShop.Data.Migrations;
    using OnlineShop.Models;

    public class OnlineShopDbContext : IdentityDbContext<ApplicationUser>
    {
        public OnlineShopDbContext()
            : base("name=OnlineShopDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OnlineShopDbContext, Configuration>());
        }

        public virtual IDbSet<Ad> Ads { get; set; }

        public virtual IDbSet<Category> Categories { get; set; }

        public virtual IDbSet<AdType> AdTypes { get; set; }

        public static OnlineShopDbContext Create()
        {
            return new OnlineShopDbContext();
        }
    }
}