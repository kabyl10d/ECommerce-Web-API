using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleCrud.Data.Entities;
using SampleCrud.Models;

namespace SampleCrud.Repository.Interfaces
{
    public interface IReviewRepository
    {
        public Task<Review?> AddReview(string username, Guid prodId, AddReviewDto addReviewDto);
        public Task<List<Review>?> GetReviewsByUser(string username);
        public Task<List<Review>?> GetReviewsForProduct(Guid prodId);
        public Task<bool> DeleteReview(Guid userId, int reviewId);
        public Task<Review?> EditReview(int reviewId, [FromBody] JsonPatchDocument<Review> patchDoc);
    }
}
