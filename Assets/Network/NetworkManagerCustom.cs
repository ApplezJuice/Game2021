using Mirror;

public class NetworkManagerCustom : NetworkManager
{
    public override void Start()
    {
#if UNITY_SERVER
            if (autoStartServerBuild)
            {
                StartServer();
            }
#else
        this.StartClient();
#endif
    }
}
