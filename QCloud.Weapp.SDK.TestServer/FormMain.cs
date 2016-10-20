using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Web.Http.Common;
using System.Net;
using System.Threading;

namespace QCloud.WeApp.TestServer
{
    public partial class FormMain : Form
    {
        private TestServer server;

        private string ServerUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["test-server-url"];
            }
        }

        private bool IsRunning
        {
            get
            {
                return !buttonStart.Enabled;
            }
            set
            {
                buttonStart.Enabled = !value;
                buttonStop.Enabled = value;
                textBoxServerUrl.Enabled = !value;
            }
        }

        public FormMain()
        {
            InitializeComponent();
            textBoxServerUrl.Text = ServerUrl;
            Start(this, null);
        }

        private void Start(object sender, EventArgs e)
        {
            server = new TestServer(textBoxServerUrl.Text);
            server.OnServerHandleResult += Server_OnServerHandleResult; ;
            server.Start();
            IsRunning = true;
        }

        private void Server_OnServerHandleResult(HandleResult result)
        {
            this.Invoke(new LogHandleResultDelegate(LogHandleResult), new object[] { result });
        }

        delegate void LogHandleResultDelegate(HandleResult result);
        private void LogHandleResult(HandleResult result)
        {
            var itemFields = new string[] {
                result.Time.ToString("HH:mm:ss"),
                result.Url
            };
            var item = new ListViewItem(itemFields)
            {
                Tag = result,
                Selected = true
            };
            listViewRequest.SelectedItems.Clear();
            listViewRequest.Items.Add(item);
        }

        private void Stop(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                IsRunning = false;
                server.Stop();
            }
        }

        private void SelectLog(object sender, EventArgs e)
        {
            if (listViewRequest.SelectedItems.Count > 0)
            {
                var item = listViewRequest.SelectedItems[0];
                HandleResult result = item.Tag as HandleResult;
                textBoxInfo.Text = result.ToString();
            }
        }

        private void ClearLog(object sender, EventArgs e)
        {
            listViewRequest.Items.Clear();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Stop();
        }
    }
}
