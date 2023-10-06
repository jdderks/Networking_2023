using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetStartGame : NetMessage
{
    public NetStartGame()
    {
        Code = OpCode.START_GAME;
    }

    public NetStartGame(DataStreamReader reader)
    {
        Code = OpCode.START_GAME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);

    }

    public override void Deserialize(DataStreamReader reader)
    {
        //byte is already read in OnData
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_START_GAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }
}
