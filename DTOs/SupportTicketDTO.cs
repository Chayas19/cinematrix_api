namespace CineMatrix_API.DTOs
{
    public class SupportTicketDTO

    {

        public string UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
        public string AssignedTo { get; set; }
    }
}
