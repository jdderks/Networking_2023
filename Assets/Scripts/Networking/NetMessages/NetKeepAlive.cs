using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetKeepAlive : NetMessage
{


    public NetKeepAlive()
    {
        Code = OpCode.KEEP_ALIVE;
    }

    public NetKeepAlive(DataStreamReader reader)
    {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        //base.Serialize(ref writer);
    }

    public override void Deserialize(DataStreamReader reader)
    {

    }
    public override void ReceivedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);//base.ReceivedOnClient();
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }


}
