using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace JGP1
{

    class Program
    {

        public static ImageCodecInfo GetEncoder(ImageFormat format)
    {
    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
    foreach (ImageCodecInfo codec in codecs)
    {
        if (codec.FormatID == format.Guid)
            return codec;
    }
    return null;
    }
        static void Main(string[] args)
        {

Bitmap im = new Bitmap(@"C:\Users\Administrator\Desktop\1.bmp");
//转成jpg
var eps = new EncoderParameters(1);
var ep = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);
eps.Param[0] = ep;
var jpsEncodeer = GetEncoder(ImageFormat.Jpeg);
//保存图片
var imgurl = "\\"+Guid.NewGuid().ToString()+ ".jpg";
im.Save(Path.GetDirectoryName(@"C:\Users\Administrator\Desktop\无标题.bmp")+ imgurl, jpsEncodeer, eps);
//释放资源

       
im.Dispose();
ep.Dispose();
eps.Dispose();
        }
    }
}
