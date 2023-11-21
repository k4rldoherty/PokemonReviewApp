namespace PokemonReviewApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public int Rating { get; set; }
        
        // Each Review will have one reviewer who posts the review, and a Pokemon the review is about.
        public Reviewer Reviewer { get; set; }
        public Pokemon Pokemon { get; set; }
    }
}
