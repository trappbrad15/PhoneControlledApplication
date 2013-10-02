using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;

namespace PhoneControlledApplication
{
    class PhoneServer
    {
        private static PhoneServer instance;
        private string[] fileFormats = {".mov", ".wmz", ".wms", ".m4a", ".mp4", ".m4v", ".mp4v", ".3g2", ".3gp2", ".3gp",
                                        ".3gpp", ".aac", ".adt", "adts", ".wav", ".cda", ".mpg", ".mpeg", ".m1v", ".mp2",
                                        ".mp3", ".mpa", ".mpe", ".m3u", ".avi"};
        private Socket socket_;
        private IPAddress ipAddress_;
        private TcpListener myListener;
        private bool isRunning_;
        private string returnString;
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
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            if (localIP != "")
            {
                ipAddress_ = IPAddress.Parse(localIP);
                myListener = new TcpListener(ipAddress_, 8001);
            }
            else
            {
                //failed to find ip address
            }
        }


        public void getConnection()
        {
            Console.Write("Waiting for client connection...\n");
            myListener.Start();
            while (isRunning_)
            {
                socket_ = myListener.AcceptSocket();
                Console.Write(socket_.RemoteEndPoint.ToString() + '\n');
                byte[] b = new byte[10000];
                int message = socket_.Receive(b);
                char asciiMessage = ' ';
                string fullMessage = null;
                string command = "";
                string path = "";
                string selectedPlaylistName = "";
                string[] temp;
                for (int i = 0; i < message; i++)
                {
                    asciiMessage = Convert.ToChar(b[i]);
                    fullMessage += asciiMessage.ToString();
                }
                temp = fullMessage.Split(';');
                command = temp[0];

                Console.Write("Server command: " + command + '\n');

                switch (command)
                {
                    case "BuildPlaylist":
                        path = temp[1];
                        Console.Write("Path: " + path + '\n');
                        returnString = getFilesAtLocation(path, 0);
                        b = Encoding.UTF8.GetBytes(returnString);
                        socket_.Send(b, SocketFlags.None);
                        break;
                    case "SelectPlaylist":
                        returnString = getPlaylists();
                        b = Encoding.UTF8.GetBytes(returnString);
                        socket_.Send(b, SocketFlags.None);
                        break;
                    case "MediaPlayer":
                        Global.orders = "START";
                        break;
                    case "COMMAND":
                        Global.orders = temp[1];
                        break;
                    case "SavePlaylist":
                        returnString = "ack";
                        b = Encoding.UTF8.GetBytes(returnString);
                        socket_.Send(b, SocketFlags.None);
                        Global.playlistName = temp[1];
                        string tempFile = "";
                        foreach (string file in temp)
                        {
                            if (file == temp[0] || file == Global.playlistName || file == "")
                            {
                                //this is the command, name or blank => don't do anything
                            }
                            else
                            {
                                tempFile = file.Substring(7);
                                tempFile = tempFile.TrimEnd('}');

                                Global.mediaFiles.Add(tempFile);
                            }
                            Global.orders = "SAVE";
                        }
                        break;
                    default:
                        break;
                }
                socket_.Close();
            }
        }

        string getPlaylists()
        {
            string playlistNames = "";
            string path = System.Environment.GetEnvironmentVariable("USERPROFILE");
            path = path + "\\Music\\Playlists";

            foreach (string file in Directory.GetFiles(path))
            {
                playlistNames += Path.GetFileNameWithoutExtension(file) + ";";
            }
            return playlistNames;
        }
        string getFilesAtLocation(string path, int iteration)
        {
            string fileWithExtension = "";
            string fileWithoutExtension = "";
            string allFiles = "";
            string tempPath = Path.GetFullPath(path);
            if(Directory.Exists(tempPath))
            {
                foreach(string folder in Directory.GetDirectories(tempPath))
                {
                    allFiles += getFilesAtLocation(folder, iteration+1);
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        foreach(string format in fileFormats)
                        {
                            if(file.Contains(format))
                            {
                                string[] fileLocations = new string[2];
                                fileWithExtension = Path.GetFullPath(file);
                                fileWithoutExtension = Path.GetFileNameWithoutExtension(file);
                                fileLocations[0] = fileWithoutExtension;
                                fileLocations[1] = fileWithExtension;
                                Global.allFoundFiles.Add(fileLocations);
                                allFiles += Path.GetFileNameWithoutExtension(file) + ";";
                            }
                        }
                    }
                }
                if (iteration == 0)
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        foreach (string format in fileFormats)
                        {
                            if (file.Contains(format))
                            {
                                string[] fileLocations = new string[2];
                                fileWithExtension = Path.GetFullPath(file);
                                fileWithoutExtension = Path.GetFileNameWithoutExtension(file);
                                fileLocations[0] = fileWithoutExtension;
                                fileLocations[1] = fileWithExtension;
                                Global.allFoundFiles.Add(fileLocations);
                                allFiles += Path.GetFileNameWithoutExtension(file) + ";";
                            }
                        }
                    }
                }
            }
            return allFiles;
        }
    }
}
