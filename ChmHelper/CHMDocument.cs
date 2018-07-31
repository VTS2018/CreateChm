using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ChmHelper
{
    /// <summary>
    /// chm文件类
    /// </summary>
    public class CHMDocument
    {
        #region 属性

        //文件名
        private string _fileName = null;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        //输出文本
        private string _outPutText = null;
        public string OutPutText
        {
            get { return _outPutText; }
            set { _outPutText = value; }
        }

        //标题
        private string _title = null;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private CHMNodeList nodeList = new CHMNodeList();
        public CHMNodeList Nodes
        {
            get { return nodeList; }
            set { nodeList = value; }
        }

        #endregion

        #region 成员变量

        private StreamWriter streamWriter;
        private XBookConfig config = XBookConfig.Instance;//配置文件类

        //默认设置临时文件的名字
        private string strHhp = "xeditor.hhp";
        private string strHhc = "xeditor.hhc";
        private string strHhk = "xeditor.hhk";

        #endregion

        #region 构造函数
        /// <summary>
        /// 默认的构造函数
        /// </summary>
        public CHMDocument()
        {

        }
        #endregion

        #region 创建hhp文件
        /// <summary>
        /// 创建hhp文件
        /// </summary>
        /// <param name="htmFile">htm文件名</param>
        private void OpenHhp()
        {
            FileStream fs = new FileStream(strHhp, FileMode.Create); //创建hhp文件
            streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding(config.EncodeType));//可能会报警告 
            streamWriter.WriteLine("[OPTIONS]");
            streamWriter.WriteLine("Compatibility=1.1 or later");
            streamWriter.WriteLine("Compiled file=" + "Alexis.chm");  //chm文件名,要带后缀名
            streamWriter.WriteLine("Contents file=" + strHhc);  //hhc文件名
            streamWriter.WriteLine("Index file=" + strHhk);
            if (config.DefaultPage == null)
            {
                string fileTempPath = GetFirstPage(nodeList);

                if (fileTempPath.Contains("html_files"))
                {
                    config.DefaultPage = "html_files\\" + fileTempPath.Substring(fileTempPath.LastIndexOf('\\') + 1);
                }
                else
                {
                    config.DefaultPage = fileTempPath;
                }
            }
            streamWriter.WriteLine("Default topic=" + config.DefaultPage);  //默认页
            streamWriter.WriteLine("Display compile progress=yes"); //是否显示编译过程
            streamWriter.WriteLine("Language=0x804 中文(中国)");  //chm文件语言
            streamWriter.WriteLine("Title=" + _title);//标题
            streamWriter.WriteLine("Default Window=Main");
            streamWriter.WriteLine();
            streamWriter.WriteLine("[WINDOWS]");
            streamWriter.WriteLine("Main=,\"xeditor.hhc\",\"xeditor.hhk\",,,,,,,0x20,180,0x104E, [80,60,720,540],0x0,0x0,,,,,0");//这里最重要了，一般默认即可
            streamWriter.WriteLine();
            streamWriter.WriteLine("[FILES]");
            NodesHhp(nodeList);
            streamWriter.WriteLine();
            streamWriter.Close();
        }

        private string GetFirstPage(CHMNodeList nodeList)
        {
            string str = "";
            if (nodeList == null || nodeList.Count == 0)
                return str;
            foreach (CHMNode node in nodeList)
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    return node.Local;
                }
                else
                {
                    str = GetFirstPage(node.Nodes);
                }
            }
            return str;
        }

        //递归写hhp中的Files
        private void NodesHhp(CHMNodeList nodeList)
        {
            if (nodeList == null || nodeList.Count == 0)
                return;
            foreach (CHMNode node in nodeList)
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    streamWriter.WriteLine(node.Local);
                }
                else
                {
                    NodesHhp(node.Nodes);
                }
            }
        }



        #endregion

        #region 创建hhc文件

        /// <summary>
        /// 目录文件
        /// </summary>
        /// <param name="title">目录的主标题</param>
        private void OpenHhc()
        {
            FileStream fs = new FileStream(strHhc, FileMode.Create); //创建hhc文件
            streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding(config.EncodeType));

            streamWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">");
            streamWriter.WriteLine("<HTML>");
            streamWriter.WriteLine("  <HEAD>");
            streamWriter.WriteLine("  <meta name=\"GENERATOR\" content=\"Microsoft&reg; HTML Help Workshop 4.1\">");
            streamWriter.WriteLine("  <!-- Sitemap 1.0 -->");
            streamWriter.WriteLine("  </HEAD>");

            streamWriter.WriteLine("  <BODY>");
            streamWriter.WriteLine("    <OBJECT type=\"text/site properties\">");
            streamWriter.WriteLine("        <param name=\"Window Styles\" value=\"0x237\">");
            streamWriter.WriteLine("    </OBJECT>");

            streamWriter.WriteLine("    <UL>");
            streamWriter.WriteLine("        <LI><OBJECT type=\"text/sitemap\">");
            streamWriter.WriteLine("            <param name=\"Name\" value=\"" + this._title + "\">");
            streamWriter.WriteLine("            </OBJECT>");

            NodesHhc(nodeList);

            streamWriter.WriteLine("    </UL>");
            streamWriter.WriteLine("  </BODY>");
            streamWriter.WriteLine("</HTML>");
            streamWriter.Close();
        }

        /// <summary>
        /// 递归实现将nodes写入hhc文件中
        /// </summary>
        /// <param name="nodeList"></param>
        private void NodesHhc(CHMNodeList nodeList)
        {
            if (nodeList.Count == 0 || nodeList == null)
                return;
            streamWriter.WriteLine("        <UL>");

            foreach (CHMNode node in nodeList)
            {
                if (node.Nodes != null && node.Nodes.Count > 0)//如果是父节点
                {
                    streamWriter.WriteLine("    <LI><OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("            <param name=\"Name\" value=\"" + node.Name + "\">");
                    streamWriter.WriteLine("        </OBJECT>");
                    NodesHhc(node.Nodes);
                }
                else//如果是子节点
                {
                    streamWriter.WriteLine("         <LI><OBJECT type=\"text/sitemap\">");
                    streamWriter.WriteLine("                <param name=\"Name\" value=\"" + node.Name + "\">");
                    streamWriter.WriteLine("                <param name=\"Local\" value=\"" + node.Local + "\">");
                    streamWriter.WriteLine("             </OBJECT>");
                }
            }

            streamWriter.WriteLine("        </UL>");
        }

        #endregion

        #region 创建hhk文件

        /// <summary>
        /// 索引文件
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenHhk()
        {
            FileStream fs = new FileStream(strHhk, FileMode.Create); //创建hhp文件
            streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding(config.EncodeType));
            streamWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML//EN\">");
            streamWriter.WriteLine("<HTML>");
            streamWriter.WriteLine("    <HEAD>");
            streamWriter.WriteLine("        <meta name=\"GENERATOR\" content=\"Microsoft&reg; HTML Help Workshop 4.1\">");
            streamWriter.WriteLine("        <!-- Sitemap 1.0 -->");
            streamWriter.WriteLine("    </HEAD>");
            streamWriter.WriteLine("    <BODY>");
            streamWriter.WriteLine("        <UL>");
            streamWriter.WriteLine("	        <LI><OBJECT type=\"text/sitemap\">");

            foreach (CHMNode node in nodeList)
            {
                if (node.Nodes == null)
                    continue;
                streamWriter.WriteLine("	        <param name=\"Name\" value=\"" + node.Name + "\">");
                streamWriter.WriteLine("            <param name=\"Local\" value=\"" + node.Local + "\">");
            }

            streamWriter.WriteLine("            </OBJECT>");
            streamWriter.WriteLine("        </UL>");
            streamWriter.WriteLine("    </BODY>");
            streamWriter.WriteLine("</HTML>");
            streamWriter.WriteLine();
            streamWriter.Close();
        }

        #endregion

        #region 使用hha.dll进行编译
        delegate string GetInfo(string log);

        [DllImport("hha.dll")]
        static extern bool HHA_CompileHHP(string hhp, GetInfo pro, GetInfo fi, int flag);

        public string GetInfo1(string log)
        {
            this.OutPutText += log;
            return log;
        }

        /// <summary>
        /// 使用hha.dll进行编译
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Compile(string file)
        {
            this.CreateHhp();
            this.OpenHhc();
            this.OpenHhk();
            HHA_CompileHHP(this.strHhp, GetInfo1, GetInfo1, 0);
            return true;
        }


        /// <summary>
        /// 创建hhp文件
        /// </summary>
        /// <param name="htmFile">htm文件名</param>
        private void CreateHhp()
        {
            FileStream fs = new FileStream(strHhp, FileMode.Create); //创建hhp文件
            streamWriter = new System.IO.StreamWriter(fs, System.Text.Encoding.GetEncoding(config.EncodeType));//可能会报警告 
            streamWriter.WriteLine("[OPTIONS]");
            streamWriter.WriteLine("Compatibility=1.1 or later");
            streamWriter.WriteLine("Compiled file=" + "Alexis.chm");  //chm文件名,要带后缀名
            streamWriter.WriteLine("Contents file=" + strHhc);  //hhc文件名
            streamWriter.WriteLine("Index file=" + strHhk);
            streamWriter.WriteLine("Default topic=" + config.DefaultPage);  //默认页
            streamWriter.WriteLine("Display compile progress=yes"); //是否显示编译过程
            streamWriter.WriteLine("Language=0x804 中文(中国)");  //chm文件语言
            streamWriter.WriteLine("Title=" + _title);//标题
            streamWriter.WriteLine("Default Window=Main");
            streamWriter.WriteLine();
            streamWriter.WriteLine("[WINDOWS]");
            streamWriter.WriteLine("Main=,\"xeditor.hhc\",\"xeditor.hhk\",,,,,,,0x20,180,0x104E, [80,60,720,540],0x0,0x0,,,,,0");//这里最重要了，一般默认即可
            streamWriter.WriteLine();
            streamWriter.WriteLine("[FILES]");
            NodesHhp(nodeList);
            streamWriter.WriteLine();
            streamWriter.Close();
        }
        #endregion

        #region 编译

        /// <summary>
        /// 开始编译
        /// </summary>
        /// <returns></returns>
        public bool Compile()
        {
            this.OpenHhc();//创建hhc文件

            this.OpenHhk();

            this.OpenHhp();

            string _chmFile = "Alexis.chm";//chm文件名，与hhp中一致
            Process helpCompileProcess = new Process();  //创建新的进程，用Process启动HHC.EXE来Compile一个CHM文件
            try
            {
                try//判断文件是否存在并不被占用
                {
                    string path = _chmFile;  //chm生成路径
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    throw new Exception("文件被打开！");
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.FileName = config.HhcPath;  //调入HHC.EXE文件 
                processStartInfo.Arguments = "\"" + strHhp + "\"";//获取空的HHP文件

                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardOutput = true;
                helpCompileProcess.StartInfo = processStartInfo;
                helpCompileProcess.Start();
                helpCompileProcess.WaitForExit(); //组件无限期地等待关联进程退出
                _outPutText = helpCompileProcess.StandardOutput.ReadToEnd();
                if (helpCompileProcess.ExitCode == 0)
                {
                    //MessageBox.Show(new Exception().Message);
                    //todo 可以写入log
                    return false;
                }

                //如果删除临时文件
                //if (config.IsDeleteTempFiles)
                //{
                //    //File.Delete(strHhc);
                //    //File.Delete(strHhp);
                //    //File.Delete(strHhk);
                //}
            }
            finally
            {
                helpCompileProcess.Close();
            }
            return true;
        }

        #endregion

        #region 反编译


        #endregion

        #region 获得所有的文档节点
        /// <summary>
        /// 获得所有的文档节点
        /// </summary>
        /// <returns>文档节点列表</returns>
        public CHMNodeList GetAllNodes()
        {
            CHMNodeList list = new CHMNodeList();
            InnerGetNodes(list, nodeList);
            return list;
        }
        private void InnerGetNodes(CHMNodeList list, CHMNodeList list2)
        {
            foreach (CHMNode node in list2)
            {
                list.Add(node);
                InnerGetNodes(list, node.Nodes);//以递归的方式获得所有节点
            }
        }
        #endregion

        /// <summary>
        /// 加载文档
        /// </summary>
        /// <param name="filename">文件名</param>
        public void Load(string filename)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(filename);
            FromXML(doc.DocumentElement);
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="filename">文件名</param>
        public void Save(string filename)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.AppendChild(doc.CreateElement("CHMDocument"));
            ToXML(doc.DocumentElement);
            doc.Save(filename);
            _fileName = filename;
        }

        private void FromXML(System.Xml.XmlElement RootElement)
        {
            //this.defaultPage = RootElement.GetAttribute("DefaultTopic");
            this._title = RootElement.GetAttribute("Title");//标题
            nodeList.Clear();

            foreach (System.Xml.XmlNode node in RootElement.ChildNodes)
            {
                if (node.Name == "Items")
                {
                    NodesFromXML(nodeList, (System.Xml.XmlElement)node);
                }
            }
        }

        private void ToXML(System.Xml.XmlElement RootElement)
        {
            //RootElement.SetAttribute("DefaultTopic", this.strDefaultTopic);
            RootElement.SetAttribute("Title", this._title);
            System.Xml.XmlElement element = RootElement.OwnerDocument.CreateElement("Items");
            RootElement.AppendChild(element);
            NodesToXML(nodeList, element);
        }

        //xml转为为nodes
        private void NodesFromXML(CHMNodeList nodes, System.Xml.XmlElement RootElement)
        {
            foreach (System.Xml.XmlNode node in RootElement.ChildNodes)
            {
                if (node.Name == "Node")
                {
                    System.Xml.XmlElement element = (System.Xml.XmlElement)node;
                    CHMNode NewNode = new CHMNode();
                    NewNode.Name = element.GetAttribute("Name");
                    NewNode.Local = element.GetAttribute("Local");
                    NewNode.ImageNo = element.GetAttribute("ImageNumber");
                    NewNode.KeyWords = element.GetAttribute("KeyWords");
                    nodes.Add(NewNode);
                    foreach (System.Xml.XmlNode node2 in element.ChildNodes)
                    {
                        if (node2.Name == "Items")
                        {
                            NodesFromXML(NewNode.Nodes, (System.Xml.XmlElement)node2);
                        }
                    }
                }
            }
        }

        //nodes保存为xml
        private void NodesToXML(CHMNodeList nodes, System.Xml.XmlElement RootElement)
        {
            System.Xml.XmlDocument doc = RootElement.OwnerDocument;
            foreach (CHMNode node in nodes)
            {
                System.Xml.XmlElement NodeElement = doc.CreateElement("Node");
                NodeElement.SetAttribute("Name", node.Name);
                NodeElement.SetAttribute("Local", node.Local);
                NodeElement.SetAttribute("ImageNumber", node.ImageNo);
                NodeElement.SetAttribute("KeyWords", node.KeyWords);
                RootElement.AppendChild(NodeElement);
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    System.Xml.XmlElement ItemsElement = doc.CreateElement("Items");
                    NodeElement.AppendChild(ItemsElement);
                    NodesToXML(node.Nodes, ItemsElement);
                }
            }
        }

        private System.Xml.XmlElement[] GetChildElement(System.Xml.XmlElement element)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (System.Xml.XmlNode node in element.ChildNodes)
            {
                if (node is System.Xml.XmlElement)
                    list.Add(node);
            }
            return (System.Xml.XmlElement[])list.ToArray(typeof(System.Xml.XmlElement));
        }

        #region util
        /// <summary>
        /// 获取文件的绝对路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>返回该路径的绝对路径</returns>
        public string GetAbsolutePath(string path)
        {
            if (System.IO.Path.IsPathRooted(path))
                return path;
            else
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_fileName), path);

        }

        /// <summary>
        /// 获取文件的相对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetRelativePath(string path)
        {
            string strPath = System.IO.Path.GetDirectoryName(_fileName);//获取chm文件的路径
            strPath = path.ToLower();//都转换为小写
            string path2 = path.ToLower();//将传进来的路径转换为
            if (path2.StartsWith(path))
            {
                path2 = path.Substring(strPath.Length);
                if (path2.StartsWith("/") || path2.StartsWith("\\"))
                    path2 = path2.Substring(1);
                return path2;
            }
            return strPath;
        }
        #endregion
    }
}