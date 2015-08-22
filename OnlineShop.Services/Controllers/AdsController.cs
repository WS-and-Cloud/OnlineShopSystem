namespace OnlineShop.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;

    using OnlineShop.Models;
    using OnlineShop.Services.Models;

    [Authorize]
    [RoutePrefix("api/ads")]
    public class AdsController : BaseApiController
    {
        // GET api/ads
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetAllAds()
        {
            var ads = this.Data.Ads.All().Where(a=> a.Status != AdStatus.Closed).Select(AllAdsViewModel.Create);

            return this.Ok(ads);
        }

        // POST api/ads
        [HttpPost]
        public IHttpActionResult CreateAd(CreateAdBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUserId = this.User.Identity.GetUserId();
            var ad = new Ad
                     {
                         Name = model.Name,
                         Description = model.Description,
                         Price = model.Price,
                         PostedOn = DateTime.Now,
                         Status = AdStatus.Open,
                         TypeId = model.TypeId,
                         OwnerId = currentUserId
                     };
            if (model.Categories == null)
            {
                return this.BadRequest("You need to provide at least 1 category id.");
            }

            foreach (var categoryId in model.Categories)
            {
                var category = this.Data.Categories.Find(categoryId);
                if (category == null)
                {
                    return this.BadRequest("Invalid category id.");
                }

                ad.Categories.Add(category);
            }

            this.Data.Ads.Add(ad);
            this.Data.SaveChanges();

            var result =
                this.Data.Ads.All().Where(a => a.Id == ad.Id).Select(AllAdsViewModel.Create).FirstOrDefault();
            
            return this.Ok(result);
        }

        // PUT api/ads/{id}/close
        [HttpPut]
        [Route("{id}/close")]
        public IHttpActionResult CloseAd(int id)
        {
            var ad = this.Data.Ads.All().FirstOrDefault(a => a.Id == id);
            if (ad == null)
            {
                return this.NotFound();
            }

            var userId = this.User.Identity.GetUserId();
            if (ad.OwnerId != userId)
            {
                return this.BadRequest("You are not owner of this ad!");
            }

            if (ad.Status == AdStatus.Closed)
            {
                return this.BadRequest("This ad is already closed.");
            }

            ad.Status = AdStatus.Closed;
            this.Data.SaveChanges();
            return this.Ok(string.Format("Ad successfully closed {0}", ad.Name));
        }
    }
}