namespace GuitarTrainer.Model
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public ICollection<Sample> Samples { get; set; } = new List<Sample>();
        public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
        public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}
