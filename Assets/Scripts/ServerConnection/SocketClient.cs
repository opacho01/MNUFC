using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages TCP socket communication with a server, handling connection, messaging, and data retrieval.
/// </summary>
public class SocketClient : MonoBehaviour
{
    /// <summary>
    /// The IP address of the server to connect to.
    /// </summary>
    private static readonly string serverIp;

    /// <summary>
    /// The port number used for the server connection.
    /// </summary>
    private static readonly int serverPort;

    /// <summary>
    /// The TCP client instance used for communication.
    /// </summary>
    private static TcpClient _client;

    /// <summary>
    /// The network stream used for sending and receiving data.
    /// </summary>
    private static NetworkStream _stream;

    /// <summary>
    /// Stores the response containing the product list.
    /// </summary>
    public static ProductListResponse productList;

    /// <summary>
    /// Stores the response containing inventory data.
    /// </summary>
    public static InventoryResponse inventoryResponse;

    /// <summary>
    /// Static constructor to initialize the socket client and establish connection.
    /// </summary>
    static SocketClient()
    {
        serverIp = "127.0.0.1";
        serverPort = 5000;
        _client = null;
        _stream = null;
        productList = null;
        inventoryResponse = null;
        ConnectSocket();
    }

    /// <summary>
    /// Closes the socket connection when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Close();
    }

    /// <summary>
    /// Closes and disposes the current socket connection.
    /// </summary>
    private static void DisposeSocket()
    {
        if (_stream != null)
        {
            _stream.Close();
            _stream.Dispose();
            _stream = null;
        }
        if (_client != null)
        {
            _client.Close();
            _client.Dispose();
            _client = null;
        }
    }

    /// <summary>
    /// Establishes a connection to the server via TCP socket.
    /// </summary>
    private static void ConnectSocket()
    {
        try
        {
            DisposeSocket();
            _client = new TcpClient(serverIp, serverPort);
            _stream = _client.GetStream();
            Debug.Log("[socket] Connected to server successfully.");
        }
        catch (SocketException ex)
        {
            Debug.LogError("[socket] SocketException: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    /// <summary>
    /// Sends a message to the server over the established socket connection.
    /// </summary>
    /// <param name="message">The message to send.</param>
    private new static void SendMessage(string message)
    {
        try
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("No connection to the server.");
            }
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            _stream.Write(bytes, 0, bytes.Length);
            Debug.Log("[socket] Sent: " + message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    /// <summary>
    /// Receives a message from the server with a specified timeout period.
    /// </summary>
    /// <param name="timeoutMilliseconds">The timeout duration for receiving data (default: 1500ms).</param>
    /// <returns>The received message as a string, or null if an error occurs.</returns>
    private static string ReceiveMessage(int timeoutMilliseconds = 1500)
    {
        try
        {
            if (_stream == null)
            {
                throw new InvalidOperationException("No connection to the server.");
            }
            _stream.ReadTimeout = timeoutMilliseconds;
            string text = "";
            string receivedData;
            do
            {
                byte[] buffer = new byte[1024];
                int count = _stream.Read(buffer, 0, buffer.Length);
                receivedData = Encoding.ASCII.GetString(buffer, 0, count);
                text += receivedData;
                Debug.Log("[socket] Received:\n" + receivedData);
            }
            while (!receivedData.EndsWith("<|EOM|>"));
            return text.TrimEnd("<|EOM|>");
        }
        catch (IOException ex) when (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.TimedOut)
        {
            Debug.LogError("[socket] " + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Closes the socket connection.
    /// </summary>
    public static void Close()
    {
        DisposeSocket();
    }

    /// <summary>
    /// The input field for entering the product name.
    /// </summary>
    public TMP_InputField productNameXIF;

    /// <summary>
    /// The input field for entering the slot number.
    /// </summary>
    public TMP_InputField slotXIF;

    /// <summary>
    /// The input field for entering the quantity.
    /// </summary>
    public TMP_InputField quantityXIF;

    /// <summary>
    /// Sends a vending request to the server.
    /// </summary>
    /// <param name="productName">The name of the product to vend.</param>
    /// <param name="slot">The slot from which to vend the product.</param>
    /// <param name="quantity">The quantity of the product to vend.</param>
    /// <returns>The vending operation's status response from the server.</returns>
    public static string Vend(string productName, string slot, int quantity)
    {
        if (_stream == null || _client == null || !_client.Connected)
        {
            ConnectSocket();
        }
        SendMessage($"\r\n        {{\r\n            \"request_type\": \"vend_request\",\r\n            \"product_name\": \"{productName}\",\r\n            \"slot\": \"{slot}\",\r\n            \"quantity\": {quantity}\r\n        }}");
        string response = ReceiveMessage(5000);
        if (!string.IsNullOrEmpty(response))
        {
            try
            {
                return JsonUtility.FromJson<VendResponse>(response).status;
            }
            catch (Exception)
            {
                return "Failed";
            }
        }
        return "Failed";
    }

    /// <summary>
    /// Sends a prize request by vending an item with quantity set to 1.
    /// </summary>
    /// <param name="name">The name of the prize item to vend.</param>
    /// <param name="slot">The slot from which to vend the prize item.</param>
    public void SendPrize(string name, string slot)
    {
        Vend(name, slot, 1);
    }
}
