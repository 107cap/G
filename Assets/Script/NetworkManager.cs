using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;


public class NetworkManager : MonoBehaviour
{
    //TODO - 서버와 초기 접속이 될 시 자기 클라번호 저장해두기
    private static UdpClient udpClient;
    private static string serverIP = "192.168.0.252";
    private static int serverPort = 8080;
    //private static int localPort = 0;
    public ConcurrentQueue<IPacket> sendQue = new ConcurrentQueue<IPacket>();
    public ConcurrentQueue<IPacket> receiveQue = new ConcurrentQueue<IPacket>();

    public void SetConnectionInfo(string ip, string port)
    {
        serverIP = ip;
        serverPort = int.Parse(port);
    }

    public NetworkManager()
    {
        udpClient = new UdpClient();
        udpClient.Client.Blocking = false;
    }

    // deque 시켜서 send만 clinet->server
    public async Task SendAsync()
    {
        IPacket packet = null;
        sendQue.TryDequeue(out packet);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
        //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await udpClient.SendAsync(buff, buff.Length, serverEndPoint);
    }

    public void Send()
    {
        IPacket packet = null;
        sendQue.TryDequeue(out packet);
        
        if (packet != null)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            PlayerPacket pac = packet as PlayerPacket;
            byte[] buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pac));
            udpClient.Send(buff, buff.Length, serverEndPoint);
        }
        
    }

    public void flush()
    {
        IPacket packet = null;
        byte[] buff = null;
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        for (int i = sendQue.Count; i > 0; i--)
        {
            sendQue.TryDequeue(out packet);
            switch (packet.Type)
            {
                case (PacketType.PLAYER):
                {
                        PlayerPacket pac = packet as PlayerPacket;
                        buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pac));
                        break;
                }
                case (PacketType.EVENT):
                {
                        EventPacket pac = packet as EventPacket;
                        buff = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pac));
                        break;
                }
            }
            udpClient.Send(buff, buff.Length, serverEndPoint);
        }
    }
    
    // 받은 애 enque만 server->client
    public async Task ReceiveAsync()
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();
            receiveQue.Enqueue(
                JsonConvert.DeserializeObject<IPacket>(Encoding.UTF8.GetString(receivedResult.Buffer)));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

    }

    public void Receive()
    {
        if (udpClient.Available > 0)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            byte[] buff = udpClient.Receive(ref serverEndPoint);
            string receivedData = Encoding.UTF8.GetString(buff);
            
            IPacket packet = JsonConvert.DeserializeObject<IPacket>(receivedData);
            if (packet != null)
            {
                receiveQue.Enqueue(packet);
                if (packet.Type == PacketType.PLAYER)
                    Debug.Log("리시브 시점 - " + "번호 : " + packet.ClientNum + ", " + (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ((PlayerPacket)packet).timestamp));
            }
        }
    }
}
