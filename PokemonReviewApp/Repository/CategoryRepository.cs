using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context) 
        {
            _context = context;
        }

        public bool CategoryExists(int id)
        {
            // .Any() returns a bool for whatever is inside the brackets
            return _context.Categories.Any(c => c.Id == id);
        }

        public bool CreateCategory(Category category)
        {
            // Change Tracker
            // Add, Updating, Modifying
            // Connected OR Disconnected
            _context.Categories.Add(category);

            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _context.Remove(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(p => p.Id).ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonByCategory(int catId)
        {
            return _context.PokemonCategories
                .Where(e => e.CategoryId == catId)
                .Select(p => p.Pokemon)
                .ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges(); // When the SQL code is generated and sent to the database.
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _context.Update(category);
            return Save();
        }
    }
}
