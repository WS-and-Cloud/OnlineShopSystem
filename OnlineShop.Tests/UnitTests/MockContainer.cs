namespace OnlineShop.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    using Moq;

    using OnlineShop.Data.Repositories;
    using OnlineShop.Models;

    public class MockContainer
    {
        public Mock<IRepository<Ad>> AdRepositoryMock { get; set; }

        public Mock<IRepository<Category>> CategoryRepositoryMock { get; set; }

        public Mock<IRepository<AdType>> AdTypeRepositoryMock { get; set; }

        public Mock<IRepository<ApplicationUser>> ApplicationUserRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeAds();

            this.SetupFakeCategories();

            this.SetupFakeAdTypes();

            this.SetupFakeUsers();
        }

        private void SetupFakeUsers()
        {
            var fakeUsers = new List<ApplicationUser>()
                            {
                                new ApplicationUser() { Id = "123", UserName = "Pesho#1" },
                                new ApplicationUser() { Id = "232", UserName = "Pesho#2" },
                                new ApplicationUser() { Id = "323", UserName = "Pesho#3" },
                            };
            this.ApplicationUserRepositoryMock = new Mock<IRepository<ApplicationUser>>();
            this.ApplicationUserRepositoryMock.Setup(r => r.All()).Returns(fakeUsers.AsQueryable());
            this.ApplicationUserRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (string id) =>
                {
                    return fakeUsers.FirstOrDefault(u => u.Id == id);
                });
        }

        private void SetupFakeAdTypes()
        {
            var fakeAdTypes = new List<AdType>()
                          {
                              new AdType() { Id = 1, Name = "Normal", Index = 100, PricePerDay = 100 },
                              new AdType() { Id = 2, Name = "Premium", Index = 200, PricePerDay = 100 },
                              new AdType() { Id = 3, Name = "Gold", Index = 300, PricePerDay = 3100 },
                          };
            this.AdTypeRepositoryMock = new Mock<IRepository<AdType>>();
            this.AdTypeRepositoryMock.Setup(r => r.All()).Returns(fakeAdTypes.AsQueryable());
            this.AdTypeRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    return fakeAdTypes.FirstOrDefault(aT => aT.Id == id);
                });
        }

        private void SetupFakeCategories()
        {
            var adTypes = new List<AdType>()
                          {
                              new AdType() { Name = "Normal", Index = 100, PricePerDay = 100},
                              new AdType() { Name = "Premium", Index = 200, PricePerDay = 100 },
                          };

            var owner = new ApplicationUser() { Id = "123", UserName = "gosho" };
            var fakeAds = new List<Ad>()
                          {
                              new Ad()
                              {
                                  Id = 1,
                                  Name = "Audi A6",
                                  Type = adTypes[0],
                                  PostedOn = DateTime.Now.AddDays(-6),
                                  Owner = owner,
                                  Price = 400
                              },
                              new Ad()
                              {
                                  Id = 2,
                                  Name = "Bmw 320",
                                  Type = adTypes[1],
                                  PostedOn = DateTime.Now.AddDays(-3),
                                  Owner = null,
                                  Price = 300
                              }
                          };

            var fakeCategories = new List<Category>()
                                 {
                                     new Category() { Id = 1, Name = "Category #1", Ads = fakeAds },
                                     new Category() { Id = 2, Name = "Category #2", Ads = fakeAds },
                                     new Category() { Id = 3, Name = "Category #3", Ads = fakeAds },
                                 };
            this.CategoryRepositoryMock = new Mock<IRepository<Category>>();

            this.CategoryRepositoryMock.Setup(r => r.All()).Returns(fakeCategories.AsQueryable());
            this.CategoryRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    return fakeCategories.FirstOrDefault(c => c.Id == id);
                });
        }

        private void SetupFakeAds()
        {
            var adTypes = new List<AdType>()
                          {
                              new AdType() { Id = 1, Name = "Normal", Index = 100, PricePerDay = 100 },
                              new AdType() { Id = 2, Name = "Premium", Index = 200, PricePerDay = 100 },
                          };
            var categories = new List<Category>()
                             {
                                 new Category() { Id = 1, Name = "Category #1" },
                                 new Category() { Id = 2, Name = "Category #2" }
                             };
            var owner = new ApplicationUser() { Id = "123", UserName = "gosho" };
            var fakeAds = new List<Ad>()
                          {
                              new Ad()
                              {
                                  Id = 1,
                                  Name = "Audi A6",
                                  Type = adTypes[0],
                                  PostedOn = DateTime.Now.AddDays(-6),
                                  Owner = owner,
                                  Price = 400,
                                  Categories = categories
                              },
                              new Ad()
                              {
                                  Id = 2,
                                  Name = "Bmw 320",
                                  Type = adTypes[1],
                                  PostedOn = DateTime.Now.AddDays(-3),
                                  Owner = owner,
                                  Price = 300,
                                  Categories = categories
                              },
                              new Ad()
                              {
                                  Id = 3,
                                  Name = "Lada",
                                  Type = adTypes[0],
                                  PostedOn = DateTime.Now.AddDays(-6),
                                  Owner = owner,
                                  Price = 1400,
                                  Categories = categories
                              },
                          };
            this.AdRepositoryMock = new Mock<IRepository<Ad>>();
            this.AdRepositoryMock.Setup(r => r.All()).Returns(fakeAds.AsQueryable());
            this.AdRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    return fakeAds.FirstOrDefault(a => a.Id == id);
                });
        }
    }
}