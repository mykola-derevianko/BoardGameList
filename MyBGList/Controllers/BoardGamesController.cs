using Microsoft.AspNetCore.Mvc;
using MyBGList.Models;
using MyBGList.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations;
using MyBGList.Attributes;
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
                [Range(1, 100)] int pageSize = 10,
                [SortColumnValidator(typeof(BoardGameDTO))] string? sortColumn = "Name",
                [SortOrderValidator] string? sortOrder = "ASC",
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


        [HttpPost(Name = "GetBoardGames")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Post(BoardGameDTO model) 
        {
            var boardGame = await _context.BoardGames
                .Where(bg => bg.Id == model.Id)
                .FirstOrDefaultAsync();

            if(boardGame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                    boardGame.Name = model.Name;
                if (model.Year.HasValue && model.Year > 0)
                    boardGame.Year = model.Year.Value;
                boardGame.LastModifiedDate = DateTime.UtcNow;
                _context.BoardGames.Update(boardGame);
                await _context.SaveChangesAsync();
            }
            return new RestDTO<BoardGame?>()
            {
                Data = boardGame,
                Links = new List<LinkDTO>{
                    new LinkDTO(
                        href: $"{Request.Scheme}://{Request.Host}/BoardGames?{model}",
                        rel: "self",
                        type: "POST"
                    )
                }
            };
        }

        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Delete( int id)
        {
            var boardGame = await _context.BoardGames
                .FirstOrDefaultAsync(bg => bg.Id == id);
            if(boardGame != null)
            {
                _context.BoardGames.Remove(boardGame);
                await _context.SaveChangesAsync();
            }
            return new RestDTO<BoardGame?>()
            {
                Data = boardGame,
                Links = new List<LinkDTO>{
                    new LinkDTO(
                        href: $"{Request.Scheme}://{Request.Host}/BoardGames?{id}",
                        rel: "self",
                        type: "DELETE"
                    )
                }
            };
        }
    }
}