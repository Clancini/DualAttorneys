using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

public class NetworkReceiver
{
    private static NetworkReceiver instance;
    public static NetworkReceiver Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetworkReceiver();
            }
            return instance;
        }
    }

    private Thread steamReceiver;
    private bool isRunning = false;

    public ConcurrentQueue<byte[]> receivedMessages { get; private set; } = new ConcurrentQueue<byte[]>();

    const int failDelayMS = NetMessageDefiner.failDelayMS;

    public NetworkReceiver()
    {
        isRunning = true;

        steamReceiver = new Thread(new ThreadStart(ReceiveMessage));
    }

    void ReceiveMessage()
    {
        IntPtr[] messageBuffer = new IntPtr[NetMessageDefiner.maxMessagedReadAtOnce];

        while (isRunning)
        {
            if (DualAttorneysLobby.Instance.connection == HSteamNetConnection.Invalid)  // If not connected, there's no point in going further
            {
                Thread.Sleep(failDelayMS);  // Sleep for a bit to avoid hogging the CPU
                return;
            }

            int numMessages = SteamNetworkingSockets.ReceiveMessagesOnConnection(DualAttorneysLobby.Instance.connection, messageBuffer, NetMessageDefiner.maxMessagedReadAtOnce);
            // Above, get the number of waiting-to-be-read messages. Below, if that number is  0, skip the rest of the loop else, continue parsing
            if (numMessages <= 0)
            {
                Thread.Sleep(failDelayMS);
                continue; 
            }

            // For every message we received, we turn it into a byte array and add it to the queue
            for (int i = 0; i < numMessages; i++)
            {
                try
                {
                    SteamNetworkingMessage_t steamNetworkingMessage = Marshal.PtrToStructure<SteamNetworkingMessage_t>(messageBuffer[i]);

                    byte[] byteArrayMessage = new byte[steamNetworkingMessage.m_cbSize];
                    Marshal.Copy(steamNetworkingMessage.m_pData, byteArrayMessage, 0, steamNetworkingMessage.m_cbSize);

                    receivedMessages.Enqueue(byteArrayMessage);
                }
                finally
                {
                    Marshal.DestroyStructure<SteamNetworkingMessage_t>(messageBuffer[i]);
                }
            }
        }
    }
}

