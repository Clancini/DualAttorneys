public static class RPCDefiner
{
    public enum RPCType : ushort
    {
        RespondToPing = 0 // Debug RPC to test network functionality. It's a response to Ping and sends a Pong message.
    }

    public static void DispatchRPC(RPCType type, params object[] param)
    {
        switch(type)
        {
            case RPCType.RespondToPing:

                break;
        }
    }
}
