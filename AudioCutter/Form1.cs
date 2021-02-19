using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using WMPLib;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NAudio;
using NAudio.Wave;
using DAudio;


namespace AudioCutter
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();          
        }
        
        WindowsMediaPlayer Player = new WindowsMediaPlayer();
        
        private void PlayFile(String url)
        {          
            Player.URL = url;   
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
            pictureBox2.Image = Image.FromFile(@"картинки\play1.png");
            pictureBox3.Image = Image.FromFile(@"картинки\stop1.png");
            button4.Enabled = false;
            groupBox3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            button1.Visible = false;
        }

        private NAudio.Wave.BlockAlignReductionStream stream = null;

        private NAudio.Wave.DirectSoundOut output = null;

        private void контактыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ОККТ Сервер \nВерста Андрей Ярославович inc. \nВсе права защищены,во время выполнения работы никто не пострадал","Контакты", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа была разработана на языке С# и выполняет следующие действия:\n" +
                "\n1)Обрезка аудио файлов\n" +
                "\n2)Склейка аудио файлов\n" +
                "\nПрежде всего вам необходимо загрузить один или несколько аудио файлов формата mp3, затем выполнять операции над файлами ", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        string[] mass = new string[10];
        int koll = 0;
        string[] files = new string[2];
        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (kolel >= 5)
                return;
            openFileDialog1.Filter = "Audio Files|*.mp3";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                string res = openFileDialog1.FileName;
                res = Path.GetFileName(res);
                checkedListBox1.Items.Add(res);
            }
                    mass[koll] = openFileDialog1.FileName;
                koll++;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(checkBox1.Checked==true&& checkBox2.Checked == true)
            {
                Combine(files, File.Create(
                        new string(DateTime.Now.ToString().Where(char.IsDigit).ToArray()) + ".mp3"));
                DisposeWave();    
            } 
        }
       
        private void Combine(string[] inputFiles, Stream output)
        {
            foreach (string file in inputFiles)
            {

                Mp3FileReader reader = new Mp3FileReader(file);
                if ((output.Position == 0) && (reader.Id3v2Tag != null))
                {
                    output.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
                }
                Mp3Frame frame;
                while ((frame = reader.ReadNextFrame()) != null)
                {
                    output.Write(frame.RawData, 0, frame.RawData.Length);
                }
                reader.Dispose();
            }
            MessageBox.Show("Файлы успешно соединены");
            //output.Close();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = Image.FromFile(@"картинки\audio.gif");
            int j=checkedListBox1.SelectedIndex;
            string jj = mass[j];
            PlayFile(jj);
            Player.controls.play();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = null;
            if (koll == 0)
                return;          
            Player.controls.pause();
        }
        private void DisposeWave()
        {
            //функция для очистки потоков при выходе из приложения
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                output.Dispose();
                output = null;
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
            Dispose();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //выход из приложения
            DisposeWave();
        }
        int z = 0;
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            z = checkedListBox1.CheckedItems.Count;
            //кол-во элементов
            if (z == 1)
            { button4.Enabled = true; }
            //koll=checkedListBox1.Items.Count;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.Items.Count < 1)
                return;
            else
            checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
        }
        int kolel = 0;
        string cursel1 = "";
        string[] str = new string[2];
        private void button4_Click(object sender, EventArgs e)
        {
            if (kolel >= 5)
                return;
            else
            {
                int[] m = new int[checkedListBox1.CheckedItems.Count];
                int ch = 0;
                if (z == 1)
                {
                    int j = checkedListBox1.SelectedIndex;
                    cursel1 = mass[j];
                    string jj = Path.GetFileName(mass[j]);
                    label1.Text = jj;
                    ch++;
                }
                else if (z == 2)
                {
                    for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                    {
                        files[i] = mass[i];
                        str[i] = checkedListBox1.CheckedItems[i].ToString();
                    }
                    ch++;
                    label1.Text = str[0];
                    label2.Text = str[1];
                }
                kolel++;
            }
        }
        bool chc = false;
        private void button3_Click(object sender, EventArgs e)
        {
            if (label1.Text == ""&& (checkBox1.Checked==false|| checkBox2.Checked == false)) { return; }
            groupBox3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            button1.Visible = true;
            if (checkBox1.Checked == true)
            {
                label8.Text = Path.GetFileName(mass[1]);
                chc = false;
            }
            if (checkBox2.Checked == true)
            {
                label8.Text = Path.GetFileName(mass[2]);
                chc = true;
            }
        }
        public void Trimm(string inputPath, string outputPath, TimeSpan? begin, TimeSpan? end)
        {
            if (begin.HasValue && end.HasValue && begin > end)
                throw new ArgumentOutOfRangeException("end", "end should be greater than begin");

            using (var reader = new Mp3FileReader(inputPath))
            using (var writer = File.Create(outputPath))
            {
                Mp3Frame frame;
                while ((frame = reader.ReadNextFrame()) != null)
                    if (reader.CurrentTime >= begin || !begin.HasValue)
                    {
                        if (reader.CurrentTime <= end || !end.HasValue)
                            writer.Write(frame.RawData, 0, frame.RawData.Length);
                        else break;
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var mp3Path = "";
            if (chc==false)
            {
                mp3Path = mass[0];
            }
            else
            {
                mp3Path = mass[1];
            }
            var outputPath = Path.ChangeExtension(mp3Path, ".trimmed.mp3");
            double a, b;
            a = double.Parse(textBox1.Text);
            b = double.Parse(textBox2.Text);
            Trimm(mp3Path, outputPath, TimeSpan.FromSeconds(a), TimeSpan.FromSeconds(b));
            MessageBox.Show("Файл успешно обрезан");
            
        }
    }
}