namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;

    using OnlineShop.Data;
    using OnlineShop.Data.UnitOfWork;

    public class BaseApiController : ApiController
    {
        private IOnlineShopData data;

        public BaseApiController()
            : this(new OnlineShopData(new OnlineShopDbContext()))
        {
        }

        public BaseApiController(IOnlineShopData data)
        {
            this.Data = data;
        }

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
