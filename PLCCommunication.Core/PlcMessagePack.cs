using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using PLCCommunication.Core.Enums;

namespace PLCCommunication.Core
{
    public class PlcMessagePack
    {

        public Int32 ChannelId { get; set; } = 0; //同一个 socket，不同的线程的时候，需要使用
        public Int32 MsgType { get; set; } //消息类型 0:req/ 1:rsp
        public Int32 CommandId { get; set; } //指令编号
        public float Param1 { get; set; } //参数 1，根据不同指令，决定是否启用
        public float Param2 { get; set; } = 0; //参数 2，根据不同指令，决定是否启用
        public float Param3 { get; set; } = 0; //参数 3，根据不同指令，决定是否启用
        public float Param4 { get; set; } = 0; //参数 4，根据不同指令，决定是否启用
        public float Param5 { get; set; } = 0; //参数 5，根据不同指令，决定是否启用
        public Int64 Errorcode { get; set; } = 0; //错误代码，0 为成功，其它为错误代码，详见第3节
        public string Text { get; set; } = ""; //默认32 个 byte 的字符数组，可以用来传字符串（该长度可以根据项目需求不同，进行不同的配置）


        public const int RequestIndicator = 0;
        public  const int RespondIndicator = 1;
        
        /// <summary>
        /// Send pause request to plc
        /// </summary>
        public static PlcMessagePack PauseMessage= new PlcMessagePack()
        {
            CommandId = 8,
            Param1 = 1,
        };

        public static PlcMessagePack StopMessage = new PlcMessagePack()
        {
            CommandId = 6,
            Param1 = 1,
        };
        
        public static PlcMessagePack StartMessage= new PlcMessagePack()
        {
            CommandId = 4,
            Param1 = 1,
        };

        public static readonly PlcMessagePack ResetMessage= new PlcMessagePack()
        {
            CommandId = 2,
            Param1 = 1,
        };
        
        
        public static readonly PlcMessagePack ContinueMessage = new PlcMessagePack()
        {
            CommandId = 10,
            Param1 = 1,
        };
        
        public static readonly PlcMessagePack AbortMessage = new PlcMessagePack()
        {
            CommandId = 18,
            Param1 = 0,
        };
        
        public static PlcMessagePack FromBytes(byte[] buffer)
        {
           var output  = new PlcMessagePack()
           {
               ChannelId = BitConverter.ToInt32(buffer, 0),
               MsgType = BitConverter.ToInt32(buffer, 4),
               CommandId = BitConverter.ToInt32(buffer, 8),
               Param1 = BitConverter.ToSingle(buffer, 12),
               Param2 = BitConverter.ToSingle(buffer, 16),
               Param3 = BitConverter.ToSingle(buffer, 20),
               Param4 = BitConverter.ToSingle(buffer, 24),
               Param5 = BitConverter.ToSingle(buffer, 28),
               Errorcode = BitConverter.ToInt64(buffer, 32),
               Text = Encoding.Default
                   .GetString(buffer.Skip(40).Take(32).ToArray()).TrimEnd('\0')
           };

           return output;
        }

        public byte[] ToBytes()
        {
            byte[] output = new byte[72];
           var msg = this;
           byte[] channelId = BitConverter.GetBytes(msg.ChannelId); //将int32转换为字节数组
            byte[] msgType = BitConverter.GetBytes(msg.MsgType); //将int32转换为字节数组
            byte[] commandId = BitConverter.GetBytes(msg.CommandId); //将int32转换为字节数组
            byte[] param1 = BitConverter.GetBytes(msg.Param1); //将float转换为字节数组
            byte[] param2 = BitConverter.GetBytes(msg.Param2); //将float转换为字节数组
            byte[] param3 = BitConverter.GetBytes(msg.Param3); //将float转换为字节数组
            byte[] param4 = BitConverter.GetBytes(msg.Param4); //将float转换为字节数组
            byte[] param5 = BitConverter.GetBytes(msg.Param5); //将float转换为字节数组
            byte[] errorcode = BitConverter.GetBytes(msg.Errorcode); //将int64转换为字节数组
            byte[] text = Encoding.ASCII.GetBytes(msg.Text); //将string转换为字节数组

            Array.Copy(channelId, 0, output, 0, 4);
            Array.Copy(msgType, 0, output, 4, 4);
            Array.Copy(commandId, 0, output, 8, 4);
            Array.Copy(param1, 0, output, 12, 4);
            Array.Copy(param2, 0, output, 16, 4);
            Array.Copy(param3, 0, output, 20, 4);
            Array.Copy(param4, 0, output, 24, 4);
            Array.Copy(param5, 0, output, 28, 4);
            Array.Copy(errorcode, 0, output, 32, 8);
            Array.Copy(text, 0, output, 40, text.Length);

            return output;
        }

    }
}