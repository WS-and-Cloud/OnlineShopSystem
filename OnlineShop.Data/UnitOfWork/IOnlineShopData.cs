namespace OnlineShop.Data.UnitOfWork
{
    using OnlineShop.Data.Repositories;
    using OnlineShop.Models;

    public interface IOnlineShopData
    {
        IRepository<Ad> Ads { get; }

        IRepository<AdType> AdTypes { get; }

        IRepository<Category> Categories { get; }

        IRepository<ApplicationUser> ApplicationUsers { get; } 

        int SaveChanges();
    }
}
