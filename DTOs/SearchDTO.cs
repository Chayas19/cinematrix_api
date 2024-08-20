namespace CineMatrix_API.DTOs
{
    public class SearchDTO
    {

        public string? LanguageName { get; set; }
        public int? GenreId { get; set; }
        public string? ActorName { get; set; }
        public PaginationDTO Pagination { get; set; }


    }
}
