using Microsoft.AspNetCore.Mvc;
using MyBGList.Models;
using MyBGList.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;
        private readonly ApplicationDbContext _context;

        public BoardGamesController(ApplicationDbContext context, ILogger<BoardGamesController> logger)
        {
            _context = context;
            _logger = logger;

        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<BoardGame[]>> Get(
                int pageIndex = 0,
                int pageSize = 10,
                string? sortColumn = "Name",
                string? sortOrder = "ASC",
                string? filterQuery = null
            )
        {
            var query = _context.BoardGames.AsQueryable();
            if (!string.IsNullOrEmpty(filterQuery))
                query = query.Where(bg => bg.Name.Contains(filterQuery));
            var recordCount = await query.CountAsync();
            query = query
                    .OrderBy($"{ sortColumn} { sortOrder}")
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
            _logger.LogInformation("Getting all board games");
            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                RecordCount = recordCount,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                        href: $"{Request.Scheme}://{Request.Host}/BoardGames?pageIndex={pageIndex}&pageSize={pageSize}",
                        rel: "self",
                        type: "GET")
                }
            };
        }
    }
}