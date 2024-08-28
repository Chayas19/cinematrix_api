//using CineMatrix_API.DTOs;
//using CineMatrix_API.Models;
//using CineMatrix_API.Repository;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CineMatrix_API.Controllers
//{
//    [ApiController]
//    [Route("api/support-tickets")]
//    [Authorize]
//    public class SupportTicketController : ControllerBase
//    {
//        private readonly ISupportTicketService _supportTicketService;

//        public SupportTicketController(ISupportTicketService supportTicketService)
//        {
//            _supportTicketService = supportTicketService;
//        }

//        [HttpPost]
//        [Authorize(Roles = "Guest, PrimeUser")]
//        public async Task<ActionResult> CreateTicket([FromBody] CreateSupoortTicketDTO dto)
//        {
//            await _supportTicketService.CreateTicketAsync(dto);
//            return Ok(new { message = "Support ticket created successfully." });
//        }

//        [HttpGet("{ticketId}")]
//        [Authorize(Roles = "Admin, Support")]
//        public async Task<ActionResult<SupportTicket>> GetTicketById(int ticketId)
//        {
//            var ticket = await _supportTicketService.GetTicketByIdAsync(ticketId);
//            if (ticket == null)
//            {
//                return NotFound();
//            }
//            return Ok(ticket);
//        }

//        [HttpGet("user/{userId}")]
//        [Authorize(Roles = "Guest, PrimeUser")]
//        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetTicketsByUserId(string userId)
//        {
//            var tickets = await _supportTicketService.GetTicketsByUserIdAsync(userId);
//            return Ok(tickets);
//        }

//        [HttpPut("{ticketId}")]
//        [Authorize(Roles = "Admin, Support")]
//        public async Task<ActionResult> UpdateTicket(int ticketId, [FromBody] UpdateSupportTicketDTO dto)
//        {
//            await _supportTicketService.UpdateTicketAsync(ticketId, dto);
//            return Ok(new { message = "Support ticket updated successfully." });
//        }

//        [HttpGet]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetAllTickets()
//        {
//            var tickets = await _supportTicketService.GetAllTicketsAsync();
//            return Ok(tickets);
//        }
//    }
//}
