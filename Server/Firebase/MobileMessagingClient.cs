using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace HVMDash.Server.Firebase
{
    public class MobileMessagingClient
    {
        public readonly FirebaseMessaging messaging;

        public MobileMessagingClient()
        {
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.GetApplicationDefault(),
            //});

            var app = FirebaseApp.Create(new AppOptions() { Credential = GoogleCredential.FromFile("spotytovkshare-firebase-adminsdk-2pdkq-299fac3b28.json").CreateScoped("https://www.googleapis.com/auth/firebase.messaging") });
            messaging = FirebaseMessaging.GetMessaging(app);
        }
    }
}
