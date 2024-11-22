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
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace ZAMANLAYICI
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;

        #region Delegates
        public delegate void UPDATE_SECOND_TEXT(string Second);
        public delegate void UPDATE_MINUTE_TEXT(string Minute);
        public delegate void UPDATE_HOUR_TEXT(string Hour);
        #endregion

        #region
        public void UpdateHourText(string hour)
        {
            lbSaat.Text = hour;
        }
        public void UpdateMinuteText(string minute)
        {
            lbDakika.Text = minute;
        }
        public void UpdateSecondText(string second)
        {
            lbSaniye.Text = second;
        }
        #endregion

        const byte HEADER = 0xAA;
        private const int DATA_LENGTH = 3;
        private byte[] rxBuffer = new byte[7];
        private byte[] txBuffer = new byte[7];
        #region
        void SerialOnReciveHandler(object sender, SerialDataReceivedEventArgs e)

        {
            int bytesToRead = serialPort.BytesToRead;
            byte[] receivedData = new byte[bytesToRead];
            serialPort.Read(receivedData, 0, bytesToRead);

            int rxindex = 0;

            while (rxindex < receivedData.Length && receivedData[rxindex] != HEADER)
            {
                rxindex++;
            }

            if (rxindex >= receivedData.Length) return;

            rxBuffer[0] = HEADER;

            if (rxindex + 1 >= receivedData.Length) return;
            rxBuffer[1] = receivedData[rxindex + 1];

            if (rxindex + 2 >= receivedData.Length) return;
            byte dataLength = receivedData[rxindex + 2];

            if (dataLength != 3) return;

            rxBuffer[2] = dataLength;

            if (rxindex + 5 >= receivedData.Length) return;
            rxBuffer[3] = receivedData[rxindex + 3];
            rxBuffer[4] = receivedData[rxindex + 4];
            rxBuffer[5] = receivedData[rxindex + 5];

            if (rxindex + 6 >= receivedData.Length) return;
            byte checksum = receivedData[rxindex + 6];

            byte calculatedChecksum = (byte)(rxBuffer[1] + rxBuffer[2] + rxBuffer[3] + rxBuffer[4] + rxBuffer[5]);
            if (checksum != calculatedChecksum) return;

            rxBuffer[6] = checksum;

            string hour = rxBuffer[5].ToString("D2");
            if (lbSaat.InvokeRequired)
            {
                lbSaat.Invoke(new Action(() => lbSaat.Text = hour));
            }
            else
            {
                lbSaat.Text = hour;
            }

            string minute = rxBuffer[4].ToString("D2");
            if (lbDakika.InvokeRequired)
            {
                lbDakika.Invoke(new Action(() => lbDakika.Text = minute));
            }
            else
            {
                lbSaat.Text = minute;
            }

            string second = rxBuffer[3].ToString("D2");
            if (lbSaniye.InvokeRequired)
            {
                lbSaniye.Invoke(new Action(() => lbSaniye.Text = second));
            }
            else
            {
                lbSaniye.Text = second;
            }

        }
        #endregion

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort
            {
                PortName = "COM16",
                BaudRate = 115200,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReceivedBytesThreshold = 1,
                Handshake = Handshake.None,
                WriteTimeout = 3000,
            };
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
            serialPort.DataReceived += SerialOnReciveHandler;
        }

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            base.OnFormClosing(e);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                byte[] txBuffer = new byte[10];
                byte[] txData = new byte[5];

                byte txChecksum = 0;
                byte txindex = 0;
                byte txDataLength = 3;
                byte commandStart = 0x01;

                txBuffer[txindex++] = HEADER;
                txBuffer[txindex++] = commandStart;
                txBuffer[txindex++] = txDataLength;

                for (int i = 0; i < txDataLength && i < txData.Length; i++)
                {
                    txBuffer[txindex++] = txData[i];
                }

                for (int i = 1; i < txindex; i++)
                {
                    txChecksum += txBuffer[i];
                }

                txBuffer[txindex++] = txChecksum;

                serialPort.Write(txBuffer, 0, txindex);

            }
            else
            {
                MessageBox.Show("Port açık değil!");
            }
        }
        
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                byte[] txBuffer = new byte[10];
                byte[] txData = new byte[5];

                byte txChecksum = 0;
                byte txindex = 0;
                byte txDataLength = 3;
                byte commandStop = 0x02;

                txBuffer[txindex++] = HEADER;
                txBuffer[txindex++] = commandStop;
                txBuffer[txindex++] = txDataLength;

                for (int i = 0; i < txDataLength && i < txData.Length; i++)
                {
                    txBuffer[txindex++] = txData[i];
                }

                for (int i = 1; i < txindex; i++)
                {
                    txChecksum += txBuffer[i];
                }

                txBuffer[txindex++] = txChecksum;

                serialPort.Write(txBuffer, 0, txindex);
            }
            else
            {
                MessageBox.Show("Port açık değil!");
            }
        }
    }
}
