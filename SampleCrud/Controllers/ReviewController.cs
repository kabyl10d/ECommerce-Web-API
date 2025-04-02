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

        /// <summary>
        /// Adds a new review for a product.
        /// </summary>
        /// <param name="prodId">The product ID.</param>
        /// <param name="addReviewDto">The review details.</param>
        /// <returns>Returns the added review or an error message.</returns>
        /// <response code="200">Returns the added review.</response>
        /// <response code="400">If the review could not be added.</response>
        /// <response code="401">If the user is unauthorized.</response>
        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> AddReview(Guid prodId, AddReviewDto addReviewDto)
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Unauthorized user.");
            }

            Review? review = await _reviewRepository.AddReview(username, prodId, addReviewDto);
            if (review == null)
            {
                return BadRequest("Can't add review.");
            }
            return Ok(review);
        }

        /// <summary>
        /// Gets reviews by the logged-in user.
        /// </summary>
        /// <returns>Returns the reviews of the user or an error message.</returns>
        /// <response code="200">Returns the reviews of the user.</response>
        /// <response code="401">If the user is unauthorized.</response>
        /// <response code="404">If no reviews are found for the user.</response>
        [Authorize(Roles = "customer")]
        [HttpGet]
        [Route("User")]
        public async Task<IActionResult> GetReviewsByUser()
        {
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Unauthorized user.");
            }
            var reviews = await _reviewRepository.GetReviewsByUser(username);
            if (reviews == null)
            {
                return NotFound("No reviews found for the user.");
            }
            return Ok(reviews);
        }

        /// <summary>
        /// Gets reviews for a specific product.
        /// </summary>
        /// <param name="prodId">The product ID.</param>
        /// <returns>Returns the reviews of the product or an error message.</returns>
        /// <response code="200">Returns the reviews of the product.</response>
        /// <response code="404">If no reviews are found for the product.</response>
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

        /// <summary>
        /// Deletes a review.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <returns>Returns no content or an error message.</returns>
        /// <response code="204">If the review is successfully deleted.</response>
        /// <response code="404">If the review is not found.</response>
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

        /// <summary>
        /// Edits a review.
        /// </summary>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="patchDoc">The patch document containing the changes.</param>
        /// <returns>Returns the edited review or an error message.</returns>
        /// <response code="200">Returns the edited review.</response>
        /// <response code="404">If the review is not found.</response>
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
