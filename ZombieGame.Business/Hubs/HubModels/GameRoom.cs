using System;
using System.Collections.Generic;

namespace ZombieGame.Business.Hubs.HubModels
{
    public class GameRoom
    {
        public string Id { get; }
        public int PlayerCount { get; set; }
        public int CurrentPlayerCount { get; set; }

        public Dictionary<string, Player> Players { get; set; }
        public string Name { get; set;}

        public int[,] Map { get; set; }
        
        public GameRoom(string name,int playerCount, int size)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            PlayerCount = playerCount;
            Map = new int[size * 100, size * 100];
            CurrentPlayerCount = 0;
            Players = new Dictionary<string, Player>();
        }
    }
}