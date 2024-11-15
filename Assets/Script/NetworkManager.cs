using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class NetworkManager
{
    private static UdpClient udpClient;
    private static string serverIP = "172.16.1.217";
    private static int serverPort = 8080;
    private static int localPort = 0;
    public ConcurrentQueue<byte[]> sendQue = new ConcurrentQueue<byte[]>();
    public ConcurrentQueue<byte[]> receiveQue = new ConcurrentQueue<byte[]>();

    public NetworkManager()
    {
        udpClient = new UdpClient();
    }

    // deque 시켜서 send만 clinet->server
    public async Task SendAsync()
    {
        byte[] buff = null;
        sendQue.TryDequeue(out buff);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await udpClient.SendAsync(buff, buff.Length, serverEndPoint);
    }

    public void Send()
    {
        byte[] buff = null;
        sendQue.TryDequeue(out buff);
        if (buff != null)
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(buff, buff.Length, serverEndPoint);
        }
        
    }
    
    // 받은 애 enque만 server->client
    public async Task Receive()
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

        try
        {
            UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();
            receiveQue.Enqueue(receivedResult.Buffer);
            
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Receive");
        }
    }

    void Process()
    {
        // 패킷 데이터 변환
    }



}
