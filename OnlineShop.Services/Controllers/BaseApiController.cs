namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;

    using OnlineShop.Data;
    using OnlineShop.Data.UnitOfWork;
    using OnlineShop.Tests.UnitTests;

    public class BaseApiController : ApiController
    {
        private IOnlineShopData data;


        public BaseApiController(IOnlineShopData data, IUserIdProvider userIdProvier)
        {
            this.Data = data;
            this.UserIdProvider = userIdProvier;
        }

        protected IUserIdProvider UserIdProvider { get; set; }

        protected IOnlineShopData Data
        {
            get
            {
                return this.data;
            }

            set
            {
                this.data = value;
            }
        }
    }
}
