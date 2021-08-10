using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Execution;
using ZombieGame.Business.DTOs.Request;
using ZombieGame.Business.Game.Planets;
using ZombieGame.Business.Game.Projectiles;

namespace ZombieGame.Business.Game
{
    public class GameRoom
    {
        public string Id { get; }
        public int PlayerCount { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        
        public int MoveSpeed { get; set; }

        public Dictionary<string, Player> Players { get; set; }
        public List<Planet> Planets { get; set; }
        
        public ConcurrentBag<Bullet> Bullets { get; set; }
        public string Name { get; set;}
        
        private Timer PositionInterval { get; set; }
        
        private Timer BulletInterval { get; set; }
        
        private Timer PlanetInterval { get; set; }

        public int Tick { get; set; }

        public GameRoom(string name,int playerCount, int size, int tick)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            PlayerCount = playerCount;
            SizeX = size * 1000;
            SizeY = size * 1000;
            MoveSpeed = 10;
            Players = new Dictionary<string, Player>();
            Planets = new List<Planet>();
            Bullets = new ConcurrentBag<Bullet>();
            Tick = tick;
        }
        
        private void PositionLoop(object? state)
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
                if (player.PosX > SizeX || player.PosX < 0 || player.PosY > SizeY ||
                    player.PosY < 0)
                {
                    player.PosX = posX;
                    player.PosY = posY;
                }
            }
        }

        private void BulletLoop(object? state)
        {
            Parallel.ForEach(Bullets, bullet =>
            {
                switch (bullet.Towards)
                {
                    case Towards.UP:
                        bullet.PosY -= bullet.Speed;
                        break;
                    case Towards.DOWN:
                        bullet.PosY +=  bullet.Speed;
                        break;
                    case Towards.RIGHT:
                        bullet.PosX +=  bullet.Speed;
                        break;
                    case Towards.LEFT:
                        bullet.PosX -=  bullet.Speed;
                        break;
                }
                if (bullet.PosX > SizeX || bullet.PosX < 0 || bullet.PosY > SizeY ||
                    bullet.PosY < 0)
                {
                    Bullets.TryTake(out bullet);
                }
                
            });
        }

        private void PlanetLoop(object? state)
        {
            
        }

        public void StartThreads()
        {
            PositionInterval = new Timer(PositionLoop, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000 / Tick));
            BulletInterval = new Timer(BulletLoop, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000 / Tick));
            PlanetInterval = new Timer(PlanetLoop, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        public void StopThreads()
        {
            PositionInterval = null;
            BulletInterval = null;
            PlanetInterval = null;
        }
    }
}