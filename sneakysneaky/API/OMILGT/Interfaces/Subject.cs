using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OMILGT
{
    namespace Interfaces
    {

        public interface ISubject
        {
            void Notify();

            void AddWatcher(IWatcher newWatcher);
            void RemoveWatcher(IWatcher watcher);


        }
    }
}
