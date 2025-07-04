using Microsoft.AspNetCore.Mvc;
using MyBGList.Models;
namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public IEnumerable<BoardGame> Get()
        {
            _logger.LogInformation("Getting all board games");
            return new List<BoardGame>
            {
                new BoardGame
                {
                    Id = 1,
                    Name = "Catan",
                    Year = 1995,
                    MinPlayers = 3,
                    MaxPlayers = 4
                },
                new BoardGame
                {
                    Id = 2,
                    Name = "Carcassonne",
                    Year = 2000,
                    MinPlayers = 2,
                    MaxPlayers = 5
                },
                new BoardGame
                {
                    Id = 3,
                    Name = "Ticket to Ride",
                    Year = 2004,
                    MinPlayers = 2,
                    MaxPlayers = 5
                }
            };
        }
    }
}