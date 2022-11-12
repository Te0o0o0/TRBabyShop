﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TRBabyShop.Core.Contracts;
using TRBabyShop.Core.Models;
using TRBabyShop.Infrastructure.Data.Common;
using TRBabyShop.Infrastructure.Data.Models;
using TRBabyShop.Models;

namespace TRBabyShop.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService reviewService;
        private readonly IRepository repo;

        public ReviewController(IReviewService _reviewService, IRepository _repo)
        {
            reviewService = _reviewService;
            repo = _repo;
        }

        public async Task<IActionResult> All(int productId)
        {
            var model = await reviewService.GetProductReviews(productId);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Add(int productId)
        {
            var model = new ReviewViewModel()
            {
               
                ProductId = productId,
                CreatedOn = DateTime.Now
                
            };

            return View(model);

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(ReviewViewModel model)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                await reviewService.AddReview(model, userId);

                return RedirectToAction("All", "Review");
            }
            catch (Exception e)
            {
                var errorMessage = new ErrorViewModel { RequestId = e.Message };
                return View("Error", errorMessage);
            }
        }

        public async Task<IActionResult> Delete(int reviewId)
        {
            var review = await reviewService.DeleteReview(reviewId);

            return RedirectToAction("All", "Product");
        }
    }
}
