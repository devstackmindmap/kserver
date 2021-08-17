using System;
using System.Collections.Generic;
using System.Text;

namespace AkaUtility
{
    public class RecurNode
    {
        private Dictionary<string, RecurNode> _letters = null;
        private RecurNode _parent = null;
        private bool _isLastLeaf = false;
        private int _depth = 0;
        public void Initialize(int depth = 0)
        {
            _letters = new Dictionary<string, RecurNode>(StringComparer.OrdinalIgnoreCase);
            _depth = depth;
        }

        public Dictionary<string, RecurNode> Letters
        {
            get
            {
                return _letters;
            }
        }

        public bool IsLastLeaf
        {
            get
            {
                return _isLastLeaf;
            }
        }

        public int Depth
        {
            get { return _depth; }
        }

        public bool Add(string filter, RecurNode parent)
        {
            _parent = parent;

            if (_isLastLeaf)
            {
                return true;
            }

            if (string.IsNullOrEmpty(filter))
            {
                _isLastLeaf = true;
                return true;
            }

            try
            {
                RecurNode node = null;
                if (_letters.TryGetValue(filter[0].ToString(), out node) == false)
                {
                    node = new RecurNode();
                    node.Initialize(_depth + 1);
                    node.Add(filter.Substring(1), this);
                    _letters.Add(filter[0].ToString(), node);
                }
                else
                {
                    node.Add(filter.Substring(1), this);
                }
            }
            catch (Exception ex)
            {
            }
            return true;
        }
    }
}
