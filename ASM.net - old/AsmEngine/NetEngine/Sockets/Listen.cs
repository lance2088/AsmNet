using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using AsmEngine.Wrappers;
using AsmEngine.Instructions;

namespace AsmEngine.NetEngine.Sockets
{
    internal unsafe class Listen : Acall
    {
        public Listen()
            : base()
        { }

        public override void Call()
        {
            unchecked
            {
                unsafe
                {
                    ws2_32.WSAData wsaData = new ws2_32.WSAData();
                    wsaData.Version = 2;
                    wsaData.HighVersion = 2;

                    if (ws2_32.WSAStartup(36, out wsaData) == 0) //0=success
                    {
                        IntPtr SocketHandle;
                        void* SocketAddr;
                        SocketHandle = ws2_32.socket(ws2_32.AddressFamily.InterNetworkv4, ws2_32.SocketType.Stream, ws2_32.ProtocolType.Tcp);
                        SocketAddr = (void*)SocketHandle;

                        if (SocketHandle != (IntPtr)ws2_32.INVALID_SOCKET)
                        {
                            ws2_32.sockaddr_in remoteAddress = new ws2_32.sockaddr_in();
                            remoteAddress.sin_family = 2;
                            remoteAddress.sin_port = ws2_32.htons(Convert.ToUInt16(args[1].value));
                            remoteAddress.sin_addr = (int)ws2_32.inet_addr((string)args[0].value);

                            if (ws2_32.bind(SocketHandle, remoteAddress, Marshal.SizeOf(remoteAddress)) == 0)
                            {
                                if (ws2_32.listen(SocketHandle, 0) == 0)
                                {
                                    AssemblerExecute.registers.EAX = SocketHandle.ToInt32();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}