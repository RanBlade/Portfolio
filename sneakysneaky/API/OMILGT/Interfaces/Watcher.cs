using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OMILGT
{
    namespace Interfaces
    {
        public enum WatcherMessages
        {
            TIMERCOMPLETE,
            TOTAL_MESSAGES
        }

        public interface IWatcher
        {
            void OnNotify(string watcherName, WatcherMessages message);
        }
    }
}
