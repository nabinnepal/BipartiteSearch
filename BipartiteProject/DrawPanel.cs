using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BipartiteProject
{
    public class DrawPanel : Panel
    {
        #region Fields
        private Point _p1;
        private Point _p2;
        private Point _p3;
        private bool _mouseDown;
        private int _mid = -1;
        private IList<Pair<Node>> _matching;
        private readonly IList<Node> _leftNodes = new List<Node>();
        private readonly IList<Node> _rightNodes = new List<Node>();
        private readonly IList<Pair<Node>> _edges=new List<Pair<Node>>();
        private bool _matchingDone;
        public event EventHandler FirstLineDrawn;
        #endregion

        #region Ctor
        public DrawPanel()
        {
            MouseMove += DrawPanel_MouseMove;
            MouseUp += DrawPanel_MouseUp;
            MouseDown += DrawPanel_MouseDown;
        } 
        #endregion

        #region Methods
        public void FindMatching()
        {
            var matcher = new BipartiteMatcher(_edges, _leftNodes, _rightNodes);
            _matching = matcher.GetMatch();
            _matchingDone = true;
            Invalidate();
        } 
        #endregion
        
        #region Events
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            foreach (var pair in _edges)
            {
                var leftNode = pair.First;
                var rightNode = pair.Second;
                
                g.DrawString(leftNode.DisplayValue, new Font("Times New Roman", 12), Brushes.Black,
                    leftNode.Coordinates.X - 15, leftNode.Coordinates.Y + 5);
                g.DrawString(rightNode.DisplayValue, new Font("Times New Roman", 12), Brushes.Black,
                    rightNode.Coordinates.X + 5, rightNode.Coordinates.Y + 5);
                
                g.DrawLine(new Pen(Color.Black), leftNode.Coordinates, rightNode.Coordinates);
            }
            
            if (_mid > 0)
                g.DrawLine(new Pen(Color.Black), _mid, 0, _mid, Height);

            if (_matchingDone)
            {
                foreach (var matchingRow in _matching)
                {
                    var left = matchingRow.First;
                    var right = matchingRow.Second;

                    g.DrawLine(new Pen(Color.Red, 7.0f), left.Coordinates, right.Coordinates);
                }
            }

        }

        private void DrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                _p3.X = e.X;
                _p3.Y = e.Y;
                EraseAndDrawLine();
                _p2 = _p3;
            }
        }
        private void DrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            Point p = new Point(e.X, e.Y);
            Node existingNode;

            if (Exists(p, out existingNode, _leftNodes, _rightNodes))
            {
                _p2.X = _p1.X = existingNode.Coordinates.X;
                _p2.Y = _p1.Y = existingNode.Coordinates.Y;
            }
            else
            {
                _p2.X = _p1.X = e.X;
                _p2.Y = _p1.Y = e.Y;
            }
        }
        private void DrawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            Node leftNode;
            Node rightNode;
            _mouseDown = false;

            if (_p1.X > _p2.X)
            {
                Point p = _p1;
                _p1 = _p2;
                _p2 = p;
            }

            if (_leftNodes.Count == 0)//first line drawn
            {
                _mid = (_p1.X + _p2.X) / 2;

                leftNode = new Node(_p1, 0, _mid);
                _leftNodes.Add(leftNode);
                rightNode = new Node(_p2, 0, _mid, "v");
                _rightNodes.Add(rightNode);

                if (FirstLineDrawn != null)
                    FirstLineDrawn(this, EventArgs.Empty);
            }
            else
            {
                if ((_p1.X > _mid && _p2.X > _mid) || (_p1.X < _mid && _p2.X < _mid))
                {
                    MessageBox.Show("This will violate Bipartite Graph Property");
                    Invalidate();
                    return;
                }
                if (!Exists(_p1, out leftNode, _leftNodes))
                {
                    leftNode = new Node(_p1, _leftNodes.Count, _mid);
                    _leftNodes.Add(leftNode);

                }
                if (!Exists(_p2, out rightNode, _rightNodes))
                {
                    rightNode = new Node(_p2, _rightNodes.Count, _mid, "v");
                    _rightNodes.Add(rightNode);
                }
            }
            var edge = new Pair<Node>(leftNode, rightNode);
            _edges.Add(edge);
            Invalidate();
        }
        #endregion

        #region Private Methods
        private static bool Exists(Point point, out Node node, IEnumerable<Node> nodes)
        {
            foreach (var availableNode in nodes)
            {
                double distance = Math.Sqrt((point.X - availableNode.Coordinates.X) * (point.X - availableNode.Coordinates.X) +
                    (point.Y - availableNode.Coordinates.Y) * (point.Y - availableNode.Coordinates.Y));

                if (distance < 5)
                {
                    node = availableNode;
                    return true;
                }
            }
            node = null;
            return false;
        }

        private static bool Exists(Point point, out Node node, 
            IEnumerable<Node> leftNodes, IEnumerable<Node> rightNodes)
        {
            return Exists(point, out node, leftNodes) || 
                Exists(point, out node, rightNodes);
        }

        private void EraseAndDrawLine()
        {
            Graphics g = CreateGraphics();
            Pen erasePen = new Pen(BackColor);
            g.DrawLine(erasePen, _p1, _p2);
            g.DrawLine(new Pen(Color.Black), _p1.X, _p1.Y, _p3.X, _p3.Y);
        }

        #endregion
    }
}
