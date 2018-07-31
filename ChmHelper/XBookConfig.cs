using System;
using System.Collections.Generic;
using System.Text;

namespace ChmHelper
{
    /// <summary>
    /// 电子书配置文件
    /// </summary>
    public class XBookConfig
    {
        private string _compiler;
        public string Compiler
        {
            get { return this._compiler; }
            set { this._compiler = value; }
        }

        // 编译器路径
        private string _hhcPath = @"C:\Program Files (x86)\HTML Help Workshop\hhc.exe";

        public string HhcPath
        {
            get { return _hhcPath; }
            set { _hhcPath = value; }
        }

        //是否删除临时文件
        private bool _isDeleteTempFiles = true;
        public bool IsDeleteTempFiles
        {
            get { return _isDeleteTempFiles; }
            set { _isDeleteTempFiles = value; }
        }

        //编码规则
        private string _encodeType = "GB2312";
        public string EncodeType
        {
            get { return this._encodeType; }
            set { _encodeType = value; }
        }

        //默认首页
        private string _defaultPage;
        public string DefaultPage
        {
            get { return this._defaultPage; }
            set { _defaultPage = value; }
        }

        private string _editor;
        public string Editor
        {
            set { _editor = value; }
            get { return this._editor; }
        }


        //类似于单例模式，只实例化一个XBookConfig类
        private static XBookConfig _instance = null;
        public static XBookConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new XBookConfig();
                return _instance;
            }
        }
        private XBookConfig()
        {
        }

        //加载配置文件文件
        public void Load(string strFileName)
        {
            if (!System.IO.File.Exists(strFileName))
                return;
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(strFileName);
            _hhcPath = doc.DocumentElement.GetAttribute("hhc");
            _isDeleteTempFiles = doc.DocumentElement.HasAttribute("deleteTempFile");
            _editor = doc.DocumentElement.GetAttribute("editor");
        }

        //保存配置文件
        public void Save(string strFileName)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.AppendChild(doc.CreateElement("config"));
            doc.DocumentElement.SetAttribute("hhc", this._hhcPath);
            if (this._isDeleteTempFiles)
                doc.DocumentElement.SetAttribute("deleteTempFile", "1");
            doc.DocumentElement.SetAttribute("editor", this._editor);
            doc.Save(strFileName);
        }
    }
}