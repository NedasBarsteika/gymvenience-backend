using gymvenience_backend.Models;

namespace gymvenience.Models
{
    public class TrainerAvailability
    {
        public string Id { get; set; }
        public string TrainerId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Reserved { get; set; } = false;
        public string GymId { get; set; }

        public TrainerAvailability()
        {
            Id = Guid.NewGuid().ToString();
        }

        public TrainerAvailability(string id, string trainerId, DateTime date, TimeSpan startTime, TimeSpan duration, bool reserved, string gymId)
        {
            Id = id;
            TrainerId = trainerId;
            Date = date;
            StartTime = startTime;
            Duration = duration;
            Reserved = reserved;
            GymId = gymId;
        }
    }
}
