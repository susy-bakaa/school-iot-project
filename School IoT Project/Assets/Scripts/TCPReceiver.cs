using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class TCPReceiver : MonoBehaviour
{
    TCPReceiver instance;

    public enum AxisMapping
    {
        X, // Maps to the X-axis from accelerometer data
        Y, // Maps to the Y-axis from accelerometer data
        Z, // Maps to the Z-axis from accelerometer data
        None // Axis disabled
    }

    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream stream;

    public int port = 5000;  // Port for communication
    public Transform targetObject;  // The target object to move
    public Vector2 tiltLimit = new Vector2(-20,20);
    public float scale = 1f;

    public AxisSettings xSettings = new AxisSettings { mapping = AxisMapping.X, enabled = true, reversed = false, scale = 100f };
    public AxisSettings ySettings = new AxisSettings { mapping = AxisMapping.Y, enabled = true, reversed = false, scale = 100f };
    public AxisSettings zSettings = new AxisSettings { mapping = AxisMapping.Z, enabled = true, reversed = false, scale = 100f };

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        try
        {
            // Start listening on the specified port
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Debug.Log($"TCP Server started at port {port}, waiting for a connection...");

            // Start accepting clients asynchronously
            tcpListener.BeginAcceptTcpClient(OnClientConnected, null);
        }
        catch (Exception e)
        {
            Debug.LogWarning("TCP Server error: " + e.Message);
        }
    }

    // Called when a client connects
    private void OnClientConnected(IAsyncResult result)
    {
        try
        {
            // Accept the client connection
            tcpClient = tcpListener.EndAcceptTcpClient(result);
            stream = tcpClient.GetStream();
            Debug.Log("Client connected!");

            // Start reading data asynchronously
            ReadDataAsync();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Client connection error: " + e.Message);
        }
    }

    // Method to read data continuously from the connected client
    private async void ReadDataAsync()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (tcpClient != null && tcpClient.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    // Convert bytes to string
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.Log("Received data: " + receivedData);

                    // Parse and apply data on the main thread
                    UnityMainThreadDispatcher.Instance().Enqueue(() => ParseAndApplyData(receivedData));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error reading data: " + e.Message);
        }
    }

    // Method to parse and apply data to the target object
    private void ParseAndApplyData(string data)
    {
        try
        {
            // Example: Parse X, Y, Z data from "X:val,Y:val,Z:val"
            string[] parts = data.Split(',');
            float ax = float.Parse(parts[0].Split(':')[1]);
            float ay = float.Parse(parts[1].Split(':')[1]);
            float az = float.Parse(parts[2].Split(':')[1]);

            // Calculate pitch and roll angles based on accelerometer data
            float pitch = Mathf.Atan2(ax, Mathf.Sqrt(ay * ay + az * az)) * Mathf.Rad2Deg;
            float roll = Mathf.Atan2(ay, Mathf.Sqrt(ax * ax + az * az)) * Mathf.Rad2Deg;

            // Map the pitch and roll to the target rotation (assuming Unity X and Z control the tilt)
            float clampedPitch = Mathf.Clamp(pitch * scale, tiltLimit.x, tiltLimit.y);
            float clampedRoll = Mathf.Clamp(roll * scale, tiltLimit.x, tiltLimit.y);
            Vector3 targetRotation = new Vector3(clampedPitch, 0f, clampedRoll);

            if (targetObject != null)
            {
                // Start a coroutine to smoothly rotate to the new target rotation over 0.5 seconds
                StartCoroutine(SmoothRotate(targetRotation, 0.5f));
            }
            else
            {
                StopAllCoroutines();
                targetObject = GameObject.FindGameObjectWithTag("Stage").transform;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed parsing the sensor data: " + e.Message);
        }
    }

    private float GetMappedValue(AxisSettings settings, int x, int y, int z)
    {
        if (!settings.enabled)
            return 0f;

        float value = 0f;

        switch (settings.mapping)
        {
            case AxisMapping.X:
                value = x;
                break;
            case AxisMapping.Y:
                value = y;
                break;
            case AxisMapping.Z:
                value = z;
                break;
            case AxisMapping.None:
                return 0f; // Axis is disabled
        }

        // Apply reversal if needed
        return settings.reversed ? -value : value;
    }


    private IEnumerator SmoothRotate(Vector3 targetEulerAngles, float duration)
    {
        // Store the initial rotation
        Quaternion initialRotation = Quaternion.identity;

        if (targetObject != null)
            initialRotation = targetObject.rotation;

        // Calculate the target rotation from the given Euler angles
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Smoothly interpolate from the initial to the target rotation
            if (targetObject != null)
                targetObject.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsed / duration);

            // Increment the elapsed time
            elapsed += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the final rotation is set to the exact target rotation
        if (targetObject != null)
            targetObject.rotation = targetRotation;
    }

    void OnApplicationQuit()
    {
        // Close the TCP listener and client when quitting the application
        tcpListener?.Stop();
        tcpClient?.Close();
    }

    [System.Serializable]
    public struct AxisSettings
    {
        public AxisMapping mapping;  // The mapping for this axis (X, Y, Z, None)
        public bool enabled;         // Whether this axis is enabled
        public bool reversed;        // Whether this axis should be reversed
        public float scale;
    }

}
