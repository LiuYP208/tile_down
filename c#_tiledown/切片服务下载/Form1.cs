using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using 切片服务下载.Class;
using System.Threading;
using System.IO;

namespace 切片服务下载
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ShowSet();
            ShowSetTile();
            setWMSSet();
        }
        //http://mt2.google.cn/vt/lyrs=h&hl=zh-CN&gl=cn
        //默认下载网址
        public static string m_BasicUrl = "http://mt2.google.cn/vt/lyrs=h&hl=zh-CN&gl=cn";

        public static string m_HostUrl = "mt2.google.cn";

        //下载本地路径
        public static string m_DownlaodPath = "D:\\img\\google";
        //最大层数
        public static int m_MaxLevel = 1;

        //显示配置
        public void ShowSet()
        {
            this.textBox1.Text = m_BasicUrl;
            this.textBox2.Text = m_DownlaodPath;
            this.textBox3.Text = m_MaxLevel.ToString();
            this.textBox4.Text = m_HostUrl;
        }

        //显示配置
        public void LoadSet()
        {
            try
            {
                m_BasicUrl = this.textBox1.Text;
                m_DownlaodPath = this.textBox2.Text;
                m_MaxLevel = Convert.ToInt32(this.textBox3.Text);
                m_HostUrl = this.textBox4.Text;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        //点击下载事件
        private void btn_downlaod_Click(object sender, EventArgs e)
        {
            LoadSet();
            for (int z = 0; z <= m_MaxLevel; z++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(GetXY));
                t.Start(z);

            }
            MessageBox.Show("异步下载开始!");
        }

        private static void GetXY(object oz)
        {
            int z = Convert.ToInt32(oz);
            Console.WriteLine("线程开启，层数：" + z);

            int Max = Convert.ToInt32(Math.Pow(2, z) - 1);
            for (int x = 0; x <= Max; x++)
            {
                for (int y = 0; y <= Max; y++)
                {
                    ImageClass.DownLoadImageClass(m_BasicUrl, x, y, z, m_DownlaodPath, m_HostUrl);
                }
            }
            Console.WriteLine(String.Format("第{0}层下载完毕", z));
        }

        #region Tile部分

        //http://mt2.google.cn/vt/lyrs=h&hl=zh-CN&gl=cn
        //默认下载网址
        public static string m_BasicUrlTile = "http://www.scgis.net.cn/iMap/iMapServer/defaultRest/services/newtianditudom_scann/tile/";

        public static string m_HostUrlTile = "www.scgis.net.cn";

        //下载本地路径
        public static string m_DownlaodPathTile = "D:\\img\\Tile";

        //最大层数
        public static int m_MaxLevelTile = 1;


        public delegate void LoadDataDelegate(List<XYZFile> m_List);

        //点击进行合并
        private void button1_Click(object sender, EventArgs e)
        {
            LoadSetTile();
            int TotalCount = 0;
            //线程数
            int ThreadNum = 100;
            List<XYZFile> m_XYZFileList = new List<XYZFile>();
            for (int z = 0; z <= m_MaxLevelTile; z++)
            {
                int xMax = Convert.ToInt32(Math.Pow(2, z) - 1);
                int yMax = Convert.ToInt32(Math.Pow(2, z - 1) - 1);

                for (int x = 0; x <= xMax; x++)
                {
                    for (int y = 0; y <= yMax; y++)
                    {
                        TotalCount++;
                        XYZFile m_XYZFileModel = new XYZFile() { x = x, y = y, z = z, id = TotalCount };
                        m_XYZFileList.Add(m_XYZFileModel);
                    }
                }
            }
            foreach (XYZFile m_Model in m_XYZFileList)
            {
                ImageClass.DownLoadImageTile(m_BasicUrlTile, m_Model.x, m_Model.y, m_Model.z, m_DownlaodPathTile);
            }

            //int ThreadTileNum = TotalCount / ThreadNum + 1;
            //for (int i = 0; i < ThreadNum; i++)
            //{
            //    var tmpList = m_XYZFileList.Where(s => (s.id >= i * ThreadTileNum) && (s.id <= (i + 1) * ThreadTileNum)).ToList();
            //    if (tmpList.Count > 0)
            //    {
            //        LoadDataDelegate m_del = GetXYTile;
            //        m_del.BeginInvoke(tmpList, null, null);
            //    }
            //}
            MessageBox.Show("异步下载开始!");
        }

        private static void GetXYTile(List<XYZFile> m_List)
        {
            foreach (XYZFile m_Model in m_List)
            {
                ImageClass.DownLoadImageTile(m_BasicUrlTile, m_Model.x, m_Model.y, m_Model.z, m_DownlaodPathTile);
            }
        }

        //显示配置
        public void ShowSetTile()
        {
            this.txt_tile1.Text = m_BasicUrlTile;
            this.txt_tile2.Text = m_HostUrlTile;
            this.txt_tile3.Text = m_DownlaodPathTile;
            this.txt_tile4.Text = m_MaxLevelTile.ToString();
        }

        //显示配置
        public void LoadSetTile()
        {
            try
            {
                m_BasicUrlTile = this.txt_tile1.Text;
                m_HostUrlTile = this.txt_tile2.Text;
                m_DownlaodPathTile = this.txt_tile3.Text;
                m_MaxLevelTile = Convert.ToInt32(this.txt_tile4.Text);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        #endregion

        #region 服务器下载

        public static string m_URLPathWMS = "http://www.scgis.net.cn/imap/iMapServer/defaultRest/services/newtianditudom_scann/WMS?LAYERS=0&TRANSPARENT=TRUE&ISBASELAYER=false&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&FORMAT=image%2Fpng&SRS=EPSG%3A4326&BBOX=" + "xleft" + "," + "ytop" + "," + "xright" + "," + "yright" + "&WIDTH=256&HEIGHT=256";
        public static string m_HostWMS = "www.scgis.net.cn";
        public static int m_MaxLevelWMS = 1;
        public static string m_LocalPathWMS = "D:\\img\\WMS_20160820";

        public void getWMSSet()
        {
            m_URLPathWMS = WMS1.Text;
            m_HostWMS = WMS2.Text;
            m_MaxLevelWMS = Convert.ToInt32(WMS3.Text);
            m_LocalPathWMS = WMS4.Text;
        }

        public void setWMSSet()
        {
            WMS1.Text = m_URLPathWMS;
            WMS2.Text = m_HostWMS;
            WMS3.Text = m_MaxLevelWMS.ToString();
            WMS4.Text = m_LocalPathWMS;
            WMS1.Enabled = false;
            WMS2.Enabled = false;
        }



        //点击进行下载
        private void button2_Click(object sender, EventArgs e)
        {
            //WMSTileDownLoadClass

            getWMSSet();
            int TotalCount = 0;
            List<XYZFile> m_XYZFileList = new List<XYZFile>();

            for (int z = 0; z <= m_MaxLevelWMS; z++)
            {
                //if (z != 5)
                //{
                //    continue;
                //}
                int xMax = 0;
                int yMax = 0;
                switch (z)
                {
                    case 0: { xMax = 2; yMax = 1; break; }
                    case 1: { xMax = 4; yMax = 2; break; }
                    case 2: { xMax = 8; yMax = 4; break; }
                    case 3: { xMax = 16; yMax = 8; break; }
                    case 4: { xMax = 32; yMax = 16; break; }
                    case 5: { xMax = 72; yMax = 36; break; }
                    case 6: { xMax = 144; yMax = 72; break; }
                    case 7: { xMax = 288; yMax = 144; break; }
                    case 8: { xMax = 576; yMax = 288; break; }

                    default: break;
                }

                for (int x = 0; x < xMax; x++)
                {
                    for (int y = 0; y < yMax; y++)
                    {
                        string m_ImgName = String.Format("{0}.png", y);
                        string m_Path = String.Format("{0}\\{1}\\{2}\\{3}", m_LocalPathWMS, z, x, m_ImgName);
                        if (!File.Exists(m_Path))
                        {
                            TotalCount++;
                            XYZFile m_XYZFileModel = new XYZFile() { x = x, y = y, z = z, id = TotalCount };
                            m_XYZFileList.Add(m_XYZFileModel);


                        }
                        else
                        {
                            Console.WriteLine("文件已经存在:" + m_Path);
                        }

                        // WMSTileDownLoadClass.DownLoadImageWMS(m_URLPathWMS, x, y, z, m_LocalPathWMS);
                    }
                }

            }

            int ThreadTileNum = TotalCount / 30 + 1;

            
            for (int i = 0; i < 30; i++)
            {
                var tmpList = m_XYZFileList.Where(s => (s.id >= i * ThreadTileNum) && (s.id <= (i + 1) * ThreadTileNum)).ToList();
                if (tmpList.Count > 0)
                {

                    LoadDataDelegate m_del = GetXYWMS;
                    m_del.BeginInvoke(tmpList, null, null);
                }
            }
            MessageBox.Show("多线程下载开始,总数：" + TotalCount);

        }


        private static void GetXYWMS(List<XYZFile> m_List)
        {
            foreach (XYZFile m_Model in m_List)
            {
                WMSTileDownLoadClass.DownLoadImageWMS(m_URLPathWMS, m_Model.x, m_Model.y, m_Model.z, m_LocalPathWMS);
            }
        }


        #endregion
    }
    public class XYZFile
    {
        public XYZFile()
        {
        }
        public int id;
        public int x;
        public int y;
        public int z;
    }
}
