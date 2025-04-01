using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleCrud.Data.Entities;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;

namespace SampleCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> AddReview(Guid prodId, AddReviewDto addReviewDto)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;

            Review? review = await _reviewRepository.AddReview(username, prodId, addReviewDto);
            if (review == null)
            {
                return BadRequest("Can't add review.");
            }
            return Ok(review);
        }

        [Authorize(Roles = "customer")]
        [HttpGet]  
        public async Task<IActionResult> GetReviewsByUser()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            var reviews = await _reviewRepository.GetReviewsByUser(username);
            if (reviews == null)
            {
                return NotFound("No reviews found for the user.");
            }
            return Ok(reviews);
        }

        [Authorize(Roles = "customer,merchant")]
        [HttpGet]
        public async Task<IActionResult> GetReviewsForProduct(Guid prodId)
        {
            var reviews = await _reviewRepository.GetReviewsForProduct(prodId);
            if (reviews == null)
            {
                return NotFound("No reviews found for the product.");
            }
            return Ok(reviews);
        }

        [Authorize(Roles = "customer")]
        [HttpDelete]
        public async Task<IActionResult> DeleteReview(Guid userId, int reviewId)
        {
            bool isDeleted = await _reviewRepository.DeleteReview(userId, reviewId);
            if (!isDeleted)
            {
                return NotFound("Review not found.");
            }
            return NoContent();
        }

        [Authorize(Roles = "customer")]
        [HttpPatch]
        public async Task<IActionResult> EditReview(int reviewId, [FromBody] JsonPatchDocument<Review> patchDoc)
        {
            var review = await _reviewRepository.EditReview(reviewId, patchDoc);
            if (review == null)
            {
                return NotFound("Review not found.");
            }
            return Ok(review);
        }
    }
}
