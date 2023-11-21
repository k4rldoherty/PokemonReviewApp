using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _dataContext;
        public ReviewRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool CreateReview(Review review)
        {
            _dataContext.Reviews.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _dataContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> review)
        {
            _dataContext.RemoveRange(review); // RemoveRange for deleting multiple values.
            return Save();
        }

        public Review GetReview(int id)
        {
            return _dataContext.Reviews.Where(r => r.Id == id).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _dataContext.Reviews.OrderBy(r => r.Id).ToList();
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokemon)
        {
            // Given a pokemons ID, retrieve all the reviews of the pokemon with that id.
            return _dataContext.Reviews.Where(p => p.Pokemon.Id == pokemon).ToList();
        }

        public bool ReviewExists(int reviewId)
        {
            return _dataContext.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _dataContext.Update(review);
            return Save();
        }
    }
}
