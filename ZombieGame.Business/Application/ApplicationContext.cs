using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace ZombieGame.Business.Utilities
{
    public class ApplicationContext
    { 
        private readonly ConcurrentDictionary<Type, EventHandler> handlers = new(); // Belki EventListener listesi alınabilir birden fazla handle olan senaryoalrda
        
        public void FireEvent(object sender, ApplicationEvent @event)
        {
            this.handlers[@event.GetType()].Invoke(sender, @event);
        }

        public void ListenEvent(Type type, EventHandler handler)
        {
            this.handlers.TryAdd(type, handler);
        }

    }

    public class ApplicationEvent: EventArgs
    {
        public override string ToString()
        {
            return nameof(this.GetType);
        }
    }

}