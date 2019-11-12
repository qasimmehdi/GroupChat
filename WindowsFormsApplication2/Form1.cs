using System;
using System.Windows.Forms;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Linq;



namespace WindowsApplication2
{
    public partial class Form1 : Form
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        public Form1()
        {
            InitializeComponent();
        }

        string path = "";
        PictureBox[] boxes = new PictureBox[10];
        int pictureBoxCount = 0;

        private void button1_Click(object sender, EventArgs e)  //SEND
        {
            
            
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(imgToByteArray(path) + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)  //CONNECT
        {
            readData = "Conected to Chat Server ...";
            msg();
            clientSocket.Connect(textBox4.Text, 8888);
            serverStream = clientSocket.GetStream();

            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox3.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
        }

        private void getMessage()
        {
            
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[9999999];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;

                
                
                msg();
            }
        }

        private void msg()
        {
            
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                if (readData[0] == ',')
                {
                    readData = readData.TrimStart(',');
                    //MessageBox.Show(readData);
                    readData = readData.Substring(0, readData.IndexOf('$'));
                    byte[] imgBytes = Convert.FromBase64String(readData);
                    using (var ms = new MemoryStream(imgBytes))
                    {
                        if (pictureBoxCount == 10)
                        {
                            clearPicBoxes();
                            pictureBoxCount = 0;
                        }
                        boxes[pictureBoxCount].Image = Image.FromStream(ms);
                        pictureBoxCount++;
                        
                    }

                }
                else
                {
                    textBox1.Text = textBox1.Text + Environment.NewLine + readData;
                }
            }
        }

        private string imgToByteArray(string path)
        {
            Image image = Image.FromFile(path);
            var ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] bytes = File.ReadAllBytes(path);
            string str = Convert.ToBase64String(bytes);
            
            ms.Close();

            return str;
        }

        private void button3_Click(object sender, EventArgs e)  //SELECT IMG
        {
            button1.Enabled = true;

            using (OpenFileDialog file = new OpenFileDialog())
            {
                if (file.ShowDialog() == DialogResult.OK)
                {
                    path = file.FileName;
                }
            }
            textBox2.Text = path;
        }

        private void clearPicBoxes()
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].Image = null;
                //boxes[i].Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            boxes[0] = pictureBox1;
            boxes[1] = pictureBox2;
            boxes[2] = pictureBox3;
            boxes[3] = pictureBox4;
            boxes[4] = pictureBox5;
            boxes[5] = pictureBox6;
            boxes[6] = pictureBox7;
            boxes[7] = pictureBox8;
            boxes[8] = pictureBox9;
            boxes[9] = pictureBox10;
        }
    }
}