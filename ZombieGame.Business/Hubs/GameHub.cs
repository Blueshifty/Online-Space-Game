using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZombieGame.Business.Application;
using ZombieGame.Business.DTOs.Event;
using ZombieGame.Business.DTOs.Request;
using ZombieGame.Business.DTOs.Response;
using ZombieGame.Business.Hubs.HubModels;
using ZombieGame.Business.Utilities.Mapper;
using static ZombieGame.Business.Game.Dictionaries;

namespace ZombieGame.Business.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMapper _mapper;

        private static int Tick = 60;
        private static readonly Dictionary<string, Timer> RoomIntervals = new();

        private static IHubContext<GameHub> HubContext;
        private static ApplicationContext ApplicationContext;

        public GameHub(IHubContext<GameHub> hubContext, IMapper mapper, ApplicationContext context)
        {
            _mapper = mapper;
            HubContext = hubContext;
            ApplicationContext = context;
            context.ListenEvent(typeof(DisconnectUserEvent), DisconnectUserHandle);
        }


        public async void DisconnectUserHandle(object sender, EventArgs args)
        {
            DisconnectUserEvent @event = (DisconnectUserEvent) args;
            GameRooms[PlayerRoomMap[@event.DisconnectUser.ConnectionId]].Players.Remove(@event.DisconnectUser.ConnectionId);
            ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
            await HubContext.Groups.RemoveFromGroupAsync(@event.DisconnectUser.ConnectionId, @event.DisconnectUser.RoomId);

        }
        
        private static void GameLoop(object? state)
        {
            var gameRoom = GameRooms[state.ToString()];

            foreach (var player in gameRoom.Players.Values.Where(p => p.Towards != Towards.NOWHERE))
            {
                var posX = player.PosX;
                var posY = player.PosY;

                switch (player.Towards)
                {
                    case Towards.UP:
                        player.PosY -= gameRoom.MoveSpeed;
                        break;
                    case Towards.DOWN:
                        player.PosY += gameRoom.MoveSpeed;
                        break;
                    case Towards.RIGHT:
                        player.PosX += gameRoom.MoveSpeed;
                        break;
                    case Towards.LEFT:
                        player.PosX -= gameRoom.MoveSpeed;
                        break;
                }
                
                if (player.PosX > gameRoom.SizeX || player.PosX < 0 || player.PosY > gameRoom.SizeY ||
                    player.PosY < 0)
                {
                    player.PosX = posX;
                    player.PosY = posY;
                }
            }
            HubContext.Clients.Group(gameRoom.Id).SendAsync("update", gameRoom.Players.Values);
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

        public async Task JoinGame(JoinDto joinDto)
        {
            try
            {
                var player = new Player(joinDto.Name);

                PlayerRoomMap[Context.ConnectionId] = joinDto.RoomId;

                var gameRoom = GameRooms[joinDto.RoomId];

                if (gameRoom.Players.Count == 0)
                {
                    RoomIntervals[gameRoom.Id] = new Timer(GameLoop, gameRoom.Id, TimeSpan.Zero,
                        TimeSpan.FromMilliseconds(1000 / Tick));
                }
                
                if (gameRoom.Players.Count < gameRoom.PlayerCount)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, joinDto.RoomId);
                    gameRoom.Players[Context.ConnectionId] = player;
                    ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
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
                    var gameRoom = new GameRoom($"Game Room {i}", i*5 , i*5);
                    GameRooms[gameRoom.Id] = gameRoom;
                }
            }
            return _mapper.Map<List<GameRoom>, List<RoomOnDashboardDto>>(GameRooms.Values.ToList());
        }

        public override Task OnDisconnectedAsync(Exception ex)
        {
            if (PlayerRoomMap.ContainsKey(Context.ConnectionId) && GameRooms[PlayerRoomMap[Context.ConnectionId]].Players.ContainsKey(Context.ConnectionId))
            {
                GameRooms[PlayerRoomMap[Context.ConnectionId]].Players.Remove(Context.ConnectionId);
                
                if (GameRooms[PlayerRoomMap[Context.ConnectionId]].Players.Count == 0)
                {
                    RoomIntervals.Remove(PlayerRoomMap[Context.ConnectionId]);
                    
                }
                PlayerRoomMap.Remove(Context.ConnectionId);
                ApplicationContext.FireEvent(this, new JoinOrLeaveEvent());
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