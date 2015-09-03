namespace OnlineShop.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Web.Http;

    using EntityFramework.Extensions;

    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OnlineShop.Data;
    using OnlineShop.Models;
    using OnlineShop.Services;

    using Owin;

    [TestClass]
    public class AdsIntegrationTests
    {
        private const string TestUsername = "pesho";

        private const string TestPassword = "Aa#1234";

        private static TestServer httpTestServer;

        private static HttpClient httpClient;

        private string accessToken;

        public string AccessToken
        {
            get
            {
                if (this.accessToken == null)
                {
                    var loginResponse = this.Login();
                    var loginData = loginResponse.Content.ReadAsAsync<LoginDTO>().Result;
                    this.accessToken = loginData.Access_Token;
                }

                return this.accessToken;
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            httpTestServer = TestServer.Create(
                appBuilder =>
                {
                    var config = new HttpConfiguration();
                    WebApiConfig.Register(config);
                    var startup = new Startup();
                    
                    startup.Configuration(appBuilder);
                    appBuilder.UseWebApi(config);
                });

            httpClient = httpTestServer.HttpClient;

            SeedDatabase();
        }

        public void AssemblyCleanup()
        {
            if (httpTestServer != null)
            {
                httpTestServer.Dispose();
            }
        }

        // Test Methods
        [TestMethod]
        public void Login_WithCorrectUsernameAndPassword_ShouldReturn200Ok_AndAcessToken()
        {
            var loginResponse = this.Login();
            var loginData = loginResponse.Content.ReadAsAsync<LoginDTO>().Result;

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.IsNotNull(loginData.Access_Token);
        }

        [TestMethod]
        public void PostAd_WithInvalidAdType_ShouldNotAddTheAdAndReturn400BadRequest()
        {
            var context = new OnlineShopDbContext();
            var category = context.Categories.FirstOrDefault();
            var adsCountBeforeInsert = context.Ads.Count();
            var data = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("name", "Opel Astra"),
                new KeyValuePair<string, string>("description", "Astra"),
                new KeyValuePair<string, string>("price", "200"),
                new KeyValuePair<string, string>("typeid", "-1"),   // Invalid Id
                new KeyValuePair<string, string>("categories", category.Id.ToString()),
            });

            var httpPostResponse = this.PostAd(data);
            var adsCountAfterInsert = context.Ads.Count();

            // Assert
            Assert.AreEqual(adsCountBeforeInsert, adsCountAfterInsert);
            Assert.AreEqual(HttpStatusCode.BadRequest, httpPostResponse.StatusCode);
        }

        [TestMethod]
        public void PostAd_WithOutCategories_ShouldNotAddTheAdAndReturn400BadRequest()
        {
            var context = new OnlineShopDbContext();
            var adsCountBeforeInsert = context.Ads.Count();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"),
                new KeyValuePair<string, string>("description", "Astra"),
                new KeyValuePair<string, string>("price", "200"),
                new KeyValuePair<string, string>("typeid", "1"),
                new KeyValuePair<string, string>("categories", ""),
            });

            var httpPostResponse = this.PostAd(data);
            var adsCountAfterInsert = context.Ads.Count();

            // Assert
            Assert.AreEqual(adsCountBeforeInsert, adsCountAfterInsert);
            Assert.AreEqual(HttpStatusCode.BadRequest, httpPostResponse.StatusCode);
        }

        [TestMethod]
        public void PostAd_WithMoreThan3Categories_ShouldNotAddTheAdAndReturn400BadRequest()
        {
            var context = new OnlineShopDbContext();
            var adsCountBeforeInsert = context.Ads.Count();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"),
                new KeyValuePair<string, string>("description", "Astra"),
                new KeyValuePair<string, string>("price", "200"),
                new KeyValuePair<string, string>("typeid", "1"),
                new KeyValuePair<string, string>("categories", "1"),
                new KeyValuePair<string, string>("categories", "2"),
                new KeyValuePair<string, string>("categories", "3"),
                new KeyValuePair<string, string>("categories", "4"),
            });

            var httpPostResponse = this.PostAd(data);
            var adsCountAfterInsert = context.Ads.Count();

            // Assert
            Assert.AreEqual(adsCountBeforeInsert, adsCountAfterInsert);
            Assert.AreEqual(HttpStatusCode.BadRequest, httpPostResponse.StatusCode);
        }

        [TestMethod]
        public void PostAd_WithOutName_ShouldNotAddTheAdAndReturn400BadRequest()
        {
            var context = new OnlineShopDbContext();
            var category = context.Categories.FirstOrDefault();
            var adType = context.AdTypes.FirstOrDefault();
            var adsCountBeforeInsert = context.Ads.Count();
            var data = new FormUrlEncodedContent(new[]
            {
                // Name is missing
                new KeyValuePair<string, string>("description", "Astra"),
                new KeyValuePair<string, string>("price", "200"),
                new KeyValuePair<string, string>("typeid", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories", category.Id.ToString()),
            });

            var httpPostResponse = this.PostAd(data);
            var adsCountAfterInsert = context.Ads.Count();

            // Assert
            Assert.AreEqual(adsCountBeforeInsert, adsCountAfterInsert);
            Assert.AreEqual(HttpStatusCode.BadRequest, httpPostResponse.StatusCode);
        }

        [TestMethod]
        public void PostAd_WithCorrectData_ShouldAddTheAdAndReturn200Ok()
        {
            var context = new OnlineShopDbContext();
            var category = context.Categories.FirstOrDefault();
            var adType = context.AdTypes.FirstOrDefault();
            var adsCountBeforeInsert = context.Ads.Count();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"),
                new KeyValuePair<string, string>("description", "Astra"),
                new KeyValuePair<string, string>("price", "200"),
                new KeyValuePair<string, string>("typeid", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories", category.Id.ToString()),
            });

            var httpPostResponse = this.PostAd(data);
            var adsCountAfterInsert = context.Ads.Count();

            // Assert
            Assert.AreEqual(adsCountBeforeInsert + 1, adsCountAfterInsert);
            Assert.AreEqual(HttpStatusCode.OK, httpPostResponse.StatusCode);
        }

        [TestInitialize]
        private static void SeedDatabase()
        {
            TestCleanUp();
            var context = new OnlineShopDbContext();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);

            var user = new ApplicationUser() { UserName = TestUsername, Email = TestUsername + "@gmail.com" };
            

            var result = userManager.CreateAsync(user, TestPassword).Result;
            if (!result.Succeeded)
            {
                Assert.Fail(string.Join(Environment.NewLine, result.Errors));
            }

            SeedCategories(context);
            SeedAdTypes(context);
            context.SaveChanges();
        }

        [TestCleanup]
        private static void TestCleanUp()
        {
            var context = new OnlineShopDbContext();

            context.Ads.Delete();
            context.AdTypes.Delete();
            context.Categories.Delete();
            context.Users.Delete();
        }

        private static void SeedCategories(OnlineShopDbContext context)
        {
            context.Categories.Add(new Category() { Name = "Category #1" });
            context.Categories.Add(new Category() { Name = "Category #2" });
        }

        private static void SeedAdTypes(OnlineShopDbContext context)
        {
            context.AdTypes.Add(new AdType() { Name = "Normal", Index = 100, PricePerDay = 100 });
            context.AdTypes.Add(new AdType() { Name = "Gold", Index = 200, PricePerDay = 1100 });
        }

        private HttpResponseMessage Login()
        {
            var loginData = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("Username", TestUsername),
                new KeyValuePair<string, string>("Password", TestPassword),
                new KeyValuePair<string, string>("grant_type", "password"),
            });

            var httpResponse = httpClient.PostAsync("/Token", loginData).Result;

            return httpResponse;
        }

        private HttpResponseMessage PostAd(FormUrlEncodedContent adDataContent)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AccessToken);
            return httpClient.PostAsync("/api/ads", adDataContent).Result;
        }
    }
}