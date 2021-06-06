using Bolt;

public class ServerInfo : Bolt.IProtocolToken
{
    public bool privateSession;
    public string password;

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteBool(privateSession);
        packet.WriteString(password);
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        privateSession = packet.ReadBool();
        password = packet.ReadString();
    }
}
