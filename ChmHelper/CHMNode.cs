using System;
using System.Collections.Generic;
using System.Text;

namespace ChmHelper
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class CHMNode
    {
        #region 本地名
        private string _local;
        public string Local
        {
            get { return this._local; }
            set { _local = value; }
        }
        #endregion

        #region 名称值
        private string _name;
        public string Name
        {
            get { return this._name; }
            set { _name = value; }
        }
        #endregion

        #region 图标索引
        private string _imageNo;
        public string ImageNo
        {
            get { return this._imageNo; }
            set { _imageNo = value; }
        }
        #endregion

        #region 关键字
        private string _keyWords;
        public string KeyWords
        {
            get { return this._keyWords; }
            set { _keyWords = value; }
        }
        #endregion

        #region 节点集合
        //该节点的所有子节点集合
        private CHMNodeList _nodes = new CHMNodeList();
        public CHMNodeList Nodes
        {
            get { return this._nodes; }
            set { _nodes = value; }
        }
        #endregion
    }
}