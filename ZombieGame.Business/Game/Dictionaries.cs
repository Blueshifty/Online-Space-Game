using System.Collections.Generic;
using System.Threading;

namespace ZombieGame.Business.Game
{
    public class Dictionaries
    {
        public static readonly Dictionary<string, GameRoom> GameRooms = new();
        
        public static readonly Dictionary<string, string> PlayerRoomMap  = new();
    }
}