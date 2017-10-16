using Param_RootNamespace.Services;
using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Param_RootNamespace.Services
{
    public class TrackingService : ITrackingService
    {
        /// <summary>
        /// Tracks custom events.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void TrackEvent(string eventName)
        {
            HockeyClient.Current.TrackEvent(eventName);
        }
    }
}
