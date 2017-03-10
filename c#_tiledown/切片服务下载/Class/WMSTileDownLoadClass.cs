
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Drawing;

namespace 切片服务下载.Class
{
    public class WMSTileDownLoadClass
    {
        public static void DownLoadImageWMS(string m_Imageurl, int x, int y, int z, string m_LoaclPath)
        {
            //if (z != 5)
            //{
            //    return;
            //}
            // "http://www.scgis.net.cn/imap/iMapServer/defaultRest/services/newtianditudom_scann/WMS?LAYERS=0&TRANSPARENT=TRUE&ISBASELAYER=false&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&FORMAT=image%2Fpng&SRS=EPSG%3A4326&BBOX=" + "xleft" + "," + "ytop" + "," + "xright" + "," + "yright" + "&WIDTH=256&HEIGHT=256";

            Console.WriteLine(String.Format("z:{0}：x{1}：y{2}", z, x, y));

            double levelsize = 0;
            switch (z)
            {
                case 0: { levelsize = 180; break; }
                case 1: { levelsize = 90; break; }
                case 2: { levelsize = 45; break; }
                case 3: { levelsize = 22.5; break; }
                case 4: { levelsize = 11.25; break; }
                case 5: { levelsize = 5.625; break; }
                case 6: { levelsize = 2.5; break; }
                case 7: { levelsize = 1.25; break; }
                case 8: { levelsize = 0.625; break; }
                default: break;
            }

            double xleft = -180 + x * levelsize;

            double xright = -180 + (x + 1) * levelsize;

            double ytop = -90 + y * levelsize;

            double ybottom = -90 + (y + 1) * levelsize;

            string m_url = String.Format("http://www.scgis.net.cn/imap/iMapServer/defaultRest/services/newtianditudom_scann/WMS?LAYERS=0&TRANSPARENT=TRUE&ISBASELAYER=false&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&FORMAT=image%2Fpng&SRS=EPSG%3A4326&BBOX={0},{1},{2},{3}&WIDTH=256&HEIGHT=256",
                xleft, ytop, xright, ybottom);
            Console.WriteLine("网址:" + m_url);

            string m_ImgName = String.Format("{0}.png", y);

       
            string m_Path = String.Format("{0}\\{1}\\{2}\\{3}", m_LoaclPath, z, x, m_ImgName);


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

                image.Save(m_Path, System.Drawing.Imaging.ImageFormat.Png);
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
