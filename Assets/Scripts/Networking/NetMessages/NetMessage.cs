///Much credit for this code goes to "Mercenary Camp" on Youtube: https://youtu.be/lPoiTw0qjtc?list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp
///Thank you Mercenary Camp for the Unity networking Tutorial.

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{ //0-255 slots (a byte)
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    CUBE_CLICKED = 4,
    REMATCH = 5
}

public class NetMessage
{
    public OpCode Code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public virtual void Deserialize(DataStreamReader reader)
    {

    }

    public virtual void ReceivedOnClient()
    {

    }

    public virtual void ReceivedOnServer(NetworkConnection cnn)
    {

    }
}
