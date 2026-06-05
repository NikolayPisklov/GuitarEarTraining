namespace GuitarTrainer.Model
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string Code { get; set; } = string.Empty;

        public Exercise Exercise { get; set; } = null!;
        public ICollection<Sample> Samples { get; set; } = new List<Sample>();
    }
}
