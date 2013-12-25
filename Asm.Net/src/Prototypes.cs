using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    public unsafe class Prototypes
    {
        //user32
        public delegate int MessageBoxA(int id, string msg, string title, int btn);
        public delegate int SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        //kernel32
        public delegate void ExitProcess(int id);
        public delegate int GetStdHandle(int id);
        public delegate void Sleep(int time);
        public delegate bool WriteConsoleA(int handle, string value, int len, int written, int zero);

        //ws2_32
        public delegate int WSACleanup();
        public delegate int WSAStartup(int Version, out WSAData Data);
        public delegate int WSAGetLastError();
        public delegate int socket(int af, int type, int protocol);
        public delegate int send(int s, byte * buf, int len, int flags);
        public delegate int recv(int s, int buf, int len, int flags);
        public delegate int listen(int s, int backlog);
        public delegate uint inet_addr(string cp);
        public delegate ushort htons(int hostshort);
        public delegate int closesocket(int SocketHandle);
        public delegate int inet_ntoa(int _in);
        public delegate ulong htonl(long hostlong);
        public delegate ulong ntohl(long netlong);
        public delegate ushort ntohs(int netshort);
        public delegate int bind(int SocketHandle, ref sockaddr_in addr, int namelen);
        public delegate int accept(int socketHandle, ref sockaddr_in socketAddress, ref int addressLength);
        public delegate int connect(int SocketHandle, ref sockaddr_in addr, int addrsize);

        ///<summary> Please check the arguments of the function you want to call </summary>
        public static Delegate GetDelegate(Libraries lib, Functions func)
        {
            switch (lib)
            {
                case Libraries.User32:
                {
                    switch (func)
                    {
                        case Functions.User32_MessageBoxA:
                            return new MessageBoxA((int id, string msg, string title, int button) => { return 0; });
                        case Functions.User32_SendMessageA:
                            return new SendMessage((IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam) => { return 0; });
                    }
                    break;
                }
                case Libraries.Kernel32:
                {
                    switch (func)
                    {
                        case Functions.Kernel32_ExitProcess:
                            return new ExitProcess((int a) => { });
                        case Functions.Kernel32_GetStdHandle:
                            return new GetStdHandle((int a) => { return 0; });
                        case Functions.Kernel32_Sleep:
                            return new Sleep((int a) => { });
                        case Functions.Kernel32_WriteConsoleA:
                            return new WriteConsoleA((int handle, string value, int len, int written, int zero) => { return false; });
                    }
                    break;
                }
                case Libraries.ws2_32:
                {
                    switch (func)
                    {
                        case Functions.ws2_32_recv:
                            return new recv((int s, int buf, int len, int flags) => { return 0; });
                        case Functions.ws2_32_send:
                            return new send((int s, byte * buf, int len, int flags) => { return 0; });
                        case Functions.ws2_32_closesocket:
                            return new closesocket((int SocketHandle) => { return 0; });
                        case Functions.ws2_32_htonl:
                            return new htonl((long hostlong) => { return 0; });
                        case Functions.ws2_32_htons:
                            return new htons((int hostshort) => { return 0; });
                        case Functions.ws2_32_inet_addr:
                            return new inet_addr((string cp) => { return 0; });
                        case Functions.ws2_32_inet_ntoa:
                            return new inet_ntoa((int _in) => { return 0; });
                        case Functions.ws2_32_listen:
                            return new listen((int s, int backlog) => { return 0; });
                        case Functions.ws2_32_ntohl:
                            return new ntohl((long netlong) => { return 0; });
                        case Functions.ws2_32_ntohs:
                            return new ntohs((int netshort) => { return 0; });
                        case Functions.ws2_32_socket:
                            return new socket((int af, int type, int protocol) => { return 0; });
                        case Functions.ws2_32_WSACleanup:
                            return new WSACleanup(() => { return 0; });
                        case Functions.ws2_32_WSAGetLastError:
                            return new WSAGetLastError(() => { return 0; });
                        case Functions.ws2_32_WSAStartup:
                            return new WSAStartup((int Version, out WSAData Data) => { Data = new WSAData();  return 0; });
                        case Functions.ws2_32_bind:
                            return new bind((int SocketHandle, ref sockaddr_in addr, int namelen) => { return 0; });
                        case Functions.ws2_32_accept:
                            return new accept((int socketHandle, ref sockaddr_in socketAddress, ref int addressLength) => { return 0; });
                        case Functions.ws2_32_connect:
                            return new connect((int SocketHandle, ref sockaddr_in addr, int addrsize) => { return 0; });
                    }
                    break;
                }
            }
            return null;
        }
    }

    public enum Libraries
    {
        User32,
        Kernel32,
        ws2_32
    }
    public enum Functions
    {
        ///<summary> (int id, string msg, string title, int btn) </summary>
        User32_MessageBoxA,
        ///<summary> (IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam) </summary>
        User32_SendMessageA,
        ///<summary> (int id) </summary>
        Kernel32_GetStdHandle,
        ///<summary> (int handle, string value, int len, int written, int zero) </summary>
        Kernel32_WriteConsoleA,
        ///<summary> (int milliseconds) </summary>
        Kernel32_Sleep,
        ///<summary> (int id) </summary>
        Kernel32_ExitProcess,
        ///<summary> no arguments </summary>
        ws2_32_WSACleanup,
        ///<summary> (int Version, WSAData Data) </summary>
        ws2_32_WSAStartup,
        ///<summary> no arguments </summary>
        ws2_32_WSAGetLastError,
        ///<summary> (int af, int type, int protocol) </summary>
        ws2_32_socket,
        ///<summary> (int socket, byte * buf, int len, int flags) </summary>
        ws2_32_send,
        ///<summary> (int socket, byte * buf, int len, int flags) </summary>
        ws2_32_recv,
        ///<summary> (int socket, int backlog) </summary>
        ws2_32_listen,
        ///<summary> (string cp) </summary>
        ws2_32_inet_addr,
        ///<summary> (int netshort) </summary>
        ws2_32_htons,
        ///<summary> (int socket) </summary>
        ws2_32_closesocket,
        ///<summary> (int _in) </summary>
        ws2_32_inet_ntoa,
        ///<summary> (long hostlong) </summary>
        ws2_32_htonl,
        ///<summary> (long netlong) </summary>
        ws2_32_ntohl,
        ///<summary> (int netshort) </summary>
        ws2_32_ntohs,
        ///<summary> (int SocketHandle, ref sockaddr_in addr, int namelen) </summary>
        ws2_32_bind,
        ///<summary> (int socketHandle, ref sockaddr_in socketAddress, ref int addressLength) </summary>
        ws2_32_accept,
        ///<summary> (int socketHandle, ref sockaddr_in socketAddress, ref int addressLength) </summary>
        ws2_32_connect,
    }
}