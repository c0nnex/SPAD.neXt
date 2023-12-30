namespace SPAD.neXt.Interfaces.UI
{
    public interface ITreeNodeItem
    {
        string TreeNodeName { get; }
        string TreeNodeID { get; }
        string TreeNodeParameter { get; }
    }

    public sealed class TreeNodeItem : ITreeNodeItem
    {
        public string TreeNodeName { get; }

        public string TreeNodeID { get; }

        public string TreeNodeParameter { get; }

        public TreeNodeItem(string treeNodeName, string treeNodeID, string treeNodeParameter)
        {
            TreeNodeName = treeNodeName;
            TreeNodeID = treeNodeID;
            TreeNodeParameter = treeNodeParameter;
        }
    }
}
