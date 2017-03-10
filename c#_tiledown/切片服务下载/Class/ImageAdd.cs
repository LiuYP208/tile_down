using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace 切片服务下载.Class
{
    public class ImageAdd
    {
        public ImageAdd()
        {
        }
        //横向添加图片
        public static void AddAllImageH(int z, string m_LoaclPath)
        {
            //根据层级数 计算 当前层需要多少图片

            int TotalNum = (z + 1) * (z + 1);
            List<string> m_ImgFileNameList = new List<string>();

            //创建新画布
            int BimpLength = (z + 1) * 256;
            Bitmap newBitMap = new Bitmap(BimpLength, BimpLength);

            Graphics g = Graphics.FromImage(newBitMap);
            //清除画布,背景设置为白色
            g.Clear(Color.Transparent);

            //遍历获取图片名称
            for (int x = 0; x < TotalNum; x++)
            {
                for (int y = 0; y < TotalNum; y++)
                {
                    string FileName = m_LoaclPath + String.Format("x={0}&y={1}&z={2}.png", x, y, z);
                    m_ImgFileNameList.Add(FileName);
                    // Image ResourceImage = Image.FromFile(ImageNameList.ElementAt(i));
                    Bitmap ResourceImage = (Bitmap)Bitmap.FromFile(FileName);
                    //根据xy计算 当前
                    g.DrawImage(ResourceImage, 256 * x, 256 * y, 256, 256);
                }
            }
            //
            string m_SaveName = m_LoaclPath + "\\Total.png";

            newBitMap.Save(m_SaveName, ImageFormat.Png);
            

        }

        

    }
}
