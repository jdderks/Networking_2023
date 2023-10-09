using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetCubeClicked : NetMessage
{

    public int xPosition;
    public int zPosition;
    public int team;


    public NetCubeClicked()
    {
        Code = OpCode.CUBE_CLICKED;
    }

    public NetCubeClicked(DataStreamReader reader)
    {
        Code = OpCode.CUBE_CLICKED;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(xPosition);
        writer.WriteInt(zPosition);
        writer.WriteInt(team);

    }

    public override void Deserialize(DataStreamReader reader)
    {
        //byte is already read in OnData
        xPosition = reader.ReadInt();
        zPosition = reader.ReadInt();
        team = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CUBE_CLICKED?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CUBE_CLICKED?.Invoke(this, cnn);
    }
}
