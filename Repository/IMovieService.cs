using CineMatrix_API.DTOs;

namespace CineMatrix_API.Repository
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDetailsDTO>> GetMovies();
        Task<MovieDetailsDTO> GetMovieById(int id);
        Task AddMovie(MovieCreationDTO movieDto);
        Task UpdateMovie(MovieDetailsDTO movieUpdateDto);
        Task DeleteMovie(int id);
        Task<IEnumerable<MovieDetailsDTO>> GetMoviesByLanguage(string language);
        Task<IEnumerable<MovieDetailsDTO>> GetMoviesByGenre(string genre);
    }
}