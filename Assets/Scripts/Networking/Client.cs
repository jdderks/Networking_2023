///Much credit for this code goes to "Mercenary Camp" on Youtube: https://youtu.be/lPoiTw0qjtc?list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp
///Thank you Mercenary Camp for the Unity networking Tutorial.


using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;
using System;

public class Client : MonoBehaviour
{
    public static Client Instance;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;

    public Action OnConnectionDropped;


    public void Init(string ip, ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);

        connection = driver.Connect(endpoint);

        Debug.Log("Attempting to connect to server on " + endpoint.Address);

        RegisterToEvent();
        isActive = true;
    }

    public void Shutdown()
    {
        if (isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            isActive = false;
            connection = default(NetworkConnection);
        }
    }

    private void OnDestroy()
    {
        Shutdown();
    }

    public void Update()
    {
        if (!isActive)
            return;

        driver.ScheduleUpdate().Complete(); //Empty up queue of messages
        CheckAlive();
        UpdateMessagePump(); //Are they sending a message and do we have to reply?


    }

    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            Debug.Log("Something went wrong, lost connection to server");
            OnConnectionDropped?.Invoke();
            Shutdown();
        }
    }



    public void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                SendToServer(new NetWelcome());
                Debug.Log("Connected to server!!!");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, default(NetworkConnection));
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Something went wrong, lost connection to server");
                connection = default(NetworkConnection);
                OnConnectionDropped?.Invoke();
                Shutdown(); 
            }
        }
    }

    public void SendToServer(NetMessage message)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        message.Serialize(ref writer);
        driver.EndSend(writer);
    }

    //Event parsin
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }
    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm)
    {
        SendToServer(nm);
    }


}
