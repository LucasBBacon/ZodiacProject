using System;
using System.Collections;
using UnityEngine;


namespace Utilities
{
    public abstract class Sleeper
    {
        public IEnumerator PerformSleep(float duration)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1;
        }
    }
}
