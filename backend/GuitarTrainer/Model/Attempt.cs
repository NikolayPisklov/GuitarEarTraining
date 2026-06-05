namespace GuitarTrainer.Model
{
    public class Attempt
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ExerciseId { get; set; }
        public DateTime AttemptDate { get; set; }
        public double Score { get; set; }

        public Exercise Exercise { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
