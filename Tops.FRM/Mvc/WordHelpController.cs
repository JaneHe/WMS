using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace Tops.FRM.Mvc
{
    public class WordHelpController : Controller
    {
        public ActionResult Download()
        {
            try
            {
                string doc = AppDomain.CurrentDomain.BaseDirectory + "temp.doc";    //生成后的doc 

                string temp = AppDomain.CurrentDomain.BaseDirectory + "temp.dot";    //生成后的doc 

                System.IO.File.Delete(doc);
                WordHelp word = new WordHelp();
                word.CreateNewDocument(temp);
                  

                word.InsertValue("项目", "P2P跑路计划");
                word.InsertValue("招商负责人", "张山");
                word.InsertValue("书签", DateTime.Now.ToString());
                word.InsertValue("版本", "v1.0");

                Range r = word.Application.Selection.Range.Next().Next().Next().Next().Next().Next();

                word.Document.Bookmarks.Add("真的", r);

                //word.Application.ActiveWindow.ActivePane.Selection.InsertAfter("[页眉内容]");

                //word.InsertTable("招商负责人", 2, 2, 500);

                word.InsertValue("真的", "娃娃"); 

                //word.Document.Tables[1].Cell(1,1).Range.Text.ToString()  //获取 

                word.SaveDocument(doc);
            }
            catch (Exception ex)
            {
                WordHelp.killWinWord();
                throw;
            }
            return View();
        }
    }

    /// <summary>
    /// 文档帮助类
    /// </summary>
    public class WordHelp
    {
        private Application wordApp = null;
        private Document wordDoc = null;
        public Application Application
        {
            get
            {
                return wordApp;
            }
            set
            {
                wordApp = value;
            }
        }
        public Document Document
        {
            get
            {
                return wordDoc;
            }
            set
            {
                wordDoc = value;
            }
        }

        /// <summary>
        /// 通过模板创建新文档
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateNewDocument(string filePath)
        {
            try
            {
                killWinWordProcess();
                wordApp = new ApplicationClass();
                wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                wordApp.Visible = false;
                object missing = System.Reflection.Missing.Value;
                object templateName = filePath;
                wordDoc = wordApp.Documents.Open(ref templateName, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 保存新文件
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveDocument(string filePath)
        {
            object fileName = filePath;
            object format = WdSaveFormat.wdFormatDocument;//保存格式
            object miss = System.Reflection.Missing.Value;
            wordDoc.SaveAs(ref fileName, ref format, ref miss,
                ref miss, ref miss, ref miss, ref miss,
                ref miss, ref miss, ref miss, ref miss,
                ref miss, ref miss, ref miss, ref miss,
                ref miss);
            //关闭wordDoc，wordApp对象
            object SaveChanges = WdSaveOptions.wdSaveChanges;
            object OriginalFormat = WdOriginalFormat.wdOriginalDocumentFormat;
            object RouteDocument = false;
            wordDoc.Close(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
            wordApp.Quit(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
        }

        /// <summary>
        /// 在书签处插入值
        /// </summary>
        /// <param name="bookmark"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InsertValue(string bookmark, string value)
        {
            object bkObj = bookmark;
            if (wordApp.ActiveDocument.Bookmarks.Exists(bookmark))
            {
                wordApp.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
                wordApp.Selection.TypeText(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入表格,bookmark书签
        /// </summary>
        /// <param name="bookmark"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public Table InsertTable(string bookmark, int rows, int columns, float width)
        {
            object miss = System.Reflection.Missing.Value;
            object oStart = bookmark;
            Range range = wordDoc.Bookmarks.get_Item(ref oStart).Range;//表格插入位置

            Table newTable = wordDoc.Tables.Add(range, rows, columns, ref miss, ref miss);
            //设置表的格式
            newTable.Borders.Enable = 1;  //允许有边框，默认没有边框(为0时报错，1为实线边框，2、3为虚线边框，以后的数字没试过)
            newTable.Borders.OutsideLineWidth = WdLineWidth.wdLineWidth050pt;//边框宽度
            if (width != 0)
            {
                newTable.PreferredWidth = width;//表格宽度
            }
            newTable.AllowPageBreaks = false;
            return newTable;
        }

        /// <summary>
        /// 合并单元格 表id,开始行号,开始列号,结束行号,结束列号
        /// </summary>
        /// <param name="n"></param>
        /// <param name="row1"></param>
        /// <param name="column1"></param>
        /// <param name="row2"></param>
        /// <param name="column2"></param>
        public void MergeCell(int n, int row1, int column1, int row2, int column2)
        {
            wordDoc.Content.Tables[n].Cell(row1, column1).Merge(wordDoc.Content.Tables[n].Cell(row2, column2));
        }

        /// <summary>
        /// 合并单元格 表名,开始行号,开始列号,结束行号,结束列号
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row1"></param>
        /// <param name="column1"></param>
        /// <param name="row2"></param>
        /// <param name="column2"></param>
        public void MergeCell(Microsoft.Office.Interop.Word.Table table, int row1, int column1, int row2, int column2)
        {
            table.Cell(row1, column1).Merge(table.Cell(row2, column2));
        }

        /// <summary>
        /// 设置表格内容对齐方式 Align水平方向，Vertical垂直方向(左对齐，居中对齐，右对齐分别对应Align和Vertical的值为-1,0,1)Microsoft.Office.Interop.Word.Table table
        /// </summary>
        /// <param name="n"></param>
        /// <param name="Align"></param>
        /// <param name="Vertical"></param>
        public void SetParagraph_Table(int n, int Align, int Vertical)
        {
            switch (Align)
            {
                case -1: wordDoc.Content.Tables[n].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; break;//左对齐
                case 0: wordDoc.Content.Tables[n].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; break;//水平居中
                case 1: wordDoc.Content.Tables[n].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; break;//右对齐
            }
            switch (Vertical)
            {
                case -1: wordDoc.Content.Tables[n].Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop; break;//顶端对齐
                case 0: wordDoc.Content.Tables[n].Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter; break;//垂直居中
                case 1: wordDoc.Content.Tables[n].Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom; break;//底端对齐
            }
        }

        /// <summary>
        /// 设置单元格内容对齐方式
        /// </summary>
        /// <param name="n"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="Align"></param>
        /// <param name="Vertical"></param>
        public void SetParagraph_Table(int n, int row, int column, int Align, int Vertical)
        {
            switch (Align)
            {
                case -1: wordDoc.Content.Tables[n].Cell(row, column).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; break;//左对齐
                case 0: wordDoc.Content.Tables[n].Cell(row, column).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; break;//水平居中
                case 1: wordDoc.Content.Tables[n].Cell(row, column).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; break;//右对齐
            }
            switch (Vertical)
            {
                case -1: wordDoc.Content.Tables[n].Cell(row, column).Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop; break;//顶端对齐
                case 0: wordDoc.Content.Tables[n].Cell(row, column).Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter; break;//垂直居中
                case 1: wordDoc.Content.Tables[n].Cell(row, column).Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom; break;//底端对齐
            }

        }

        /// <summary>
        /// 设置表格字体
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fontName"></param>
        /// <param name="size"></param>
        public void SetFont_Table(Microsoft.Office.Interop.Word.Table table, string fontName, double size)
        {
            if (size != 0)
            {
                table.Range.Font.Size = Convert.ToSingle(size);
            }
            if (fontName != "")
            {
                table.Range.Font.Name = fontName;
            }
        }

        /// <summary>
        /// 设置单元格字体
        /// </summary>
        /// <param name="n"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="fontName"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        public void SetFont_Table(int n, int row, int column, string fontName, double size, int bold)
        {
            if (size != 0)
            {
                wordDoc.Content.Tables[n].Cell(row, column).Range.Font.Size = Convert.ToSingle(size);
            }
            if (fontName != "")
            {
                wordDoc.Content.Tables[n].Cell(row, column).Range.Font.Name = fontName;
            }
            wordDoc.Content.Tables[n].Cell(row, column).Range.Font.Bold = bold;// 0 表示不是粗体，其他值都是
        }

        // 是否使用边框,n表格的序号,use是或否
        // 该处边框参数可以用int代替bool可以让方法更全面
        // 具体值方法中介绍
        public void UseBorder(int n, bool use)
        {
            if (use)
            {
                wordDoc.Content.Tables[n].Borders.Enable = 1;
                //允许有边框，默认没有边框(为0时无边框，1为实线边框，2、3为虚线边框，以后的数字没试过)
            }
            else
            {
                wordDoc.Content.Tables[n].Borders.Enable = 0;
            }
        }

        // 给表格插入一行,n表格的序号从1开始记
        public void AddRow(int n)
        {
            object miss = System.Reflection.Missing.Value;
            wordDoc.Content.Tables[n].Rows.Add(ref miss);
        }

        // 给表格添加一行
        public void AddRow(Microsoft.Office.Interop.Word.Table table)
        {
            object miss = System.Reflection.Missing.Value;
            table.Rows.Add(ref miss);
        }

        // 给表格插入rows行,n为表格的序号
        public void AddRow(int n, int rows)
        {
            object miss = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Table table = wordDoc.Content.Tables[n];
            for (int i = 0; i < rows; i++)
            {
                table.Rows.Add(ref miss);
            }
        }

        /// <summary>
        /// 删除表格第rows行,n为表格的序号
        /// </summary>
        /// <param name="n"></param>
        /// <param name="row"></param>
        public void DeleteRow(int n, int row)
        {
            Microsoft.Office.Interop.Word.Table table = wordDoc.Content.Tables[n];
            table.Rows[row].Delete();
        }

        /// <summary>
        /// 给表格中单元格插入元素，table所在表格，row行号，column列号，value插入的元素
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void InsertCell(Microsoft.Office.Interop.Word.Table table, int row, int column, string value)
        {
            table.Cell(row, column).Range.Text = value;
        }

        /// <summary>
        /// 给表格中单元格插入元素，n表格的序号从1开始记，row行号，column列号，value插入的元素
        /// </summary>
        /// <param name="n"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void InsertCell(int n, int row, int column, string value)
        {
            wordDoc.Content.Tables[n].Cell(row, column).Range.Text = value;
        }

        /// <summary>
        /// 给表格插入一行数据，n为表格的序号，row行号，columns列数，values插入的值
        /// </summary>
        /// <param name="n"></param>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        public void InsertCell(int n, int row, int columns, string[] values)
        {
            Microsoft.Office.Interop.Word.Table table = wordDoc.Content.Tables[n];
            for (int i = 0; i < columns; i++)
            {
                table.Cell(row, i + 1).Range.Text = values[i];
            }
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="bookmark"></param>
        /// <param name="picturePath"></param>
        /// <param name="width"></param>
        /// <param name="hight"></param>
        public void InsertPicture(string bookmark, string picturePath, float width, float hight)
        {
            object miss = System.Reflection.Missing.Value;
            object oStart = bookmark;
            Object linkToFile = false;       //图片是否为外部链接
            Object saveWithDocument = true;  //图片是否随文档一起保存 
            object range = wordDoc.Bookmarks.get_Item(ref oStart).Range;//图片插入位置
            wordDoc.InlineShapes.AddPicture(picturePath, ref linkToFile, ref saveWithDocument, ref range);
            wordDoc.Application.ActiveDocument.InlineShapes[1].Width = width;   //设置图片宽度
            wordDoc.Application.ActiveDocument.InlineShapes[1].Height = hight;  //设置图片高度
        }

        /// <summary>
        /// 插入一段文字,text为文字内容
        /// </summary>
        /// <param name="bookmark"></param>
        /// <param name="text"></param>
        public void InsertText(string bookmark, string text)
        {
            object oStart = bookmark;
            object range = wordDoc.Bookmarks.get_Item(ref oStart).Range;
            Paragraph wp = wordDoc.Content.Paragraphs.Add(ref range);
            wp.Format.SpaceBefore = 6;
            wp.Range.Text = text;
            wp.Format.SpaceAfter = 24;
            wp.Range.InsertParagraphAfter();
            wordDoc.Paragraphs.Last.Range.Text = "\n";
        }

        /// <summary>
        /// 杀掉winword.exe进程
        /// </summary>
        public void killWinWordProcess()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (System.Diagnostics.Process process in processes)
            {
                bool b = process.MainWindowTitle == "";
                if (process.MainWindowTitle == "")
                {
                    process.Kill();
                }
            }
        }
        public static void killWinWord()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (System.Diagnostics.Process process in processes)
            {
                bool b = process.MainWindowTitle == "";
                if (process.MainWindowTitle == "")
                {
                    process.Kill();
                }
            }
        }

        ////// <summary>
        ///// 把Word文件转换成pdf文件
        ///// </summary>
        ///// <param name="sourcePath">需要转换的文件路径和文件名称</param>
        ///// <param name="targetPath">转换完成后的文件的路径和文件名名称</param>
        ///// <returns>成功返回true,失败返回false</returns>
        //public static bool WordToPdf(object sourcePath, string targetPath)
        //{
        //    bool result = false;
        //    WdExportFormat wdExportFormatPDF = WdExportFormat.wdExportFormatPDF;
        //    object missing = Type.Missing;
        //    Microsoft.Office.Interop.Word.ApplicationClass applicationClass = null;
        //    Document document = null;
        //    try
        //    {
        //        applicationClass = new Microsoft.Office.Interop.Word.ApplicationClass();
        //        document = applicationClass.Documents.Open(ref sourcePath, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        //        if (document != null)
        //        {
        //            document.ExportAsFixedFormat(targetPath, wdExportFormatPDF, false, WdExportOptimizeFor.wdExportOptimizeForPrint, WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentContent, true, true, WdExportCreateBookmarks.wdExportCreateWordBookmarks, true, true, false, ref missing);
        //        }
        //        result = true;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        if (document != null)
        //        {
        //            document.Close(ref missing, ref missing, ref missing);
        //            document = null;
        //        }
        //        if (applicationClass != null)
        //        {
        //            applicationClass.Quit(ref missing, ref missing, ref missing);
        //            applicationClass = null;
        //        }
        //    }
        //    return result;
        //}

    }
}
