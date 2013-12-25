using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace AsmEngine.Wrappers
{
    internal class ws2_32
    {
        public static readonly int SOCKET_ERROR = -1;
        public static readonly int INVALID_SOCKET = ~0;

        [DllImport("ws2_32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 WSACleanup();
        [DllImport("Ws2_32.dll")]
        public static extern int WSAStartup(ushort Version, out WSAData Data);
        [DllImport("Ws2_32.dll")]
        public static extern SocketError WSAGetLastError();
        [DllImport("Ws2_32.dll")]
        public static extern IntPtr socket(AddressFamily af, SocketType type, ProtocolType protocol);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int send(IntPtr SocketHandle, byte[] buf, int len, int flags);
        [DllImport("Ws2_32.dll")]
        public static extern int recv(IntPtr SocketHandle, byte[] buf, int len, int flags);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern IntPtr accept(IntPtr SocketHandle, sockaddr_in addr, int addrsize);
        [DllImport("Ws2_32.dll")]
        public static extern int listen(IntPtr s, int backlog);
        [DllImport("Ws2_32.dll", CharSet = CharSet.Ansi)]
        public static extern uint inet_addr(string cp);
        [DllImport("Ws2_32.dll")]
        public static extern ushort htons(ushort hostshort);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int connect(IntPtr SocketHandle, sockaddr_in addr, int addrsize);
        [DllImport("Ws2_32.dll")]
        public static extern int closesocket(IntPtr SocketHandle);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int getpeername(IntPtr SocketHandle, sockaddr_in* addr, int* addrsize);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern int bind(IntPtr SocketHandle, sockaddr_in addr, int namelen);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern sbyte* inet_ntoa(sockaddr_in _in);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern ulong htonl(ulong hostlong);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern ulong ntohl(ulong netlong);
        [DllImport("Ws2_32.dll")]
        public static unsafe extern ushort ntohs(ushort netshort);






        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct WSAData
        {
            public ushort Version;
            public ushort HighVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
            public string Description;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
            public string SystemStatus;
            public ushort MaxSockets;
            public ushort MaxUdpDg;
            sbyte* lpVendorInfo;
        }

        public enum AddressFamily : int
        {
            Unknown = 0,
            InterNetworkv4 = 2,
            Ipx = 4,
            AppleTalk = 17,
            NetBios = 17,
            InterNetworkv6 = 23,
            Irda = 26,
            BlueTooth = 32
        }
        public enum SocketType : int
        {
            Unknown = 0,
            Stream = 1,
            DGram = 2,
            Raw = 3,
            Rdm = 4,
            SeqPacket = 5
        }
        public enum ProtocolType : int
        {
            BlueTooth = 3,
            Tcp = 6,
            Udp = 17,
            ReliableMulticast = 113
        }

        public unsafe struct fd_set
        {
            public const int FD_SETSIZE = 64;
            public uint fd_count;
            public fixed uint fd_array[FD_SETSIZE];
        }

        public enum OPTION_FLAGS_PER_SOCKET : short
        {
            // turn on debugging info recording  
            SO_DEBUG = 0x0001,
            // socket has had listen()  
            SO_ACCEPTCONN = 0x0002,
            // allow local address reuse  
            SO_REUSEADDR = 0x0004,
            // keep connections alive  
            SO_KEEPALIVE = 0x0008,
            // just use interface addresses  
            SO_DONTROUTE = 0x0010,
            // permit sending of broadcast msgs  
            SO_BROADCAST = 0x0020,
            // bypass hardware when possible  
            SO_USELOOPBACK = 0x0040,
            // linger on close if data present  
            SO_LINGER = 0x0080,
            // leave received OOB data in line  
            SO_OOBINLINE = 0x0100,
            SO_DONTLINGER = (int)(~SO_LINGER),
            // disallow local address reuse
            SO_EXCLUSIVEADDRUSE = ((int)(~SO_REUSEADDR)),

            ///*
            // * Additional options.
            // */
            // send buffer size  
            SO_SNDBUF = 0x1001,
            // receive buffer size  
            SO_RCVBUF = 0x1002,
            // send low-water mark  
            SO_SNDLOWAT = 0x1003,
            // receive low-water mark  
            SO_RCVLOWAT = 0x1004,
            // send timeout  
            SO_SNDTIMEO = 0x1005,
            // receive timeout  
            SO_RCVTIMEO = 0x1006,
            // get error status and clear  
            SO_ERROR = 0x1007,
            // get socket type  
            SO_TYPE = 0x1008,

            ///*
            // * WinSock 2 extension -- new options
            // */
            // ID of a socket group  
            SO_GROUP_ID = 0x2001,
            // the relative priority within a group
            SO_GROUP_PRIORITY = 0x2002,
            // maximum message size  
            SO_MAX_MSG_SIZE = 0x2003,
            // WSAPROTOCOL_INFOA structure  
            SO_PROTOCOL_INFOA = 0x2004,
            // WSAPROTOCOL_INFOW structure  
            SO_PROTOCOL_INFOW = 0x2005,
            // configuration info for service provider  
            PVD_CONFIG = 0x3001,
            // enable true conditional accept: connection is not ack-ed to the other side until conditional function returns CF_ACCEPT  
            SO_CONDITIONAL_ACCEPT = 0x3002
        }


        /// <summary>
        /// Internet socket address structure.
        /// </summary>
        public struct sockaddr_in
        {
            /// <summary>
            /// Protocol family indicator.
            /// </summary>
            public short sin_family;
            /// <summary>
            /// Protocol port.
            /// </summary>
            public ushort sin_port;
            /// <summary>
            /// Actual address value.
            /// </summary>
            public int sin_addr;
            /// <summary>
            /// Address content list.
            /// </summary>
            //[MarshalAs(UnmanagedType.LPStr, SizeConst=8)]
            //public string sin_zero;
            public long sin_zero;
        }

        [StructLayout(LayoutKind.Sequential, Size = 16)]
        public struct SocketAddr_in
        {
            public const int Size = 16;

            public short sin_family;
            public ushort sin_port;
            public struct in_addr
            {
                public uint S_addr;
                public struct _S_un_b
                {
                    public byte s_b1, s_b2, s_b3, s_b4;
                }
                public _S_un_b S_un_b;
                public struct _S_un_w
                {
                    public ushort s_w1, s_w2;
                }
                public _S_un_w S_un_w;
            }
            public in_addr sin_addr;
        }
    }
}