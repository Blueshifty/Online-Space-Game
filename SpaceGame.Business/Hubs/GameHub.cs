using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SpaceGame.Business.Application;
using SpaceGame.Business.DTOs.Event;
using SpaceGame.Business.DTOs.Request;
using SpaceGame.Business.DTOs.Response;
using SpaceGame.Business.Game;
using SpaceGame.Business.Game.Projectiles;
using SpaceGame.Business.Utilities.Mapper;
using static SpaceGame.Business.Game.Dictionaries;

namespace SpaceGame.Business.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMapper _mapper;

        private static int Tick = 200;
        
        //private static readonly Dictionary<string, Timer> RoomIntervals = new();

        private static readonly Dictionary<string, Thread> RoomThreads = new();

        private static IHubContext<GameHub> HubContext;

        private static ApplicationContext ApplicationContext;

        public GameHub(IHubContext<GameHub> hubContext, IMapper mapper, ApplicationContext context)
        {
            _mapper = mapper;
            HubContext = hubContext;
            ApplicationContext = context;
            //context.ListenEvent(typeof(DisconnectUserEvent), DisconnectUserHandle);
        }


        public async void DisconnectUserHandle(object sender, EventArgs args)
        {
            DisconnectUserEvent @event = (DisconnectUserEvent) args;
            GameRooms[PlayerRoomMap[@event.DisconnectUser.ConnectionId]].Players
                .Remove(@event.DisconnectUser.ConnectionId);
            ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
            await HubContext.Groups.RemoveFromGroupAsync(@event.DisconnectUser.ConnectionId,
                @event.DisconnectUser.RoomId);
        }

        /*
        private static void GameLoop(object? state)
        {
            var gameRoom = GameRooms[state.ToString()];
            
            HubContext.Clients.Group(gameRoom.Id).SendAsync("update", gameRoom.Players.Values, gameRoom.BulletsDict.Values);
        }*/

        private void GameLoop(GameRoom gameRoom)
        {
            while (true)
            {
                if (gameRoom.Looping)
                {
                    HubContext.Clients.Group(gameRoom.Id)
                        .SendAsync("update",
                            _mapper.Map<List<Player>, List<PlayerDto>>(gameRoom.Players.Values.ToList()),
                            _mapper.Map<List<Bullet>, List<BulletDto>>(gameRoom.BulletsDict.Values.ToList()));
                }
                Thread.Sleep(1000 / gameRoom.Tick);
            }
        }

        public async Task SendMove(MoveDto move)
        {
            try
            {
                var player = GameRooms[PlayerRoomMap[Context.ConnectionId]].Players[Context.ConnectionId];
                if (move.KeyState)
                {
                    player.Towards = move.Towards;
                }
                else if (player.Towards == move.Towards)
                {
                    player.Towards = Towards.NOWHERE;
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Error : " + ex.Message);
            }
        }

        public async Task FireBullet()
        {
            try
            {
                var gameRoom = GameRooms[PlayerRoomMap[Context.ConnectionId]];

                var player = gameRoom.Players[Context.ConnectionId];

                if (player.Towards != Towards.NOWHERE)
                {
                    var bullet = new Bullet(Context.ConnectionId, 5, 20, 0, player.Towards, "", player.PosX,
                        player.PosY);
                    gameRoom.BulletsDict.TryAdd(bullet.GetHashCode(), bullet);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Error : " + ex.Message);
            }
        }

        public async Task JoinGame(JoinDto joinDto)
        {
            try
            {
                var player = new Player(joinDto.Name);

                PlayerRoomMap[Context.ConnectionId] = joinDto.RoomId;

                var gameRoom = GameRooms[joinDto.RoomId];

                if (gameRoom.Looping == false)
                {
                    gameRoom.Looping = true;
                }
                else if (gameRoom.Players.Count == 0)
                {
                    RoomThreads[gameRoom.Id] = new Thread(() => GameLoop(gameRoom));
                    RoomThreads[gameRoom.Id].Start();
                    gameRoom.StartThreads();
                }

                if (gameRoom.Players.Count < gameRoom.PlayerCount)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, joinDto.RoomId);
                    gameRoom.Players[Context.ConnectionId] = player;
                    //ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join game room" + ex.Message);
            }
        }

        public async Task Leave(string roomId)
        {
            GameRooms[PlayerRoomMap[Context.ConnectionId]].Players.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
        }

        public IEnumerable<RoomOnDashboardDto> GetGameRooms()
        {
            //Init rooms if not exists.
            if (GameRooms.Values.Count == 0)
            {
                for (var i = 1; i <= 5; ++i)
                {
                    var gameRoom = new GameRoom($"Game Room {i}", i * 5, i * 5, Tick);
                    GameRooms[gameRoom.Id] = gameRoom;
                }
            }

            return _mapper.Map<List<GameRoom>, List<RoomOnDashboardDto>>(GameRooms.Values.ToList());
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            try
            {
                if (PlayerRoomMap.ContainsKey(Context.ConnectionId) && GameRooms[PlayerRoomMap[Context.ConnectionId]]
                    .Players.ContainsKey(Context.ConnectionId))
                {
                    var gameRoom = GameRooms[PlayerRoomMap[Context.ConnectionId]];

                    gameRoom.Players.Remove(Context.ConnectionId);

                    if (gameRoom.Players.Count == 0)
                    {
                        gameRoom.Looping = false;
                        //RoomThreads.Remove(gameRoom.Id);
                    }

                    Console.WriteLine(gameRoom.Players.Count);

                    PlayerRoomMap.Remove(Context.ConnectionId);
                    //ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine(exception.Message);
            }

            return base.OnDisconnectedAsync(ex);
        }

        private string GetDevice()
        {
            var device = Context.GetHttpContext().Request.Headers["Device"].ToString();
            if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile"))) return device;
            return "Web";
        }
    }
}