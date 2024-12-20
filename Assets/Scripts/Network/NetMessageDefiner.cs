using System;

public static class NetMessageDefiner
{
    /// <summary>
    /// What the associated message is about. <br/>
    /// Represented by a ushort (2 bytes)
    /// </summary>
    public enum MessageType : ushort
    {
        Ping = 0,   // Debug message to test network functionality
        Pong = 1    // Response to Ping
    }

    /// <summary>
    /// Represents the NEXT bytes' type.<br/>
    /// [ByteType][ByteData][ByteType][ByteData]...
    /// </summary>
    public enum MessageParamType : byte
    {
        Int = 0,
        Float = 1,
        String = 2
    }

    /// <summary>
    /// Converts supported types to byte arrays. <br/>
    /// Returns a byte array containing [ByteType][ByteData].<br/>
    /// If it's a string, [ByteType][ByteDataLength][ByteData] is returned.<br/>
    /// If the type is not supported, an empty byte array is returned.
    /// </summary>
    public static byte[] ConvertToBytes(object param)
    {
        byte[] result;

        if (param is int)
        {
            result = new byte[5];
            result[0] = (byte)MessageParamType.Int;
            Buffer.BlockCopy(BitConverter.GetBytes((int)param), 0, result, 1, 4);
        }
        else if (param is float)
        {
            result = new byte[5];
            result[0] = (byte)MessageParamType.Float;
            Buffer.BlockCopy(BitConverter.GetBytes((float)param), 0, result, 1, 4);
        }
        else if (param is string)
        {
            result = new byte[((string)param).Length + 2];
            result[0] = (byte)MessageParamType.String;
            result[1] = (byte)((string)param).Length;
            Buffer.BlockCopy(System.Text.Encoding.UTF8.GetBytes((string)param), 0, result, 2, ((string)param).Length);
        }
        else
        {
            result = new byte[0];
        }

        return result;
    }
}


