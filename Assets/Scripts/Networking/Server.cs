///Much credit for this code goes to "Mercenary Camp" on Youtube: https://youtu.be/lPoiTw0qjtc?list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp
///Thank you Mercenary Camp for the Unity networking Tutorial.


using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.Assertions;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static Server Instance { get; private set; }

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastKeepAlive;

    public Action OnConnectionDropped;

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

    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.LoopbackIpv4; //This is looped back to not hack the dutch game garden

        endpoint.Port = port;

        if (driver.Bind(endpoint) != 0)
        {
            Debug.Log("unable to bind to port " + endpoint.Port);
            return;
        }
        else
        {
            driver.Listen();
            Debug.Log("Currently listening on port " + endpoint.Port);
        }

        connections = new(2, Allocator.Persistent);
        isActive = true;
    }

    public void Shutdown()
    {
        if (isActive)
        {
            driver.Dispose();
            connections.Dispose();
            isActive = false;
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

        KeepAlive();

        driver.ScheduleUpdate().Complete(); //Empty up queue of messages

        CleanupConnections(); //Is there anybody not connected but still referenced?
        AcceptNewConnections(); //Is there someone knocking on the door to connect to the server
        UpdateMessagePump(); //Are they sending a message and do we have to reply?

    }

    public void CleanupConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    public void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
        }
    }

    public void UpdateMessagePump()
    {
        DataStreamReader stream;

        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, connections[i], this);
                } 
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    OnConnectionDropped?.Invoke();
                    Shutdown(); //This could be removed if we don't want the entire server to be shutdown if one player disconnects
                }
            }
        }
    }

    public void SendToClient(NetworkConnection connection, NetMessage message)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        message.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void Broadcast(NetMessage message)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                //Debug.Log("Sending NetMessage: " + message.Code + " to " + connections[i].InternalId);
                SendToClient(connections[i], message);
            }
        }
    }

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > keepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }
}
