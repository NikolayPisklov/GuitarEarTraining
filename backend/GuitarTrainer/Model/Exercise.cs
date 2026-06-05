namespace GuitarTrainer.Model
{
    public class Exercise
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Title { get; set; } = string.Empty;

        public Type Type { get; set; } = null!;
        public ICollection<Sample> Samples { get; set; } = new List<Sample>();
        public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
    }
}
