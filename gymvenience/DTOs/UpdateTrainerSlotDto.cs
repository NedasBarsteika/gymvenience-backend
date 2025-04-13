namespace gymvenience_backend.DTOs
{
    public class UpdateTrainerSlotDto
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
