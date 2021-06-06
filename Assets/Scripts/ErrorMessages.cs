using Bolt;

public class ErrorMessages : Bolt.IProtocolToken
{
    public string Error;

    public void Write(UdpKit.UdpPacket packet)
    { 
        packet.WriteString(Error);
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        Error = packet.ReadString();
    }
}
