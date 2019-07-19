using RequestTester.Entities;
using RequestTester.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RequestTester
{
    public partial class MainFrm : Form
    {
        const int maxParallel = 100;
        const int timeout = 10000;

        BindingList<RequestCase> data = new BindingList<RequestCase>();

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            dataGridViewResults.DataSource = data;
            listBoxServers.Items.Add("https://ya.ru/");
            listBoxServers.Items.Add("https://yandex.ru/");
        }

        private void ButtonServerAdd_Click(object sender, EventArgs e)
        {
            listBoxServers.Items.Add(textBoxServerForAdd.Text);
        }

        private void ClearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBoxServers.Items.Clear();
        }

        private void DeleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = listBoxServers.SelectedIndex;
            if (selected >= 0)
                listBoxServers.Items.RemoveAt(selected);
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            data.Clear();
            foreach (var request in RequestLoaderManager.LoadRequests())
            {
                data.Add(request);
            }
        }

        CancellationTokenSource cancelTokenSource;

        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            var servers = new List<string>();
            foreach(var server in listBoxServers.Items)
            {
                servers.Add((string)server);
            }

            if (cancelTokenSource != null)
                cancelTokenSource.Cancel();
             cancelTokenSource = new CancellationTokenSource();

            await RunQueries(servers.ToArray(), cancelTokenSource.Token);
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if(cancelTokenSource != null)
                cancelTokenSource.Cancel();
        }

        async Task RunQueries(string[] servers, CancellationToken token)
        {
            UpdateDataGridViewResultsInvoke();

            var taskList = new List<Task>(maxParallel);
            foreach (var requestCase in data)
            {
                var task = RequestParallelManager.QueryParallel(requestCase, servers, token);
                taskList.Add(task);
                if (taskList.Count == maxParallel)
                {
                    var result = await Task.WhenAny(taskList).ConfigureAwait(false);
                    taskList.Remove(result);

                    UpdateDataGridViewResultsInvoke();
                }
            }
        }

        void UpdateDataGridViewResultsInvoke()
        {
            dataGridViewResults.Invoke((MethodInvoker)delegate { dataGridViewResults.Update(); });
            dataGridViewResults.Invoke((MethodInvoker)delegate { dataGridViewResults.Refresh(); });
        }

    }
}
