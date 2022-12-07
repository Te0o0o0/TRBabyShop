﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRBabyShop.Core.Contracts;
using TRBabyShop.Core.Models;
using TRBabyShop.Infrastructure.Data.Common;
using TRBabyShop.Models;
using static TRBabyShop.Infrastructure.Data.Common.Constants;

namespace TRBabyShop.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = Status.RoleAdmin + "," + Status.RoleCustomer)]
    public class ReviewController : Controller
    {
        private readonly IReviewService reviewService;
        private readonly IRepository repo;

        public ReviewController(IReviewService _reviewService, IRepository _repo)
        {
            reviewService = _reviewService;
            repo = _repo;
        }

        [AllowAnonymous]
        public async Task<IActionResult> All(int productId)
        {
            var model = await reviewService.GetProductReviews(productId);

            return View(model);
        }

        
        [HttpGet]
        public IActionResult Add(int productId)
        {
           

            var model = new ReviewViewModel()
            {
                ProductId = productId,
                CreatedOn = DateTime.Now,
                
            };

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Add(ReviewViewModel model)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                await reviewService.AddReview(model, userId);

                return RedirectToAction("All", "Product");
            }
            catch (Exception e)
            {
                var errorMessage = new ErrorViewModel { RequestId = e.Message };
                return View("Error", errorMessage);
            }
        }

        [HttpPost]
        [Authorize(Roles =Status.RoleAdmin)]
        public async Task<IActionResult> Delete(int reviewId)
        {
            await reviewService.DeleteReview(reviewId);

            return RedirectToAction("All", "Product");
        }
    }
}
