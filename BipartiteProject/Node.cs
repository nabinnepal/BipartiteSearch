using System;
using System.Drawing;

namespace BipartiteProject
{
    public class Node : IEquatable<Node>
    {
        #region Private Fields
        private readonly Point _coordinates;
        private readonly bool _isLeftNode;
        private readonly int _index; 
        #endregion

        #region Ctor
        public Node(Point coordinates, int index, int mid)
            : this(coordinates, index, mid, "u")
        {
        }

        public Node(Point coordinates, int index, int mid, string displayPrefix)
        {
            _coordinates = coordinates;
            _index = index;
            _isLeftNode = _coordinates.X < mid ? true : false;
            DisplayPrefix = displayPrefix;
        } 
        #endregion

        #region Properties
        public bool IsLeftNode
        {
            get { return _isLeftNode; }
        }

        public string DisplayPrefix { get; private set; }

        public string DisplayValue
        {
            get { return string.Format("{0}{1}", DisplayPrefix, _index); }
        }

        public int Index { get { return _index; } }
        public Point Coordinates { get { return _coordinates; } } 
        #endregion

        #region IEquatable<Node> Members

        public bool Equals(Node other)
        {
            return other.Coordinates == _coordinates && other.Index == _index;
        }

        #endregion
    }
}
