using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int id);
        ICollection<Review> GetReviewsOfAPokemon(int pokemon);
        bool ReviewExists(int reviewId);

        bool CreateReview(Review review);
        bool Save();
        bool UpdateReview(Review review);

        bool DeleteReview(Review review);
        bool DeleteReviews(List<Review> review);
    }
}
