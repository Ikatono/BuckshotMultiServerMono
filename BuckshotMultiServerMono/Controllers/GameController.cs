using BuckshotMultiServerMono.Objects;
using BuckshotMultiServerMono.Serializable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class GameController : ControllerBase
    {
        //int counter = 0;
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[Route("/count")]
        //public ActionResult<int> Count()
        //{
        //    return new ActionResult<int>(counter++);
        //}
        [HttpPost]
        [Route("/create")]
        public IActionResult CreateGame()
        {
            var game = GameContainer.Get().CreateGame();
            var result = CreatedAtAction(nameof(CreateGame), null);
            ((IList<string?>)Response.Headers.Location).Add(game.Id);
            return result;
        }
        //TODO move name to payload?
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ErrorMessage>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Route("/game/{id}/player/join/")]
        public IActionResult JoinGame(string id, [FromBody] NewPlayer newPlayer)
        {
            var name = newPlayer.Name;
            if (string.IsNullOrEmpty(name))
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (!GameContainer.ValidateName(name))
            {
                var error = new ErrorMessage();
                this.Response.StatusCode = StatusCodes.Status400BadRequest;
                return new JsonResult(error);
            }
            var game = GameContainer.Get().FindGame(id);
            if (game is null)
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            lock (game)
            {
                //Game.AddPlayer already avoids creating duplicate player,
                //but won't tell you whether the player was a duplicate or
                //the game was full
                if (game.Player1 is Player p1)
                {
                    if (p1.Name == name)
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                if (game.Player2 is Player p2)
                {
                    if (p2.Name == name)
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
                return game.AddPlayer(name) ?
                    new StatusCodeResult(StatusCodes.Status201Created) :
                    new StatusCodeResult(StatusCodes.Status409Conflict);
            }


        }
        [Route("/game/{id}/player/ws")]
        public async Task Connect(string id, [FromQuery(Name = "PlayerName")] string? name)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                if (name is null)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
                var game = GameContainer.Get().FindGame(id);
                if (game is null)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                var player = game.FindPlayer(name);
                if (player is null)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                player.AddConnection(socket);
                using ManualResetEventSlim mre = new();
                player.ConnectionClosedEvent += (_, _) => mre.Set();
                //prevent race condition where connection closes before event is registered
                if (!player.Connected())
                {
                    //make sure connection is cleaned up properly
                    player.Disconnect();
                    return;
                }
                //holds pipeline open for the duration of the connection
                mre.Wait();
                player.Disconnect();
                return;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
