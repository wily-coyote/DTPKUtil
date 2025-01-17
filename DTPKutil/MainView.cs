﻿using System;
using System.IO;
using System.Windows.Forms;

namespace DTPKutil
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void btnPickInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AM2 DTPK Digital Track PacKage (*.snd)|*.snd|All Files (*.*)|*.*";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtInput.Text = ofd.FileName;
                //if (string.IsNullOrEmpty(txtOutput.Text))
                //{
                    txtOutput.Text = Path.GetDirectoryName(txtInput.Text) + "\\" + Path.GetFileNameWithoutExtension(txtInput.Text) + "\\";
                //}
            }
        }

        private void btnPickOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                fbd.SelectedPath = txtOutput.Text;
            }
            else if (!string.IsNullOrEmpty(txtInput.Text))
            {
                fbd.SelectedPath = Path.GetDirectoryName(txtInput.Text);
            }
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                txtOutput.Text = fbd.SelectedPath;
            }
        }

        private void btnReadInfo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInput.Text))
            {
                txtTrackInfo.Text = LoadTrack().PrintSamplesInfo();
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
	    if (!Directory.Exists(txtOutput.Text)){
		Directory.CreateDirectory(txtOutput.Text);
	    }

            if (!string.IsNullOrEmpty(txtInput.Text) && !string.IsNullOrEmpty(txtOutput.Text))
            {
                var track = LoadTrack();
                txtTrackInfo.Text = track.PrintSamplesInfo();
                for (int i = 0; i < track.Samples.Count; i++)
                {
                    string fileOut = Path.GetFileNameWithoutExtension(txtInput.Text) + "_" + (i + 1) + ".wav";
                    fileOut = Path.Combine(txtOutput.Text, fileOut);
                    File.WriteAllBytes(fileOut, WavUtil.AddWavHeader(track.GetSampleData(track.Samples[i], true, track.Is2018Format), track.Is2018Format ? (byte)32 : (byte)16));
                }
            }
        }

        private DtpkFile LoadTrack()
        {
            try
            {
                return new DtpkFile(File.ReadAllBytes(txtInput.Text));
            }
            catch (Exception ex)
            {
                txtTrackInfo.Text = "Bad File. Note that XBOX DTPK is not yet implemented.";
                MessageBox.Show($"Processing Failed: {ex.Message}");
            }
            return null;
        }

        private void BtnDecompressAs_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInput.Text))
            {
                DtpkFile track = LoadTrack();
                if (track != null && !track.Is2018Format)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "AM2 DTPK Digital Track PacKage (*.snd)|*.snd|All Files (*.*)|*.*";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        DtpkFile unpacked = track.Decompress(chk32bit.Checked);
                        File.WriteAllBytes(sfd.FileName, unpacked.FileData);
                        txtTrackInfo.Text = $"Original File:{Environment.NewLine}" + track.PrintSamplesInfo() +
                            $"{Environment.NewLine}{Environment.NewLine}Decompressed File:{Environment.NewLine}" +
                            unpacked.PrintSamplesInfo();
                    }
                }
                else
                {
                    MessageBox.Show("This track appears to be in the format used by the 2018 ports, which doesn't contain compressed samples. Nothing to decompress.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
