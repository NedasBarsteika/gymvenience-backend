using gymvenience_backend.Models;

namespace gymvenience.Models
{
    public class TrainerAvailability
    {
        public string Id {  get; set; }
        public string TrainerId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public User Trainer { get; set; }
        public bool Reserved { get; set; } = false;
    }
}
