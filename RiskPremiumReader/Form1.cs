using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RiskPremiumReader
{
    public partial class Form1 : Form
    {
        private double[,] RiskPremium;

        public Form1()
        {
            InitializeComponent();
            string fileName = "riskpremium.dat";

            RiskPremium = new double[100, 101];

            if (!File.Exists(fileName))
            {
                MessageBox.Show("Data file missing!");
                return;
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader r = new BinaryReader(fs))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 101; j++)
                        {
                            RiskPremium[i, j] = r.ReadDouble();
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int tickets = 0;
            int.TryParse(ticketbox.Text, out tickets);
            int players = 0;
            int.TryParse(playerbox.Text, out players);
            if (tickets < 2 || players < 3 || tickets > 99 || players > 100 || tickets >= players) MessageBox.Show("Invalid input, try again!");
            else
            {
                result.Text = "Base Risk Premium is " + RiskPremium[tickets, players].ToString("P1");
            }
        }
    }
}