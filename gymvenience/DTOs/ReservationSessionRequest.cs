namespace gymvenience_backend.DTOs
{
    public class ReservationSessionRequest
    {
        public string TrainerId { get; set; }
        public string SlotId    { get; set; }
        public string Date      { get; set; }    // "YYYY-MM-DD"
        public string StartTime { get; set; }    // "HH:mm:ss"
        public string Duration  { get; set; }    // "HH:mm"
        public string Origin    { get; set; }    // frontend origin URL
    }
}