using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using OnBarcode.Barcode;
using System.Drawing.Imaging;
using Tops.FRM;
using System.Web;
using System.Web.Mvc;



namespace TopWMS.Controllers

{

    public partial class DMController : Controller

    {

        public string Index(string CustomerNo)

        {
            try
            {
                DataMatrix datamatrix = new DataMatrix();
                datamatrix.Data = CustomerNo;
                datamatrix.DataMode = DataMatrixDataMode.ASCII;
                datamatrix.FormatMode = DataMatrixFormatMode.Format_10X10;
                datamatrix.UOM = UnitOfMeasure.PIXEL;
                datamatrix.X = 10;
                datamatrix.LeftMargin = 10;
                datamatrix.RightMargin = 10;
                datamatrix.TopMargin = 10;
                datamatrix.BottomMargin = 10;
                datamatrix.Resolution = 720;
                datamatrix.Rotate = Rotate.Rotate0;
                datamatrix.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                //datamatrix.drawBarcode("D:/Program Files/沈阳名华-新版/TopsERP_MingHua/TopsERP_MingHua/Content/datamatrix.jpg");   //以保存特定格式方法生产二维码

                datamatrix.drawBarcode("G:/项目/HNAH/HNAH_WMS/WMS/TopsWMS/Content/datamatrix.jpg");   //以保存特定格式方法生产二维码
                //datamatrix.drawBarcode("F:/Tops_ShenYang_MingHua/Content/datamatrix.jpg");   //以保存特定格式方法生产二维码
                return "../../Content/datamatrix.jpg";
            }
            catch (Exception)
            {
                
                throw;
            }

        
        }

        public string Index2(string CustomerNo)
        {
            try
            {
                DataMatrix datamatrix = new DataMatrix();
                datamatrix.Data = CustomerNo;
                datamatrix.DataMode = DataMatrixDataMode.ASCII;
                datamatrix.FormatMode = DataMatrixFormatMode.Format_10X10;
                datamatrix.UOM = UnitOfMeasure.PIXEL;
                datamatrix.X = 10;
                datamatrix.LeftMargin = 10;
                datamatrix.RightMargin = 10;
                datamatrix.TopMargin = 10;
                datamatrix.BottomMargin = 10;
                datamatrix.Resolution = 720;
                datamatrix.Rotate = Rotate.Rotate0;
                datamatrix.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                //datamatrix.drawBarcode("D:/Program Files/沈阳名华-新版/TopsERP_MingHua/TopsERP_MingHua/Content/datamatrix2.jpg");   //以保存特定格式方法生产二维码

                datamatrix.drawBarcode("G:/项目/HNAH/HNAH_WMS/WMS/TopsWMS/Content/datamatrix2.jpg");   //以保存特定格式方法生产二维码
                //datamatrix.drawBarcode("F:/Tops_ShenYang_MingHua/Content/datamatrix2.jpg");   //以保存特定格式方法生产二维码
                return "../../Content/datamatrix2.jpg";

            }
            catch (Exception)
            {
                throw;
            }
        }
        public string Index3(string CustomerNo)
        {
            try
            {
                DataMatrix datamatrix = new DataMatrix();
                datamatrix.Data = CustomerNo;
                datamatrix.DataMode = DataMatrixDataMode.ASCII;
                datamatrix.FormatMode = DataMatrixFormatMode.Format_10X10;
                datamatrix.UOM = UnitOfMeasure.PIXEL;
                datamatrix.X = 10;
                datamatrix.LeftMargin = 10;
                datamatrix.RightMargin = 10;
                datamatrix.TopMargin = 10;
                datamatrix.BottomMargin = 10;
                datamatrix.Resolution = 720;
                datamatrix.Rotate = Rotate.Rotate0;
                datamatrix.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                //datamatrix.drawBarcode("D:/Program Files/沈阳名华-新版/TopsERP_MingHua/TopsERP_MingHua/Content/datamatrix3.jpg");   //以保存特定格式方法生产二维码

                datamatrix.drawBarcode("G:/项目/HNAH/HNAH_WMS/WMS/TopsWMS/Content/datamatrix3.jpg");   //以保存特定格式方法生产二维码
                //datamatrix.drawBarcode("F:/Tops_ShenYang_MingHua/Content/datamatrix3.jpg");   //以保存特定格式方法生产二维码
                return "../../Content/datamatrix3.jpg";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string Index4(string CustomerNo)
        {
            try
            {
                DataMatrix datamatrix = new DataMatrix();
                datamatrix.Data = CustomerNo;
                datamatrix.DataMode = DataMatrixDataMode.ASCII;
                datamatrix.FormatMode = DataMatrixFormatMode.Format_10X10;
                datamatrix.UOM = UnitOfMeasure.PIXEL;
                datamatrix.X = 10;
                datamatrix.LeftMargin = 10;
                datamatrix.RightMargin = 10;
                datamatrix.TopMargin = 10;
                datamatrix.BottomMargin = 10;
                datamatrix.Resolution = 720;
                datamatrix.Rotate = Rotate.Rotate0;
                datamatrix.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                //datamatrix.drawBarcode("D:/Program Files/沈阳名华-新版/TopsERP_MingHua/TopsERP_MingHua/Content/datamatrix4.jpg");   //以保存特定格式方法生产二维码

                datamatrix.drawBarcode("G:/项目/HNAH/HNAH_WMS/WMS/TopsWMS/Content/datamatrix4.jpg");   //以保存特定格式方法生产二维码
                //datamatrix.drawBarcode("F:/Tops_ShenYang_MingHua/Content/datamatrix4.jpg");   //以保存特定格式方法生产二维码
                return "../../Content/datamatrix4.jpg";

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}