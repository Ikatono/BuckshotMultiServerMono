using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    public class Game
    {
        public string Id;
        public Player? Player1;
        public Player? Player2;
        public GameState State;

        public Game(string id)
        {
            Id = id;
            State = new GameState();
        }
        //return true if player added or already exists, false if name doesn't
        //exist and players are full
        //not thread-safe, must be locked externally
        public bool AddPlayer(string name)
        {
            if (Player1 is Player p1)
            {
                if (p1.Name == name)
                    return true;
            }
            if (Player2 is Player p2)
            {
                if (p2.Name == name)
                    return true;
            }
            if (Player1 is null)
            {
                Player1 = new Player(name);
                return true;
            }
            else if (Player2 is null)
            {
                Player2 = new Player(name);
                return true;
            }
            else
                return false;
        }

        internal Player? FindPlayer(string name)
        {
            throw new NotImplementedException();
        }
    }
}
