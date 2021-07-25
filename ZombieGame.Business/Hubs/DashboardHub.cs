#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZombieGame.Business.DTOs.Event;
using ZombieGame.Business.DTOs.Request;
using static ZombieGame.Business.Game.Dictionaries;
using ZombieGame.Business.Utilities;

namespace ZombieGame.Business.Hubs
{
    public class DashboardHub: Hub
    {

        private readonly IHubContext<DashboardHub> _hubContext;
        private ApplicationContext ApplicationContext { get; }
        public DashboardHub(IHubContext<DashboardHub> hubContext, ApplicationContext context)
        {
            _hubContext = hubContext;
            ApplicationContext = context;
            context.ListenEvent(typeof(JoinOrLeaveEvent), EventHandle);
        }

        private async void EventHandle(object? sender, EventArgs e)
        {
            // var @event = (JoinOrLeaveEvent) e;  Kullanacagımız senaryolarda cast ederek kullanabiliriz.
            await this.OnSend();
        }

        public async Task OnSend()
        {
            await _hubContext.Clients.All.SendAsync("onInit", StatisticUtil.Statistics(GameRooms, PlayerRoomMap));
        }

        public void OnDisconnectUser(DisconnectUser user)
        {
            ApplicationContext.FireEvent(this, new DisconnectUserEvent(user));
        }
    

        public override async Task OnConnectedAsync()
        {
            await this.OnSend();
        }
    }
}