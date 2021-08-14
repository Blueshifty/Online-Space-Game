using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using SpaceGame.Business.Game.Planets;
using SpaceGame.Business.Game.Projectiles;

namespace SpaceGame.Business.Game
{
    public class GameRoom
    {
        public string Id { get; }
        public int PlayerCount { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string Name { get; set; }
        public Dictionary<string, Player> Players { get; set; }
        public ConcurrentDictionary<int, Bullet> BulletsDict { get; set; }
        
        public ConcurrentDictionary<int, Planet> PlanetsDict { get; set; }
        
        private readonly Thread _positionThread;

        private readonly Thread _bulletThread;

        private readonly Thread _planetThread;
        
        public int Tick { get; set; }

        public GameRoom(string name, int playerCount, int size, int tick)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            PlayerCount = playerCount;
            SizeX = size * 1000;
            SizeY = size * 1000;
            Tick = tick;
            Players = new Dictionary<string, Player>();
            PlanetsDict = new ConcurrentDictionary<int, Planet>();
            BulletsDict = new ConcurrentDictionary<int, Bullet>();
            _positionThread = new Thread(PositionLoop);
            _bulletThread = new Thread(BulletLoop);
            _planetThread = new Thread(PlanetLoop);
        }

        private void PositionLoop()
        {
            while (true)
            {
                foreach (var player in Players.Values.Where(p => p.Towards != Towards.NOWHERE))
                {
                    var posX = player.PosX;

                    var posY = player.PosY;

                    switch (player.Towards)
                    {
                        case Towards.UP:
                            player.PosY -= player.Speed;
                            break;
                        case Towards.DOWN:
                            player.PosY += player.Speed;
                            break;
                        case Towards.RIGHT:
                            player.PosX += player.Speed;
                            break;
                        case Towards.LEFT:
                            player.PosX -= player.Speed;
                            break;
                    }

                    if (player.PosX > SizeX || player.PosX < 0 || player.PosY > SizeY || player.PosY < 0)
                    {
                        player.PosX = posX;
                        player.PosY = posY;
                    }
                }
                Thread.Sleep(1000 / Tick);
            }
        }

        private void BulletLoop()
        {
            while (true)
            {
                Parallel.ForEach(BulletsDict, bullet =>
                {
                    switch (bullet.Value.Towards)
                    {
                        case Towards.UP:
                            bullet.Value.PosY -= bullet.Value.Speed;
                            break;
                        case Towards.DOWN:
                            bullet.Value.PosY += bullet.Value.Speed;
                            break;
                        case Towards.RIGHT:
                            bullet.Value.PosX += bullet.Value.Speed;
                            break;
                        case Towards.LEFT:
                            bullet.Value.PosX -= bullet.Value.Speed;
                            break;
                    }
                    
                    if (bullet.Value.PosX > SizeX || bullet.Value.PosX < 0 || bullet.Value.PosY > SizeY || bullet.Value.PosY < 0)
                    {
                        BulletsDict.TryRemove(bullet.Value.GetHashCode(), out var bulletRemoved);
                    }
                });
                Thread.Sleep(1000 / Tick);
            }
        }

        private void PlanetLoop()
        {
            while (true)
            {
                Thread.Sleep(30 * 1000);
            }
        }

        public void StartThreads()
        {
            _positionThread.Start();
            _bulletThread.Start();
            _planetThread.Start();
        }

        public void StopThreads()
        {
            _positionThread.Interrupt();
            _bulletThread.Interrupt();
            _planetThread.Interrupt();
        }
    }
}