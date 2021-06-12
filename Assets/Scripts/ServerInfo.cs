using Bolt;

public class ServerInfo : Bolt.IProtocolToken
{
    public bool privateSession;
    public string password;
    public double versionNumber = 1.0;

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteBool(privateSession);
        packet.WriteString(password);
        packet.WriteDouble(versionNumber);
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        privateSession = packet.ReadBool();
        password = packet.ReadString();
        versionNumber = packet.ReadDouble();
    }
}
