using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    public class GameContainer
    {
        private readonly List<Game> Games = [];
        public Game CreateGame()
        {
            while (true)
            {
                var id = GenerateId();
                lock(Games)
                {
                    if (!Games.Any(g => g.Id == id))
                    {
                        Game game = new(id);
                        Games.Add(game);
                        return game;
                    }
                }
            }
        }
        public bool EndGame(string id)
        {
            lock (Games)
            {
                Game? game = Games.FirstOrDefault(g => g.Id == id);
                if (game is Game game_notnull)
                {
                    Games.Remove(game_notnull);
                    return true;
                }
                else
                    return false;
            }
        }
        //generates an id which must be valid, but not necessarily unique
        protected static string GenerateId()
        {
            throw new NotImplementedException();
        }
        public static bool ValidateName(string name)
        {
            throw new NotImplementedException();
        }
        public static GameContainer Get()
        {
            throw new NotImplementedException();
        }
        public Game? FindGame(string id)
        {
            lock (Games)
            {
                return Games.FirstOrDefault(g => g.Id == id);
            }
        }
    }
}
