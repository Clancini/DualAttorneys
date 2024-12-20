using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using static NetMessageDefiner;

public class NetworkSender
{
    private static NetworkSender instance;

    public static NetworkSender Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetworkSender();
            }
            return instance;
        }
    }

    private Thread steamSender;
    private bool isRunning = false;

    ConcurrentQueue<KeyValuePair<MessageType, object[]>> messageQueue = new ConcurrentQueue<KeyValuePair<MessageType, object[]>>();

    const int failDelayMS = 10; // Delay in ms to wait before trying again in case there are no messages to avoid hogging the CPU

    private NetworkSender()
    {
        isRunning = true;

        steamSender = new Thread(new ThreadStart(SendMessage));
    }

    // Where the message is turned into IntPtr and sent
    private void SendMessage() 
    {
        while(isRunning)
        {
            if(DualAttorneysLobby.Instance.connection == HSteamNetConnection.Invalid) // If not connected, there's no point in going further
            {
                Thread.Sleep(failDelayMS); // Sleep for a bit to avoid hogging the CPU
                return;
            }

            KeyValuePair<MessageType, object[]> message;
            if (messageQueue.TryDequeue(out message))
            {
                byte[] msgType = BitConverter.GetBytes((ushort)message.Key);
                List<byte> paramsAsBytes = new List<byte>();

                // For each parameter in the message, we convert it into byte array
                // We then add the whole byte array to the list
                foreach (object param in message.Value)
                {
                    paramsAsBytes.AddRange(NetMessageDefiner.ConvertToBytes(param));
                }

                byte[] fullMessage = new byte[msgType.Length + paramsAsBytes.Count];

                Buffer.BlockCopy(msgType, 0, fullMessage, 0, msgType.Length);   // Copy the message type to the full message (in the first 2 bytes)
                Buffer.BlockCopy(paramsAsBytes.ToArray(), 0, fullMessage, msgType.Length, paramsAsBytes.Count); // Copy the parameters to the full message (after the first 2 bytes)

                // Allocate memory for the message and finally send it
                IntPtr messagePtr = Marshal.AllocHGlobal(fullMessage.Length);

                try
                {
                    long messageNumber;
                    
                    Marshal.Copy(fullMessage, 0, messagePtr, fullMessage.Length);
                    SteamNetworkingSockets.SendMessageToConnection(DualAttorneysLobby.Instance.connection, messagePtr, (uint)fullMessage.Length, (int)EP2PSend.k_EP2PSendReliable, out messageNumber);
                }
                finally
                {
                    Marshal.FreeHGlobal(messagePtr);    // We're done with the message, so we free the memory
                }
            }
            else
            {
                // There are no messages to send, so we sleep for a bit to avoid hogging the CPU
                Thread.Sleep(failDelayMS);
                continue;
            }

        }
    }

    ///<summary>
    /// Queues a message to be sent to the other player.<br/>
    /// Used to update anything in the game if supported.<br/>
    /// Currently only supports int, float, and string.<br/>
    /// Currently only supports reliable messages.
    ///</summary>
    public void NetSend(MessageType type, params object[] message)
    {
        messageQueue.Enqueue(new KeyValuePair<MessageType, object[]>(type, message));
    }
}
