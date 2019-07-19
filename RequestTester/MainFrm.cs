using RequestTester.Entities;
using RequestTester.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace RequestTester
{
    public partial class MainFrm : Form
    {
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
            foreach (var requestCase in data)
            {
                requestCase._status = RequestCase.CaseStatus.NotDefined;
            }

            var servers = new List<string>();
            foreach(var server in listBoxServers.Items)
            {
                servers.Add((string)server);
            }

            if (cancelTokenSource != null)
                cancelTokenSource.Cancel();
             cancelTokenSource = new CancellationTokenSource();

            await RunQueries(servers.ToArray(), cancelTokenSource.Token, (data.Count < (int)numericUpDownMaxParallel.Value) ? data.Count : (int)numericUpDownMaxParallel.Value);
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if(cancelTokenSource != null)
                cancelTokenSource.Cancel();

            foreach (var requestCase in data)
            {
                if (requestCase._status == RequestCase.CaseStatus.Running)
                {
                    requestCase._status = RequestCase.CaseStatus.Breaked;
                }
            }
        }

        async Task RunQueries(string[] servers, CancellationToken token, int maxParallel)
        {
            int i = 0;
            UpdateDataGridViewResultsInvoke();
            UpdateProgressInvoke(i, data.Count);

            var taskList = new List<Task>(maxParallel);
            foreach (var requestCase in data)
            {
                if (token.IsCancellationRequested)
                    continue;

                var task = RequestParallelManager.QueryParallel(requestCase, servers, token);
                taskList.Add(task);
                if (taskList.Count == maxParallel)
                {
                    var result = await Task.WhenAny(taskList).ConfigureAwait(false);
                    taskList.Remove(result);

                    UpdateDataGridViewResultsInvoke();
                    UpdateProgressInvoke(++i, data.Count);
                }
            }
        }

        void UpdateDataGridViewResultsInvoke()
        {
            dataGridViewResults.Invoke((MethodInvoker)delegate {
                dataGridViewResults.Update();
                dataGridViewResults.Refresh();
            });
        }

        void UpdateProgressInvoke(int completed, int total)
        {
            statusStrip.Invoke((MethodInvoker)delegate {
                toolStripProgressBar.Maximum = total;
                toolStripProgressBar.Value = completed;
                toolStripStatusLabel.Text = $"{completed} of {total}";
            });
        }


        private void DataGridViewResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = (DataGridViewRow)dataGridViewResults.Rows[e.RowIndex];

            DiffManager.ShowDiff((RequestCase)row.DataBoundItem);

        }
    }
}
