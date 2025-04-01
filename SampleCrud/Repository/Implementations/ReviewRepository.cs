using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleCrud.Data;
using SampleCrud.Data.Entities;
using SampleCrud.Exceptions;
using SampleCrud.Models;
using SampleCrud.Repository.Interfaces;

namespace SampleCrud.Repository.Implementations
{

    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _dbContext;
        public ReviewRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Review?> AddReview(string username, Guid prodId, AddReviewDto addReviewDto)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u=>u.Username==username)!;
            Product? product = await _dbContext.Products.FindAsync(prodId)!;
            try
            {
                if (user == null || user.Role == "merchant")
                {
                    throw new UserNotFoundException("Customer not found.");
                }
                if (product == null)
                {
                    throw new ProductNotFoundException();
                }

                Review review = new Review()
                {
                    UserId = user.UserId,
                    ProductId = prodId,
                    ReviewType = addReviewDto.ReviewType,
                    ReviewText = addReviewDto.ReviewText
                };

                _dbContext.Reviews.Add(review);
                await _dbContext.SaveChangesAsync();
                return review;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Review>?> GetReviewsByUser(string username)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username)!;

            try
            {
                if (user == null || user.Role == "merchant")
                {
                    throw new UserNotFoundException("Customer not found.");
                }

                List<Review> reviews = await _dbContext.Reviews.Where(p => p.UserId == user.UserId).ToListAsync();

                return reviews;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Review>?> GetReviewsForProduct(Guid prodId)
        {
            Product? product = await _dbContext.Products.FindAsync(prodId)!;
            try
            {
                if (product == null)
                {
                    throw new ProductNotFoundException();
                }
                List<Review> reviews = await _dbContext.Reviews.Where(p => p.ProductId == prodId).ToListAsync();

                return reviews;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteReview(Guid userId, int reviewId)
        {
            List<Review> reviews = await _dbContext.Reviews.Where(u => u.UserId == userId).ToListAsync();
            Review? reviewToDelete = reviews.FirstOrDefault(r => r.reviewId == reviewId);
            try
            {
                if (reviewToDelete == null)
                {
                    throw new ReviewNotFoundException();
                }
                _dbContext.Reviews.Remove(reviewToDelete);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Review?> EditReview(int reviewId, [FromBody] JsonPatchDocument<Review> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                {
                    throw new PatchDocumentNotFoundException();
                }

                Review? reviewToEdit = await _dbContext.Reviews.FindAsync(reviewId);

                if (reviewToEdit == null)
                {
                    throw new ReviewNotFoundException();
                }

                patchDoc.ApplyTo(reviewToEdit);

                await _dbContext.SaveChangesAsync();

                return reviewToEdit;
            }
            catch
            {
                throw;
            }
        }

    }
}
