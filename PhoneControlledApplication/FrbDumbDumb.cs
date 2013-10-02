using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PhoneControlledApplication
{
    public partial class FrbDumbDumb : Form
    {
        MediaPlayer mediaPlayer;
        public FrbDumbDumb()
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
            Thread lowMan = new Thread(new ThreadStart(mediaPlayer.takeOrders));
            lowMan.Start();
            mediaPlayer.startHndl += new startEventHandler(DisplayScreen);
            mediaPlayer.saveHndl += new savePlaylistEventHandler(SavePlaylist);
            mediaPlayer.cntlHndl += new cntlsEventHandler(ModifyControls);
        }
        private void ModifyControls(object sender, EventArgs e)
        {
            if (mediaPlayer.InvokeRequired)
            {
                cntlsEventHandler s = new cntlsEventHandler(ModifyControls);
                mediaPlayer.Invoke(s, new object[] { sender, e });
            }
            else
            {
                mediaPlayer.changeControls();
            }
        }
        private void DisplayScreen(object sender, EventArgs e)
        {
            if (mediaPlayer.InvokeRequired)
            {
                startEventHandler s = new startEventHandler(DisplayScreen);
                mediaPlayer.Invoke(s, new object[] { sender, e });
            }
            else
            {
                mediaPlayer.Show();
            }
        }

        private void SavePlaylist(object sender, EventArgs e)
        {
            if (mediaPlayer.InvokeRequired)
            {
                savePlaylistEventHandler s = new savePlaylistEventHandler(SavePlaylist);
                mediaPlayer.Invoke(s, new object[] { sender, e });
            }
            else
            {
                mediaPlayer.createPlaylist();
            }
        }
    }
}
