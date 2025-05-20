namespace gymvenience_backend.DTOs
{
    public class AvailabilityDto
    {
        public string TrainerId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

}
