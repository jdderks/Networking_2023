using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetPlayerWin : NetMessage
{
    public byte teamWon;

    public NetPlayerWin()
    {
        Code = OpCode.PLAYER_WIN;
    }

    public NetPlayerWin(DataStreamReader reader)
    {
        Code = OpCode.PLAYER_WIN;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteByte(teamWon);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //byte is already read in OnData
        teamWon = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_PLAYER_WIN?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_PLAYER_WIN?.Invoke(this, cnn);
    }
}
