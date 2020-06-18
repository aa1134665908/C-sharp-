using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServerHost.parentForm = this;//初始化变量
            ServerHost.Start();//监听开始
            new Task(() => ServerHost.Listen()).Start();//利用Task创建多线程
            ServerHost.EventImageRecieved += ViewerForm_UpdatePicture;//给回调参数赋值
            
            
            
        }
        public void ViewerForm_UpdatePicture(object source, ServerEventArgs args)
        {
            this.pictureBox1.Image = args.Image;//自己的图片控件等于回调的图
        }
    }
}
