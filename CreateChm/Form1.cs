using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using ChmHelper;

namespace CreateChm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 全局变量

        string startPath = string.Empty;

        string hhcFile = @"C:\Program Files (x86)\HTML Help Workshop\hhc.exe";

        public string _defaultTopic = "index.html";

        StreamWriter streamWriter;

        string hhc = "index.hhc";

        string hhk = "index.hhk";

        string hhp = "index.hhp";

        //编译的HTML
        //string compileHTML = @"F:\Desktop\Desktop\Desktop\Code\VTSBlog\VTS.Web\CHM\日韩演员\28.html";
        string compileHTML = @"F:\Desktop\Desktop\Desktop\Code\CreateChm\CreateChm\bin\Debug\html\index.html";

        #endregion

        #region 开始创建
        private void button1_Click(object sender, EventArgs e)
        {
            startPath = Application.StartupPath;

            _defaultTopic = compileHTML;

            CreateHHC();

            CreateHHK();

            CreateHHP("我的CHM");

            if (Compile())
            {
                MessageBox.Show("Success,Pls Check!");
            }
        }
        #endregion

        #region 创建HHC文件
        private void CreateHHC()
        {
            using (FileStream fs = new FileStream(hhc, FileMode.Create, FileAccess.Write))
            {
                using (streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("GB18030")))
                {
                    streamWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">");
                    streamWriter.WriteLine("<HTML>");
                    streamWriter.WriteLine("<HEAD>");
                    streamWriter.WriteLine("<meta name=\"GENERATOR\" content=\"Microsoft&reg; HTML Help Workshop 4.1\">");
                    streamWriter.WriteLine("<!-- Sitemap 1.0 -->");
                    streamWriter.WriteLine("</HEAD>");

                    streamWriter.WriteLine("<BODY>");

                    streamWriter.WriteLine("<OBJECT type=\"text/site properties\">");
                    streamWriter.WriteLine("<param name=\"Window Styles\" value=\"0x237\">");
                    streamWriter.WriteLine("</OBJECT>");

                    #region 数据源

                    streamWriter.WriteLine("<UL>");

                    streamWriter.WriteLine("<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("<param name=\"Name\" value=\"Index\">");//表示目录
                    streamWriter.WriteLine("</OBJECT>");

                    streamWriter.WriteLine("<UL>");

                    streamWriter.WriteLine("<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("<param name=\"Name\" value=\"Index\">");
                    streamWriter.WriteLine("<param name=\"Local\" value=\"" + compileHTML + "\">");
                    streamWriter.WriteLine("</OBJECT>");

                    /*************************/

                    streamWriter.WriteLine("<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("<param name=\"Name\" value=\"201\">");
                    streamWriter.WriteLine("<param name=\"Local\" value=\"F:\\Desktop\\Desktop\\Desktop\\Code\\CreateChm\\CreateChm\\bin\\Debug\\html\\日韩演员\\201.html\">");
                    streamWriter.WriteLine("</OBJECT>");

                    streamWriter.WriteLine("</UL>");

                    streamWriter.WriteLine("</UL>");
                    /*************************/
                    #endregion
                    //TC();

                    streamWriter.WriteLine("</BODY>");
                    streamWriter.WriteLine("</HTML>");
                    streamWriter.WriteLine();
                }
            }
        }
        #endregion

        #region 创建HHK文件
        private void CreateHHK()
        {
            using (FileStream fs = new FileStream(string.Concat(startPath, "\\", hhk), FileMode.Create, FileAccess.Write))
            {
                using (streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("GB18030")))//System.Text.Encoding.GetEncoding("GB18030"));
                {
                    streamWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">");
                    streamWriter.WriteLine("<HTML>");
                    streamWriter.WriteLine("<HEAD>");
                    streamWriter.WriteLine("<meta name=\"GENERATOR\" content=\"Microsoft&reg; HTML Help Workshop 4.1\">");
                    streamWriter.WriteLine("<!-- Sitemap 1.0 -->");
                    streamWriter.WriteLine("</HEAD>");
                    streamWriter.WriteLine("<BODY>");

                    #region 关键字内容

                    streamWriter.WriteLine("<UL>");
                    streamWriter.WriteLine("	<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("		<param name=\"Name\" value=\"Index\">");
                    streamWriter.WriteLine("<param name=\"Local\" value=\"" + compileHTML + "\">");
                    streamWriter.WriteLine("</OBJECT>");

                    streamWriter.WriteLine("<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("<param name=\"Name\" value=\"201\">");
                    streamWriter.WriteLine("<param name=\"Local\" value=\"F:\\Desktop\\Desktop\\Desktop\\Code\\CreateChm\\CreateChm\\bin\\Debug\\html\\日韩演员\\201.html\">");
                    streamWriter.WriteLine("</OBJECT>");

                    streamWriter.WriteLine("</UL>");


                    #endregion

                    //TK();

                    streamWriter.WriteLine("</BODY>");
                    streamWriter.WriteLine("</HTML>");
                    streamWriter.WriteLine();
                }
            }
        }
        #endregion

        #region 创建HHP文件
        public void CreateHHP(string title)
        {
            using (FileStream fs = new FileStream(string.Concat(startPath, "\\", hhp), FileMode.Create, FileAccess.Write))
            {
                using (streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding("GB18030")))//System.Text.Encoding.GetEncoding("GB18030"));
                {
                    streamWriter.WriteLine("[OPTIONS]");
                    streamWriter.WriteLine("Title=" + title);//chm文档的标题
                    streamWriter.WriteLine("Compatibility=1.1 or later");//兼容版本
                    streamWriter.WriteLine("Compiled file=" + "我的CHM.chm");  //chm文件名 产生的CHM文档位置及文档名
                    //streamWriter.WriteLine("Compiled file=f:\\b.chm");  //chm文件名 产生的CHM文档位置及文档名
                    streamWriter.WriteLine("Contents file=" + hhc);  //hhc文件名 hhc文件，html文件归档，后面介绍
                    streamWriter.WriteLine("Index file=" + hhk);//索引文件，即是CHM文档的索引选项卡

                    streamWriter.WriteLine("Default topic=" + _defaultTopic);//默认页 开始页，即刚打开CHM显示的页面
                    streamWriter.WriteLine("Display compile progress=YES");//是否显示编译过程
                    //streamWriter.WriteLine("Language=0x804 中文(中国)");//chm文件语言
                    streamWriter.WriteLine("Default Window=Main");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("[WINDOWS]");
                    streamWriter.WriteLine("Main=\"" + title + "\",\"" + hhc + "\",\"" + hhk + "\",\"" +
                        _defaultTopic + "\",,,,,,0x20,180,0x104E, [80,60,720,540],0x0,0x0,,,,,0");//这里最重要了，一般默认即可
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("[FILES]");
                    streamWriter.WriteLine(compileHTML);
                    streamWriter.WriteLine(@"F:\Desktop\Desktop\Desktop\Code\CreateChm\CreateChm\bin\Debug\html\日韩演员\201.html");
                    //TF();
                    streamWriter.WriteLine();
                }
            }
        }
        #endregion

        #region 开始编译
        private bool Compile()
        {
            //创建新的进程，用Process启动HHC.EXE来Compile一个CHM文件
            Process helpCompileProcess = new Process();
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();

                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //调入HHC.EXE文件 
                processStartInfo.FileName = hhcFile;

                //获取空的HHP文件
                processStartInfo.Arguments = "\"" + string.Concat(startPath, "\\", hhp) + "\"";

                processStartInfo.UseShellExecute = false;

                helpCompileProcess.StartInfo = processStartInfo;

                helpCompileProcess.Start();

                helpCompileProcess.WaitForExit(); //组件无限期地等待关联进程退出

                if (helpCompileProcess.ExitCode == 0)
                {
                    MessageBox.Show(new Exception().Message);

                    return false;
                }
            }
            finally
            {
                helpCompileProcess.Close();
            }
            return true;
        }
        #endregion


        public void TC()
        {
            string Path = @"F:\Desktop\Desktop\Desktop\Code\VTSBlog\VTS.Web\CHM\日韩演员";

            StringBuilder sbr = new StringBuilder();
            string[] arr = System.IO.Directory.GetFileSystemEntries(Path);

            streamWriter.WriteLine("<UL>");
            streamWriter.WriteLine("<LI> <OBJECT type=\"text/sitemap\">");
            streamWriter.WriteLine("<param name=\"Name\" value=\"日韩演员\">");//表示目录
            streamWriter.WriteLine("</OBJECT>");

            streamWriter.WriteLine("<UL>");
            foreach (var item in arr)
            {
                if (item.EndsWith(".html"))
                {
                    string name = System.IO.Path.GetFileName(item);
                    string local = item;

                    #region 数据源
                    streamWriter.WriteLine("<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("<param name=\"Name\" value=\"" + name + "\">");
                    streamWriter.WriteLine("<param name=\"Local\" value=\"" + item + "\">");
                    streamWriter.WriteLine("</OBJECT>");
                    #endregion
                }
            }
            streamWriter.WriteLine("</UL>");

            streamWriter.WriteLine("</UL>");
        }

        public void TK()
        {
            string Path = @"F:\Desktop\Desktop\Desktop\Code\VTSBlog\VTS.Web\CHM\日韩演员";

            StringBuilder sbr = new StringBuilder();
            string[] arr = System.IO.Directory.GetFileSystemEntries(Path);

            streamWriter.WriteLine("<UL>");

            foreach (var item in arr)
            {
                if (item.EndsWith(".html"))
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(item);
                    string local = item;

                    #region 关键字内容
                    streamWriter.WriteLine("	<LI> <OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("		<param name=\"Name\" value=\"" + name + "\">");
                    streamWriter.WriteLine("<param name=\"Local\" value=\"" + local + "\">");
                    streamWriter.WriteLine("</OBJECT>");
                    #endregion
                }
            }
            streamWriter.WriteLine("</UL>");
        }

        public void TF()
        {
            string Path = @"F:\Desktop\Desktop\Desktop\Code\VTSBlog\VTS.Web\CHM\日韩演员";
            string[] arr = System.IO.Directory.GetFileSystemEntries(Path);
            foreach (var item in arr)
            {
                if (item.EndsWith(".html"))
                {
                    string local = item;
                    streamWriter.WriteLine(local);
                }
            }
            string Pathimg = @"F:\Desktop\Desktop\Desktop\Code\VTSBlog\VTS.Web\CHM\imageclub\Youyou-20150831\_b";
            string[] arrimg = System.IO.Directory.GetFileSystemEntries(Pathimg);
            foreach (var item in arrimg)
            {
                if (item.EndsWith(".jpg"))
                {
                    string local = item;
                    streamWriter.WriteLine(local);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string path = @"C:\Users\Administrator\Desktop\CHM";

            CHMDocument document = new CHMDocument();
            document.FileName = "Made by Alexis";
            document.Title = "Alexis";//设置根目录的名字

            //根节点
            CHMNode root = new CHMNode();

            root.Name = path.Substring(path.LastIndexOf('\\') + 1);

            document.Nodes.Add(root);

            GetFiles(path, root);

            //编译
            //document.Compile("a");
            document.Compile();
        }
        private void GetFiles(string filePath, CHMNode node)
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            node.Name = folder.Name;

            FileInfo[] chldFiles = folder.GetFiles("*.*");
            foreach (FileInfo chlFile in chldFiles)
            {
                if (chlFile.Extension == ".htm" || chlFile.Extension == ".html")
                {
                    CHMNode chldNode = new CHMNode();
                    chldNode.Name = chlFile.Name;
                    chldNode.Local = chlFile.FullName;
                    node.Nodes.Add(chldNode);
                }
            }

            DirectoryInfo[] chldFolders = folder.GetDirectories();
            foreach (DirectoryInfo chldFolder in chldFolders)
            {
                CHMNode chldNode = new CHMNode();
                chldNode.Name = folder.Name;
                node.Nodes.Add(chldNode);
                GetFiles(chldFolder.FullName, chldNode);
            }

        }
    }

    #region MyRegion
    //public class CHMDocument
    //{
    //    //编译过程中的hhp hhc hhk文件名
    //    private string _CompileFilename;

    //    public string CompileFilename
    //    {
    //        get { return _CompileFilename; }
    //        set { _CompileFilename = value; }
    //    }

    //    //CHM文档的标题
    //    private string _CHMTitle;

    //    public string CHMTitle
    //    {
    //        get { return _CHMTitle; }
    //        set { _CHMTitle = value; }
    //    }

    //    public CHMDocument()
    //    {

    //    }

    //    public void Compile()
    //    {

    //    }
    //} 
    #endregion
}