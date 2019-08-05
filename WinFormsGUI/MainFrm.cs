using RequestCore.Enums;
using RequestTester.Entities;
using RequestTester.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsGUI
{
    public partial class MainFrm : Form
    {
        BindingList<RequestCase> requestsCases = new BindingList<RequestCase>();

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            dataGridViewResults.DataSource = requestsCases;
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
            requestsCases.Clear();
            foreach (var request in RequestLoaderManager.LoadRequests())
            {
                requestsCases.Add(request);
            }
        }

        CancellationTokenSource cancelTokenSource;

        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            listBoxServers.Enabled = false;
            buttonServerAdd.Enabled = false;
            buttonLoad.Enabled = false;
            buttonStop.Enabled = true;

            foreach (var requestCase in requestsCases)
            {
                requestCase._status = CaseStatus.NotDefined;
            }

            var servers = new List<string>();
            foreach(var server in listBoxServers.Items)
            {
                servers.Add((string)server);
            }

            if (cancelTokenSource != null)
                cancelTokenSource.Cancel();
             cancelTokenSource = new CancellationTokenSource();
            
            await RunQueries(servers.ToArray(), cancelTokenSource.Token, (requestsCases.Count < (int)numericUpDownMaxParallel.Value) ? requestsCases.Count : (int)numericUpDownMaxParallel.Value);

            buttonStart.Enabled = true;
            listBoxServers.Enabled = true;
            buttonServerAdd.Enabled = true;
            buttonLoad.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            if(cancelTokenSource != null)
                cancelTokenSource.Cancel();

            foreach (var requestCase in requestsCases)
            {
                if (requestCase._status == CaseStatus.Running)
                {
                    requestCase._status = CaseStatus.Breaked;
                }
            }
        }

        int completedCases = 0;
        async Task RunQueries(string[] servers, CancellationToken token, int maxParallel)
        {
            completedCases = 0;
            UpdateDataGridViewResultsInvoke();
            UpdateProgressInvoke(completedCases, requestsCases.Count);

            try
            {
                var coldTasks = new List<Task>(requestsCases.Count);
                foreach (var requstCase in requestsCases)
                {
                    coldTasks.Add(new Task(async () => await RequestParallelManager.SendRequestsParallelAsync(requstCase, servers, token)));
                }

                await ThreadManager.Run(coldTasks, maxParallel, caseCallback, token);
            }
            catch(TaskCanceledException)
            {
            }
        }

        
        private void caseCallback()
        {            
            UpdateDataGridViewResultsInvoke();
            UpdateProgressInvoke(++completedCases, requestsCases.Count);
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
