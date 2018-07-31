using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ChmHelper
{
    /// <summary>
    /// 节点集合类
    /// </summary>
    public class CHMNodeList : CollectionBase
    {
        /// <summary>
        ///  获得指定索引的node对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CHMNode this[int index]
        {
            get { return (CHMNode)this.List[index]; }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">待添加的节点</param>
        /// <returns></returns>
        public int Add(CHMNode node)
        {
            return this.List.Add(node);
        }

        /// <summary>
        /// 清空节点
        /// </summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        /// <summary>
        /// 插入节点
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="node">待插入的节点</param>
        public void Insert(int index, CHMNode node)
        {
            this.List.Insert(index, node);

        }

        /// <summary>
        /// 移除第一个匹配的节点
        /// </summary>
        /// <param name="node">待移除的节点</param>
        public void Remove(CHMNode node)
        {
            this.List.Remove(node);
        }

        /// <summary>
        /// 移除在index位置的节点
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        /// <summary>
        /// 是否包含该节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Contains(CHMNode node)
        {
            return this.List.Contains(node);
        }

        /// <summary>
        /// 添加若干个节点
        /// </summary>
        /// <param name="nodes"></param>
        public void AddRange(CHMNodeList nodes)
        {
            this.InnerList.AddRange(nodes);
        }

        /// <summary>
        /// 节点位于list的索引
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int IndexOf(CHMNode node)
        {
            return this.List.IndexOf(node);
        }
    }
}
