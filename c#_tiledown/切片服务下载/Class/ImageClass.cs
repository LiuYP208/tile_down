using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using System.Drawing.Imaging;

namespace 切片服务下载.Class
{
    public class ImageClass
    {
        public ImageClass()
        { }

        //将图片合并为整体图片
        public Bitmap AddClass(List<Bitmap> m_ImageList)
        {
            Bitmap Image1 = m_ImageList.ElementAt(0);


            Bitmap Image2 = m_ImageList.ElementAt(0);

            Bitmap Image3 = m_ImageList.ElementAt(0);
            Bitmap Image4 = m_ImageList.ElementAt(0);
            Bitmap Img = new Bitmap(400, 400);      //创建一张空白图片
            Graphics g = Graphics.FromImage(Img);   //从空白图片创建一个Graphics
            g.DrawImage(Image1, new Point(0, 0));   //把图片指定坐标位置并画到空白图片上面
            g.DrawImage(Image2, new Point(0, 0));   //坐标自己算
            g.DrawImage(Image3, new Point(0, 0));   //坐标自己算
            g.DrawImage(Image4, new Point(0, 0));   //坐标自己算
            return Img;
        }

        //将图片下载

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_Imageurl">http://mt2.google.cn/vt/lyrs=m&hl=zh-CN</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="m_LoaclPath"></param>
        public static void DownLoadImageClass(string m_Imageurl, int x, int y, int z, string m_LoaclPath, string m_HostName)
        {
            //          //http://mt2.google.cn/vt/lyrs=m&hl=zh-CN&gl=cn&x={x}&y=193&z=9
            string m_url = m_Imageurl + String.Format("&x={0}&y={1}&z={2}", x, y, z);
            string m_ImgName = String.Format("x={0}&y={1}&z={2}.png", x, y, z);

            string m_Path = String.Format("{0}\\{1}\\{2}", m_LoaclPath, z, m_ImgName);
            //if (!File.Exists(m_Path))
            //{
            //    File.Create(m_Path);
            //}


            string m_FilePath = String.Format("{0}\\{1}", m_LoaclPath, z);
            if (!Directory.Exists(m_FilePath))
            {
                Directory.CreateDirectory(m_FilePath);
            }
            Console.WriteLine("开始下载:" + m_Path);
            //若文件存在 不进行下载
            if (File.Exists(m_Path))
            {
                return;
            }


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_url);
            //request.Headers = new WebHeaderCollection();

            request.Method = "GET";
            //header 部分

            request.Host = m_HostName;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";


            // request.Connection = "keep-alive";
            request.ContentType = "image/png";
            request.Date = DateTime.Now;

            WebResponse response = request.GetResponse();
            System.IO.Stream resStream = response.GetResponseStream();

            Bitmap bmp = new Bitmap(resStream);
            MemoryStream bmpStream = new MemoryStream();
            bmp.Save(bmpStream, System.Drawing.Imaging.ImageFormat.Png);
            Image image = Image.FromStream(bmpStream);

            image.Save(m_Path, ImageFormat.Png);
            Console.WriteLine("下载完成:" + m_Path);
            bmpStream.Close();
            image.Dispose();
            bmp.Dispose();
            bmpStream.Dispose();

            return;
        }


        public static void DownLoadImageTile(string m_Imageurl, int x, int y, int z, string m_LoaclPath)
        {
            //http://www.scgis.net.cn/iMap/iMapServer/defaultRest/services/newtianditudom_scann/tile/4/8/15
            string m_url = m_Imageurl + String.Format("/{0}/{1}/{2}", z, y, x);
            string m_ImgName = String.Format("{0}.png", y);

            string m_Path = String.Format("{0}\\{1}\\{2}\\{3}", m_LoaclPath, z, x, m_ImgName);
            //if (!File.Exists(m_Path))
            //{
            //    File.Create(m_Path);
            //}


            string m_FilePath = String.Format("{0}\\{1}\\{2}", m_LoaclPath, z, x);
            if (!Directory.Exists(m_FilePath))
            {
                Directory.CreateDirectory(m_FilePath);
            }
            Console.WriteLine("开始下载:" + m_Path);
            //若文件存在 不进行下载
            if (File.Exists(m_Path))
            {
                Console.WriteLine("文件已经存在:" + m_Path);
                return;
            }

            //建立网络连接

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_url);
            request.Method = "GET";
            request.Host = "www.scgis.net.cn";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            request.ContentType = "image/png";
            request.Date = DateTime.Now;

            try
            {
                WebResponse response = request.GetResponse();
                Stream resStream = response.GetResponseStream();

                Bitmap bmp = new Bitmap(resStream);
                MemoryStream bmpStream = new MemoryStream();
                bmp.Save(bmpStream, System.Drawing.Imaging.ImageFormat.Png);
                Image image = Image.FromStream(bmpStream);

                image.Save(m_Path, ImageFormat.Png);
                Console.WriteLine("下载完成:" + m_Path);
                bmpStream.Close();
                image.Dispose();
                bmp.Dispose();
                bmpStream.Dispose();
            }
            catch
            {
                Console.WriteLine("下载异常:" + m_Path); 
                request.Abort();
            }
            finally
            {
              //  request.();
            }

            return;
        }

    }
}
