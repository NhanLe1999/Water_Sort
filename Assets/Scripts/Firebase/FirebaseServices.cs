using System;
using UnityEngine;

namespace WaterSort
{
    public class FirebaseServices : MonoBehaviour
    {
        public static event Action OnFirebaseReadyToUse;
        public bool isReady = false;
        void Start()
        {
            /*Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    isReady = true;
                    OnFirebaseReadyToUse?.Invoke();
                }
            });*/
        }
    }
}
