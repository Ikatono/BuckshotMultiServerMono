using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuckshotMultiServerMono.Objects
{
    /// <summary>
    /// Holds the state of the game, including player state
    /// </summary>
    public class GameState
    {
        public int Player1Score;
        public int Player2Score;
        public int RoundNumber;
        public int Player1Health;
        public int Player2Health;
        public HandcuffState CurrentHandcuffState = HandcuffState.None;
        public bool SawUsed = false;
        public PlayerInventory Player1Inventory = new();
        public PlayerInventory Player2Inventory = new();
        public bool ActivePlayerIs1 = true;

        /// <summary>
        /// called after shooting (other than shooting self with blank)
        /// automatically handles handcuffs
        /// active player might not change, do not make any assumptions
        /// about the state after callaing this
        /// </summary>
        public void NextTurn()
        {
            if (CurrentHandcuffState == HandcuffState.Active)
            {
                CurrentHandcuffState = HandcuffState.Inactive;
            }
            else
            {
                if (CurrentHandcuffState == HandcuffState.Inactive)
                    CurrentHandcuffState = HandcuffState.None;
                ActivePlayerIs1 = !ActivePlayerIs1;
            }
        }
    }
}
