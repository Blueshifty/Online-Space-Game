using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using SpaceGame.Business.Game.Consumables;
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
        
        public ConcurrentDictionary<int, Bullet> Bullets { get; set; }

        public List<ConcurrentDictionary<int, Consumable>> Consumables { get; set; }
        
        public List<ConcurrentDictionary<int, Planet>> Planets { get; set; }

        private readonly Thread _positionThread;

        private readonly Thread _bulletThread;

        private readonly Thread _planetThread;
        
        public int Tick { get; set; }

        public bool Looping { get; set; } = true;

        public GameRoom(string name, int playerCount, int size, int tick)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            PlayerCount = playerCount;
            SizeX = size * 1000;
            SizeY = size * 1000;
            Tick = tick;
            
            Players = new Dictionary<string, Player>();
            
            Planets = new List<ConcurrentDictionary<int, Planet>>();
            
            Planets.Add(new ConcurrentDictionary<int, Planet>()); // Jupiter
            
            Planets.Add(new ConcurrentDictionary<int, Planet>()); // Saturn
            
            Planets.Add(new ConcurrentDictionary<int, Planet>()); // Mars    
            
            Planets.Add(new ConcurrentDictionary<int, Planet>()); // Venus

            Consumables = new List<ConcurrentDictionary<int, Consumable>>();
            
            Consumables.Add(new ConcurrentDictionary<int, Consumable>()); // Stardust
            
            Consumables.Add(new ConcurrentDictionary<int, Consumable>()); // Health
            
            Consumables.Add(new ConcurrentDictionary<int, Consumable>()); // Ammo
            
            Bullets = new ConcurrentDictionary<int, Bullet>();
            
            _positionThread = new Thread(PositionLoop);
            
            _bulletThread = new Thread(BulletLoop);
            
            _planetThread = new Thread(PlanetLoop);
        }

        private void PositionLoop()
        {
            while (true)
            {
                if (Looping)
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

                        foreach (var dict in Consumables)
                        {
                            var collidedConsumable = dict.Values.FirstOrDefault(c =>
                                c.PosX <= player.PosX && c.PosX + 15 >= player.PosX && c.PosY <= player.PosY &&
                                c.PosY + 15 >= player.PosY);

                            if (collidedConsumable != null)
                            {
                                collidedConsumable.Consume(player);
                                dict.TryRemove(collidedConsumable.GetHashCode(), out collidedConsumable);
                            }
                        }
                    }
                }
                Thread.Sleep(1000 / Tick);
            }
        }

        private void BulletLoop()
        {
            while (true)
            {
                if (Looping)
                {
                    Parallel.ForEach(Bullets, bullet =>
                    {
                        var removeBullet = false;
                        
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
                        
                        foreach (var planetDict in Planets)
                        {
                            var collidedPlanet = planetDict.Values.FirstOrDefault(p =>
                                (p.PosX <= bullet.Value.PosX && p.PosX + 20 >= bullet.Value.PosX) &&
                                (p.PosY <= bullet.Value.PosY && p.PosY + 20 >= bullet.Value.PosY));
                                
                            if (collidedPlanet != null)
                            {
                                collidedPlanet.Health -= bullet.Value.Damage;
                                if (collidedPlanet.Health <= 0)
                                {
                                    foreach (var consumable in collidedPlanet.Consumables)
                                    {
                                        Consumables[consumable.ConsumableIndex].TryAdd(consumable.GetHashCode(), consumable);
                                    }
                                    Planets.First().TryRemove(collidedPlanet.GetHashCode(), out collidedPlanet);
                                }
                                removeBullet = true;
                                break;
                            }
                        }
                        
                        if(!removeBullet)
                        {
                            foreach (var pair in Players)
                            {
                                if (pair.Key != bullet.Value.ShooterId && (pair.Value.PosX <= bullet.Value.PosX && pair.Value.PosX + 10 >= bullet.Value.PosX) &&
                                    (pair.Value.PosY <= bullet.Value.PosY && pair.Value.PosY + 10 >= bullet.Value.PosY))
                                {
                                    pair.Value.Health -= bullet.Value.Damage;
                                    removeBullet = true;
                                    break;
                                }
                            }

                            if (!removeBullet && bullet.Value.PosX > SizeX || bullet.Value.PosX < 0 || bullet.Value.PosY > SizeY ||
                                bullet.Value.PosY < 0)
                            {
                                removeBullet = true;
                            }
                        }
                        
                        if (removeBullet)
                        {
                            Bullets.TryRemove(bullet.Value.GetHashCode(), out var removedBullet);
                        }
                    });
                }
                Thread.Sleep(1000 / Tick);
            }
        }

        private void PlanetLoop()
        {
            while (true)
            {
                if (Looping)
                {
                    var jupiterCount = Planets[0].Values.Count(p => p.GetType() == typeof(Jupiter));

                    var spawnJupiter = Jupiter.Count - jupiterCount;

                    for (var i = 0; i < spawnJupiter; ++i)
                    {
                        var jupiter = new Jupiter(RandomNumberGenerator.GetInt32(0, SizeX),
                            RandomNumberGenerator.GetInt32(0, SizeY));
                        Planets[0].TryAdd(jupiter.GetHashCode(), jupiter);
                    }
                }
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
            Looping = false;
        }

        public void ClearResources()
        {
            Bullets.Clear();
        }
        
    }
}