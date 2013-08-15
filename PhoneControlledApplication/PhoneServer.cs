using System;
using System.Collections.Generic;using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace PhoneControlledApplication
{
    class PhoneServer
    {
        private static PhoneServer instance;

        private Socket socket_;
        private IPAddress ipAddress_;
        private TcpListener myListener;
        private bool isRunning_;

        public static PhoneServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PhoneServer();
                }
                return instance;
            }
        }

        private PhoneServer()
        {
            isRunning_ = true;
            ipAddress_ = IPAddress.Parse("192.168.0.101");
            myListener = new TcpListener(ipAddress_, 8001);
        }

        public void getConnection()
        {
            myListener.Start();
            while (isRunning_)
            {
                socket_ = myListener.AcceptSocket();
                Console.Write(socket_.RemoteEndPoint.ToString());
            }
        }


    }
}
