namespace gymvenience_backend.DTOs
{
    public class TrainerSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public float Rating { get; set; }
        public string GymName { get; set; }
        public string GymAddress { get; set; }
    }
}
