using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _dataContext;
        public ReviewerRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _dataContext.Add(reviewer);
            return Save();

        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _dataContext.Remove(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int id)
        {
            return _dataContext.Reviewers.Where(r => r.Id == id).Include(e => e.Reviews).SingleOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _dataContext.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _dataContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _dataContext.Reviewers.Any(r => r.Id == reviewerId);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _dataContext.Update(reviewer);
            return Save();
        }
    }
}
