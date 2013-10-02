using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PhoneControlledApplication
{
    public delegate void startEventHandler(object sender, EventArgs e);
    public delegate void savePlaylistEventHandler(object sender, EventArgs e);
    public delegate void cntlsEventHandler(object sender, EventArgs e);

    public partial class MediaPlayer : Form
    {
        private string issuedCommand = "";
        public event startEventHandler startHndl;
        public event savePlaylistEventHandler saveHndl;
        public event cntlsEventHandler cntlHndl;

        public MediaPlayer()
        {
            InitializeComponent();
        }

        protected virtual void OnCommand(EventArgs e)
        {
            if (cntlHndl != null)
            {
                cntlHndl(this, e);
            }
        }

        protected virtual void OnStart(EventArgs e)
        {
            if (startHndl != null)
            {
                startHndl(this, e);
            }
        }

        protected virtual void OnSave(EventArgs e)
        {
            if (saveHndl != null)
            {
                saveHndl(this, e);
            }
        }
        public void command()
        {
            OnCommand(EventArgs.Empty);
        }

        public void display()
        {
            OnStart(EventArgs.Empty);
        }

        public void save()
        {
            OnSave(EventArgs.Empty);
        }

        public void takeOrders()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                switch (Global.orders)
                {
                    case "SAVE":
                        Global.orders = "";
                        save();
                        break;
                    case "START":
                        Global.orders = "";
                        display();
                        break;
                    case "PLAY":
                        Global.orders = "";
                        Global.command = "PLAY";
                        command();
                        break;
                    case "PAUSE":
                        Global.orders = "";
                        Global.command = "PAUSE";
                        command();
                        break;
                    case "NEXT":
                        Global.orders = "";
                        Global.command = "NEXT";
                        command();
                        break;
                    case "PREV":
                        Global.orders = "";
                        Global.command = "PREV";
                        command();
                        break;
                    case "KILL":
                        Global.orders = "";
                        keepGoing = false;
                        break;
                    default:
                        break;
                        
                }
            }
        }

        public void changeControls()
        {
            switch (Global.command)
            {
                case "PLAY":
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    break;
                case "PAUSE":
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                    break;
                case "NEXT":
                    axWindowsMediaPlayer1.Ctlcontrols.next();
                    break;
                case "PREV":
                    axWindowsMediaPlayer1.Ctlcontrols.previous();
                    break;
                default:
                    break;
            }
            Global.command = "";
        }

        public void createPlaylist()
        {
            WMPLib.IWMPPlaylist playlist = axWindowsMediaPlayer1.playlistCollection.newPlaylist(Global.playlistName);
            WMPLib.IWMPMedia media;
            for (int i = 0; i <Global.mediaFiles.Count; i++)
            {
                media = axWindowsMediaPlayer1.newMedia(findPath(Global.mediaFiles[i]));
                playlist.appendItem(media);
            }
            axWindowsMediaPlayer1.currentPlaylist = playlist;
        }

        public string findPath(string name)
        {
            string path = "";
            bool found = false;
            int i = 0;
            while (i < Global.allFoundFiles.Count && !found)
            {
                string temp = Global.allFoundFiles[i].ElementAt(0);
                if (temp == name)
                {
                    path = Global.allFoundFiles[i].ElementAt(1);
                    found = true;
                }
                i++;
            }
            return path;
        }

        public bool assignPlaylist(string name)
        {
            bool status = false;
            WMPLib.IWMPPlaylist playlist;
            WMPLib.IWMPPlaylistArray playlistArray;
            WMPLib.IWMPPlaylistCollection collection;
            string path = System.Environment.GetEnvironmentVariable("USERPROFILE");
            path = path + "\\Music\\Playlists";
            collection = axWindowsMediaPlayer1.playlistCollection;
            
            

            foreach (string file in Directory.GetFiles(path))
            {
                if (file.Contains(".wpl"))
                {
                    
                }
            }

            playlistArray = collection.getAll();

            for(int i = 0; i < playlistArray.count; i++)
            {
                playlist = playlistArray.Item(i);
                if (playlist.name == name)
                {
                    axWindowsMediaPlayer1.currentPlaylist = playlist;
                    status = true;
                    break;
                }
            }
            return status;
        }
    }
}
