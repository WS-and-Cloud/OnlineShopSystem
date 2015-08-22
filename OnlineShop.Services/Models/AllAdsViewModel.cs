namespace OnlineShop.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using OnlineShop.Models;

    public class AllAdsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public DateTime PostedOn { get; set; }

        public DateTime? ClosedOn { get; set; }

        public AdTypeViewModel AdType { get; set; }

        public UserViewModel Owner { get; set; }

        public IEnumerable<CategoriesViewModel> Categories { get; set; }


        public static Expression<Func<Ad, AllAdsViewModel>> Create
        {
            get
            {
                return
                    a =>
                    new AllAdsViewModel()
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        Price = a.Price,
                        Owner = new UserViewModel { Id = a.Owner.Id, Username = a.Owner.UserName },
                        PostedOn = a.PostedOn,
                        AdType = new AdTypeViewModel() { AdType = a.Type.Name },
                        Categories =
                            a.Categories.Select(
                                c => new CategoriesViewModel() { Id = c.Id, CategoryName = c.Name })
                    };
            }
        } 
    }
}