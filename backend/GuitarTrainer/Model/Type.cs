namespace GuitarTrainer.Model
{
    public class Type
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
