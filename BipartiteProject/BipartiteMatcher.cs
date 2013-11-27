using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BipartiteProject
{
    public class BipartiteMatcher
    {
        #region Fields
        private readonly IList<Node> _leftNodes;
        private readonly IList<Node> _rightNodes;
        private readonly IList<Pair<Node>> _edges; 
        private readonly IList<Pair<Node>> _matching = new List<Pair<Node>>(); 
        #endregion

        #region Ctor
        public BipartiteMatcher(IList<Pair<Node>> edges, IList<Node> leftNodes, 
            IList<Node> rightNodes)
        {
            _edges = edges;
            _leftNodes = leftNodes;
            _rightNodes = rightNodes;
        } 
        #endregion

        #region Methods
        public IList<Pair<Node>> GetMatch()
        {
            var openNodes = new Queue<Node>();
            List<Node> visitedNodes;
            IList<Pair<Node>> parent = new List<Pair<Node>>();

            foreach (var leftNode in _leftNodes)
            {
                visitedNodes = new List<Node>();
                openNodes.Enqueue(leftNode);
                parent.Clear();

                while (openNodes.Count > 0)
                {
                    Node openNode = openNodes.Dequeue();
                    visitedNodes.Add(openNode);
                    if (openNode.IsLeftNode)
                    {
                        UpdateRightNodes(openNode, visitedNodes,
                                         openNodes, parent);
                    }
                    else
                    {
                        UpdateLeftNodes(openNode, visitedNodes,
                                        openNodes, parent);
                    }
                }
            }
            return _matching;
        } 
        #endregion

        #region Private Methods
        private void UpdateMatching(IList<Pair<Node>> parent, Node node)
        {
            bool found = true;
            Node tempNode = node;
            while (found)
            {
                found = false;

                var parentPair = parent.FirstOrDefault(p => p.First.Equals(tempNode));

                if (parentPair == null)
                    return;

                var pairInMatching = _matching.FirstOrDefault(m =>
                    m.First.Equals(parentPair.Second));

                if (pairInMatching != null)
                {
                    found = true;
                    tempNode = pairInMatching.Second;
                    _matching.Remove(pairInMatching);
                }

                var matchingRow = new Pair<Node>(parentPair.Second, parentPair.First);
                _matching.Add(matchingRow);
            }
        }

        private static void UpdateParentList(IList<Pair<Node>> parent,
            Node first, Node second)
        {
            var pair = new Pair<Node>(first, second);
            parent.Add(pair);
        }

        private void UpdateRightNodes(Node openNode, IList<Node> visitedNodes,
            Queue<Node> openNodes, IList<Pair<Node>> parent)
        {
            foreach (var rightNode in _rightNodes)
            {
                bool freeNodeFound = true;
                
                if (HasEdge(openNode, rightNode) && !visitedNodes.Contains(rightNode))
                {
                    if (_matching.Any(matchingPair => matchingPair.Second.Equals(rightNode)))
                    {
                        openNodes.Enqueue(rightNode);
                        freeNodeFound = false;
                    }

                    UpdateParentList(parent, rightNode, openNode);
                    if (freeNodeFound)
                    {
                        UpdateMatching(parent, rightNode);
                        openNodes.Clear();
                        break;
                    }
                }
            }
        }

        private void UpdateLeftNodes(Node openNode, IList<Node> visitedNodes,
            Queue<Node> openNodes, IList<Pair<Node>> parent)
        {
            foreach (var lNode in _leftNodes)
            {
                if (HasEdge(lNode,openNode) && !visitedNodes.Contains(lNode))
                {
                    if (_matching.Any(matchingPair => matchingPair.Second.Equals(openNode) &&
                        matchingPair.First.Equals(lNode)))
                    {
                        openNodes.Enqueue(lNode);
                        UpdateParentList(parent, lNode, openNode);
                    }
                }
            }
        }

        private bool HasEdge(Node left, Node right)
        {
            return _edges.Any(edgePair => edgePair.First.Equals(left) && 
                edgePair.Second.Equals(right));
        }

        #endregion
    }
}
