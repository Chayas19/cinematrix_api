using CinematrixAPI.Enums;

namespace CineMatrix_API.DTOs
{
    public class SubscriptionDTO
    {
        public int Id { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
