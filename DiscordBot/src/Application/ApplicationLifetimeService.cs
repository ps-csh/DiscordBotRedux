using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Application
{
    //TODO: Find a different name, or look into Microsoft's IHostApplicationLifetime
    internal class ApplicationLifetimeService
    {
        public delegate void LifetimeCallback();

        private LifetimeCallback? _onApplicationShuttingDown;
        private LifetimeCallback? _onApplicationShutDown;
        //private LifetimeCallback? _onApplicationShuttingDown;

        public void Shutdown()
        {
            try
            {
                _onApplicationShuttingDown?.Invoke();

                _onApplicationShutDown?.Invoke();
            }
            catch (Exception ex)
            {
                
            }
        }

        public void RegisterShuttingDownCallback(LifetimeCallback callback)
        {
            _onApplicationShuttingDown += callback;
        }

        public void RegisterShutDownCallback(LifetimeCallback callback)
        {
            _onApplicationShutDown += callback;
        }
    }
}
