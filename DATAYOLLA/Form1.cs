using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml.Linq;

namespace DATAYOLLA
{
    public partial class Form1 : Form
    {
        const byte HEADER = 0xAA;
        private SerialPort serialPort1;

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            serialPort1 = new SerialPort
            {
                PortName = "COM16", 
                BaudRate = 115200, 
                Parity = Parity.None, 
                DataBits = 8, 
                StopBits = StopBits.One
            };
            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
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

                serialPort1.Write(txBuffer, 0, txindex);
                
            }
            else
            {
                MessageBox.Show("Port açık değil!");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
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

                serialPort1.Write(txBuffer, 0, txindex);
            }
            else
            {
                MessageBox.Show("Port açık değil!");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
