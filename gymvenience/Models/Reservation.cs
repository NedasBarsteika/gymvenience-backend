namespace gymvenience.Models
{
    public class Reservation
    {
        public string Id { get; set; }  // Unique ID for the reservation
        public string UserId { get; set; }  // The user who made the reservation
        public DateTime Date { get; set; }  // The date of the reservation
        public TimeSpan Time { get; set; }  // The starting time (HH:mm)
        public TimeSpan Duration { get; set; }  // Duration of the session
        public string TrainerId { get; set; }  // Trainer's userId
        public string GymId { get; set; }  // Gym address
        public bool IsDone { get; set; } = false; // Is reservation already done flag
        public decimal RateAtBooking { get; set; } = 0m;
        public string StripeSessionId { get; set; } = string.Empty;
    }
}
