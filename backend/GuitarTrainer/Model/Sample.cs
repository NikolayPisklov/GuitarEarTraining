namespace GuitarTrainer.Model
{
    public class Sample
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public int CorrectAnswerId { get; set; }
        public string Path { get; set; } = string.Empty;

        public Exercise Exercise { get; set; } = null!;
        public AnswerOption CorrectAnswer { get; set; } = null!;
    }
}
