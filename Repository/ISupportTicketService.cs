using CineMatrix_API.DTOs;
using CineMatrix_API.Models;

namespace CineMatrix_API.Repository
{
    public interface ISupportTicketService
    {
        Task<SupportTicket> CreateTicketAsync(CreateSupoortTicketDTO dto);
        Task<SupportTicket> GetTicketByIdAsync(int id);
        Task<IEnumerable<SupportTicket>> GetAllTicketsAsync();
        Task UpdateTicketAsync(int id, UpdateSupportTicketDTO dto);

        Task<List<SupportTicket>> GetTicketsByUserIdAsync(string userId);

    }
}
