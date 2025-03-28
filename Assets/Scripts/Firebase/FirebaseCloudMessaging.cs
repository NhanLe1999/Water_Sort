using UnityEngine;

namespace WaterSort
{
    public class FirebaseCloudMessaging : MonoBehaviour
    {
        private void Awake()
        {
            FirebaseServices.OnFirebaseReadyToUse += FirebaseServices_OnFirebaseReadyToUse;
        }
        private void FirebaseServices_OnFirebaseReadyToUse()
        {
           /* UnityEngine.Debug.Log("Registration firebase message");
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;*/
        }
        /*public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
        }*/
    }
}
