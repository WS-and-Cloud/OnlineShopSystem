namespace OnlineShop.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using OnlineShop.Data.UnitOfWork;
    using OnlineShop.Models;
    using OnlineShop.Services.Controllers;
    using OnlineShop.Services.Models;

    [TestClass]
    public class AdsControllerTests
    {
        private MockContainer mockContainer;

        [TestInitialize]
        public void InitTest()
        {
            this.mockContainer = new MockContainer();
            this.mockContainer.PrepareMocks();
        }

        [TestMethod]
        public void GetAllAds_ShouldReturn_AllAds_SortedByIndex()
        {
            // Arrange
            var fakeAds = this.mockContainer.AdRepositoryMock.Object.All();
            var fakeUser = this.mockContainer.ApplicationUserRepositoryMock.Object.All().FirstOrDefault();
            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(r => r.Ads.All()).Returns(fakeAds.AsQueryable());
            var mockIdProvider = new Mock<IUserIdProvider>();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test no users available.");
            }

            mockIdProvider.Setup(r => r.GetUserId()).Returns(fakeUser.Id);

            // Act
            var adsController = new AdsController(mockContext.Object, mockIdProvider.Object);
            this.SetupController(adsController);
            var httpResponse = adsController.GetAllAds().ExecuteAsync(CancellationToken.None).Result;
            var result = httpResponse.Content.ReadAsAsync<IEnumerable<AllAdsViewModel>>().Result.Select(a => a.Id);
            var orderedFakeAds = fakeAds.OrderBy(a => a.TypeId).Select(a => a.Id).ToList();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            CollectionAssert.AreEqual(orderedFakeAds, result.ToList());
        }

        [TestMethod]
        public void CreateAd_WithCorrectData_ShouldSuccessfullyAddAdInRepository()
        {
            // Arrange
            var ads = new List<Ad>();
            
            // Act
            var mockContext = new Mock<IOnlineShopData>();
            var fakeAds = this.mockContainer.AdRepositoryMock.Object.All();
            var fakeAdTypes = this.mockContainer.AdTypeRepositoryMock.Object.All();
            var fakeCategories = this.mockContainer.CategoryRepositoryMock.Object.All();
            var fakeUsers = this.mockContainer.ApplicationUserRepositoryMock.Object.All();

            mockContext.Setup(r => r.Ads).Returns(this.mockContainer.AdRepositoryMock.Object);
            mockContext.Setup(r => r.AdTypes).Returns(this.mockContainer.AdTypeRepositoryMock.Object);
            mockContext.Setup(r => r.Categories).Returns(this.mockContainer.CategoryRepositoryMock.Object);
            mockContext.Setup(r => r.ApplicationUsers).Returns(this.mockContainer.ApplicationUserRepositoryMock.Object);
            var fakeUser = this.mockContainer.ApplicationUserRepositoryMock.Object.All().FirstOrDefault();
            var mockIdProvider = new Mock<IUserIdProvider>();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test no users available.");
            }

            mockIdProvider.Setup(r => r.GetUserId()).Returns(fakeUser.Id);
            
            var adsController = new AdsController(mockContext.Object, mockIdProvider.Object);
            this.SetupController(adsController);
            this.mockContainer.AdRepositoryMock.Setup(r => r.Add(It.IsAny<Ad>()))
                .Callback((Ad ad) =>
                {
                   // ad.Id = 10;   // Uncomment if id is required in the test
                    ad.Owner = fakeUser;
                    ads.Add(ad);
                });
            var randomName = Guid.NewGuid().ToString();
            var newAd = new CreateAdBindingModel()
                        {
                            Name = randomName,
                            Price = 555,
                            TypeId = 1,
                            Description = "Put description here",
                            Categories = new[] { 1, 2 }
                        };

            var httpResponse = adsController.CreateAd(newAd).ExecuteAsync(CancellationToken.None).Result;
           
            // Assert
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.AreEqual(1, ads.Count);
            Assert.AreEqual(ads[0].Name, newAd.Name);
        }

        [TestMethod]
        public void ClosingAdd_AsOwner_ShouldReturn200Ok()
        {
            // Arrange
            var fakeAds = this.mockContainer.AdRepositoryMock.Object.All();
            
            // Act
            var openAd = fakeAds.FirstOrDefault(a => a.Status == AdStatus.Open);
            if (openAd == null)
            {
                Assert.Fail("Cannot perform test there is no open ads.");
            }

            var adId = openAd.Id;
            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads).Returns(this.mockContainer.AdRepositoryMock.Object);
            var mockProvider = new Mock<IUserIdProvider>();
            mockProvider.Setup(ip => ip.GetUserId()).Returns(openAd.OwnerId);

            var adsController = new AdsController(mockContext.Object, mockProvider.Object);
            this.SetupController(adsController);
            var httpResponse = adsController.CloseAd(adId).ExecuteAsync(CancellationToken.None).Result;

            // Assert 
            mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.AreEqual(AdStatus.Closed, openAd.Status);
            Assert.IsNotNull(openAd.ClosedOn);
        }

        [TestMethod]
        public void CloseAd_AsNotAdOwner_ShouldReturn400BadRequest()
        {
            // Arrange
            var fakeAds = this.mockContainer.AdRepositoryMock.Object.All();
            
            // Act
            var openAd = fakeAds.FirstOrDefault(a => a.Status == AdStatus.Open);
            if (openAd == null)
            {
                Assert.Fail("Cannot perform test, because there are no open ads.");
            }

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads).Returns(this.mockContainer.AdRepositoryMock.Object);

            var mockProvider = new Mock<IUserIdProvider>();
            var fakeUsers = this.mockContainer.ApplicationUserRepositoryMock.Object.All();
            var nonExistingUser = fakeUsers.FirstOrDefault();
            mockProvider.Setup(ip => ip.GetUserId()).Returns(nonExistingUser.Id);
            var adsController = new AdsController(mockContext.Object, mockProvider.Object);
            this.SetupController(adsController);

            var httpResponse = adsController.CloseAd(openAd.Id).ExecuteAsync(CancellationToken.None).Result;

            // Assert 
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            Assert.AreEqual(AdStatus.Open, openAd.Status);
            Assert.IsNull(openAd.ClosedOn);
        }

        private void SetupController(BaseApiController controller)
        {
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
        }
    }
}