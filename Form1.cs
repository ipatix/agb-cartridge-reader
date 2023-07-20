using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGB_Cartridge_Reader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dsComboBox.SelectedIndex = 0;
            usComboBox.SelectedIndex = 0;
            drComboBox.SelectedIndex = 0;

            setProgress(0, null);

            refreshSerialPorts();
        }

        private void lockGUI()
        {
            drButtonDot.Enabled = false;
            drButtonGo.Enabled = false;
            drTextBox.Enabled = false;
            drComboBox.Enabled = false;

            dsButtonDot.Enabled = false;
            dsButtonGo.Enabled = false;
            dsTextBox.Enabled = false;
            dsComboBox.Enabled = false;

            usButtonDot.Enabled = false;
            usButtonGo.Enabled = false;
            usTextBox.Enabled = false;
            usComboBox.Enabled = false;
        }

        private void unlockGUI()
        {

            drButtonDot.Enabled = true;
            drButtonGo.Enabled = true;
            drTextBox.Enabled = true;
            drComboBox.Enabled = true;

            dsButtonDot.Enabled = true;
            dsButtonGo.Enabled = true;
            dsTextBox.Enabled = true;
            dsComboBox.Enabled = true;

            usButtonDot.Enabled = true;
            usButtonGo.Enabled = true;
            usTextBox.Enabled = true;
            usComboBox.Enabled = true;
        }

        private void refreshSerialPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Count() == 0)
            {
                serialPortsComboBox.Text = "";
                serialPortsComboBox.SelectedIndex = -1;
            }
            else
            {
                string oldSelected = serialPortsComboBox.Text;
                serialPortsComboBox.Items.Clear();
                foreach (string port in ports)
                    serialPortsComboBox.Items.Add(port);
                int index = serialPortsComboBox.Items.IndexOf(oldSelected);
                if (index == -1)
                {
                    if (serialPortsComboBox.Items.Count == 0)
                        serialPortsComboBox.Text = "";
                    else
                        serialPortsComboBox.Text = ports[0];
                }
                else
                {
                    serialPortsComboBox.SelectedIndex = index;
                }
            }
        }

        private static string humanReadable(int x)
        {
            if (x >= 1024)
                return (x >> 10).ToString() + "K";
            else
                return x.ToString();
        }

        private void setProgress(int percentage, ProgressState ps)
        {
            if (ps != null)
            {
                bytesLabel.Text = "Bytes/s: " + humanReadable(ps.bytesPerSecond);
                totalBytesLabel.Text = "Bytes: " + humanReadable(ps.totalBytes);
            }
            else
            {
                bytesLabel.Text = "Bytes/s: none";
                totalBytesLabel.Text = "Bytes: none";
            }
            progressBar1.Value = percentage;
        }

        private void transceiver_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            setProgress(e.ProgressPercentage, e.UserState as ProgressState);
        }

        private void transceiver_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show("An Error occured while executing the request:\n" + e.Error.Message, "lala", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (e.Cancelled)
                MessageBox.Show("Transfer did NOT complete!", "blabla", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Transfer Complete!", "foobar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            setProgress(0, null);
            unlockGUI();
        }

        private void transceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            Reader.Action(transceiver, readerPort, e);
        }

        private void drButtonGo_Click(object sender, EventArgs e)
        {
            string file = drTextBox.Text;
            WorkerArgs.OperationType type;
            switch (drComboBox.Text)
            {
                case "Auto":
                    type = WorkerArgs.OperationType.DOWNLOAD_AUTO_ROM;
                    break;
                case "4 MiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_4M_ROM;
                    break;
                case "8 MiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_8M_ROM;
                    break;
                case "16 MiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_16M_ROM;
                    break;
                case "32 MiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_32M_ROM;
                    break;
                default:
                    throw new ArgumentException("Invalid ROM Download Option selected");
            }
            WorkerArgs wa = new WorkerArgs(file, type);
            if (transceiver.IsBusy)
                return;
            lockGUI();
            transceiver.RunWorkerAsync(wa);
        }

        private void drButtonDot_Click(object sender, EventArgs e)
        {
            if (sfdROM.ShowDialog() == DialogResult.OK)
                drTextBox.Text = sfdROM.FileName;
        }

        private void dsButtonDot_Click(object sender, EventArgs e)
        {
            if (sfdSave.ShowDialog() == DialogResult.OK)
                dsTextBox.Text = sfdSave.FileName;
        }

        private void usButtonDot_Click(object sender, EventArgs e)
        {
            if (ofdSave.ShowDialog() == DialogResult.OK)
                usTextBox.Text = ofdSave.FileName;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (transceiver.IsBusy)
                transceiver.CancelAsync();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            transceiver.CancelAsync();

            while (transceiver.IsBusy)
                System.Threading.Thread.Sleep(100);
        }

        private void dsButtonGo_Click(object sender, EventArgs e)
        {
            string file = dsTextBox.Text;
            WorkerArgs.OperationType type;
            switch (dsComboBox.Text)
            {
                case "Auto":
                    type = WorkerArgs.OperationType.DOWNLOAD_AUTO_SAVE;
                    break;
                case "EEPROM 512 B":
                    type = WorkerArgs.OperationType.DOWNLOAD_512_EEPROM;
                    break;
                case "EEPROM 8 KiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_8K_EEPROM;
                    break;
                case "SRAM 32 KiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_SRAM_32K;
                    break;
                case "FLASH 64 KiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_FLASH_64K;
                    break;
                case "FLASH 128 KiB":
                    type = WorkerArgs.OperationType.DOWNLOAD_FLASH_128K;
                    break;
                default:
                    throw new ArgumentException("Invalid ROM Download Option selected");
            }
            WorkerArgs wa = new WorkerArgs(file, type);
            if (transceiver.IsBusy)
                return;
            lockGUI();
            transceiver.RunWorkerAsync(wa);
        }

        private void usButtonGo_Click(object sender, EventArgs e)
        {
            string file = usTextBox.Text;
            WorkerArgs.OperationType type;
            switch (usComboBox.Text)
            {
                case "Auto":
                    type = WorkerArgs.OperationType.UPLOAD_AUTO_SAVE;
                    break;
                case "EEPROM 512 B":
                    type = WorkerArgs.OperationType.UPLOAD_512_EEPROM;
                    break;
                case "EEPROM 8 KiB":
                    type = WorkerArgs.OperationType.UPLOAD_8K_EEPROM;
                    break;
                case "SRAM 32 KiB":
                    type = WorkerArgs.OperationType.UPLOAD_SRAM_32K;
                    break;
                case "FLASH 64 KiB":
                    type = WorkerArgs.OperationType.UPLOAD_FLASH_64K;
                    break;
                case "FLASH 128 KiB":
                    type = WorkerArgs.OperationType.UPLOAD_FLASH_128K;
                    break;
                default:
                    throw new ArgumentException("Invalid ROM Download Option selected");
            }
            WorkerArgs wa = new WorkerArgs(file, type);
            if (transceiver.IsBusy)
                return;
            lockGUI();
            transceiver.RunWorkerAsync(wa);
        }

        private void refreshSerialButton_Click(object sender, EventArgs e)
        {
            refreshSerialPorts();
        }
    }
    class WorkerArgs
    {
        public WorkerArgs(string file, OperationType type)
        {
            this.file = file;
            this.type = type;
        }

        public enum OperationType
        {
            DOWNLOAD_AUTO_ROM,
            DOWNLOAD_4M_ROM,
            DOWNLOAD_8M_ROM,
            DOWNLOAD_16M_ROM,
            DOWNLOAD_32M_ROM,
            DOWNLOAD_AUTO_SAVE,
            DOWNLOAD_512_EEPROM,
            DOWNLOAD_8K_EEPROM,
            DOWNLOAD_SRAM_32K,
            DOWNLOAD_FLASH_64K,
            DOWNLOAD_FLASH_128K,
            UPLOAD_AUTO_SAVE,
            UPLOAD_512_EEPROM,
            UPLOAD_8K_EEPROM,
            UPLOAD_SRAM_32K,
            UPLOAD_FLASH_64K,
            UPLOAD_FLASH_128K,
        }
        
        public readonly string file;
        public readonly OperationType type;
    }

    class ProgressState
    {
        public ProgressState(int bytesPerSecond, int totalBytes)
        {
            this.bytesPerSecond = bytesPerSecond;
            this.totalBytes = totalBytes;
        }

        public readonly int bytesPerSecond;
        public readonly int totalBytes;
    }
}
