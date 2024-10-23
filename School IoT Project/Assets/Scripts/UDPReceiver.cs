using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    public string ip = "192.168.7.168";  // Microcontroller's IP address
    public int port = 5000;  // Port for communication
    public int remotePort = 5001;
    public Transform targetObject;  // The target object to move
    public float scale = 100f;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    void Start()
    {
        try
        {
            // Initialize UDP Client for receiving
            udpClient = new UdpClient(port);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

            // Send a test packet to the microcontroller before receiving
            //SendTestPacket();

            // Start receiving data asynchronously
            udpClient.BeginReceive(new AsyncCallback(ReceiveData), null);
            Debug.Log($"Listening for UDP data on port {port}...");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to start UDP listener: " + e.Message);
        }
    }

    // Method to send a test UDP packet to the microcontroller
    private void SendTestPacket()
    {
        try
        {
            string testMessage = "Hello from Unity";  // Test packet message
            byte[] data = Encoding.ASCII.GetBytes(testMessage);

            using (UdpClient senderClient = new UdpClient())  // Create a new UDP client for sending
            {
                senderClient.Send(data, data.Length, new IPEndPoint(remoteEndPoint.Address, remotePort));  // Send the test packet
                Debug.Log("Test UDP packet sent to " + remoteEndPoint.Address.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send UDP test packet: " + e.Message);
        }
    }

    // Method for receiving data asynchronously
    private void ReceiveData(IAsyncResult result)
    {
        try
        {
            byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receivedBytes);

            Debug.Log("Received UDP packet: " + receivedString);
            Debug.Log("From IP: " + remoteEndPoint.Address.ToString());

            // Continue listening for new data
            udpClient.BeginReceive(new AsyncCallback(ReceiveData), null);

            // Parse and apply data on the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() => ParseAndApplyData(receivedString));
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving UDP data: " + e.Message);
        }
    }

    // Method to parse and apply data to the target object
    private void ParseAndApplyData(string data)
    {
        try
        {
            // Example: Parse X, Y, Z data from "X:val,Y:val,Z:val"
            string[] parts = data.Split(',');
            int x = int.Parse(parts[0].Split(':')[1]);
            int y = int.Parse(parts[1].Split(':')[1]);
            int z = int.Parse(parts[2].Split(':')[1]);

            // Apply the data to object movement or behavior (on the main thread)
            Vector3 movement = new Vector3(x / scale, y / scale, z / scale);  // Scale as needed
            targetObject.eulerAngles += movement;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse UDP data: " + e.Message);
        }
    }

    // Close the UDP client when the application quits
    void OnApplicationQuit()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

    void OnDisable()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}