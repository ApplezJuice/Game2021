using Mirror;

public class NetworkManagerCustom : NetworkManager
{
    public override void Start()
    {
        //StartServer();
        this.StartClient();
    }
}
