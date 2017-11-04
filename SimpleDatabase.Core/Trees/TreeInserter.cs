using System;
using SimpleDatabase.Core.Paging;

namespace SimpleDatabase.Core.Trees
{
    public class TreeInserter
    {
        private readonly IPager _pager;

        public TreeInserter(IPager pager)
        {
            _pager = pager;
        }

        public void LeafNodeInsert(Cursor cursor, int key, Row value)
        {
            var page = _pager.Get(cursor.PageNumber);
            var leaf = LeafNode.Read(page);

            if (leaf.CellCount >= NodeLayout.LeafNodeMaxCells)
            {
                LeafNodeSplitAndInsert(cursor, key, value);
            }
            else
            {
                leaf.InsertCell(cursor.CellNumber, key, value);
                _pager.Flush(cursor.PageNumber);
            }
        }

        private void LeafNodeSplitAndInsert(Cursor cursor, int key, Row value)
        {
            /*
             * Create a new node and move half the cells over.
             * Insert the new value in one of the two nodes.
             * Update parent or create a new parent.
             */

            var oldPage = _pager.Get(cursor.PageNumber);
            var oldNode = LeafNode.Read(oldPage);
            var (newPage, newPageNum) = _pager.GetUnusedPage();
            var newNode = LeafNode.New(newPage);

            newNode.NextLeaf = oldNode.NextLeaf;
            oldNode.NextLeaf = newPageNum;

            /*
             * All existing keys plus new key should should be divided
             * evenly between old (left) and new (right) nodes.
             * Starting from the right, move each key to correct position.
             */
            for (var i = NodeLayout.LeafNodeMaxCells; i >= 0; i--)
            {
                var destinationNode = i >= NodeLayout.LeafNodeLeftSplitCount ? newNode : oldNode;
                var indexWithinNode = i % NodeLayout.LeafNodeLeftSplitCount;

                if (i == cursor.CellNumber)
                {
                    destinationNode.SetCell(indexWithinNode, key, value);
                }
                else if (i > cursor.CellNumber)
                {
                    destinationNode.CopyCell(oldNode, i - 1, indexWithinNode);
                }
                else
                {
                    destinationNode.CopyCell(oldNode, i, indexWithinNode);
                }
            }

            // Update cell count on both leaf nodes
            oldNode.CellCount = NodeLayout.LeafNodeLeftSplitCount;
            newNode.CellCount = NodeLayout.LeafNodeRightSplitCount;

            if (oldNode.IsRoot)
            {
                CreateNewRoot(cursor.PageNumber, newPageNum);
            }
            else
            {
                throw new NotImplementedException("Update parent after split");
            }
        }

        private void CreateNewRoot(int rootPageNumber, int rightChildPageNum)
        {
            /*
             * Handle splitting the root.
             * Old root copied to new page, becomes left child.
             * Address of right child passed in.
             * Re-initialize root page to contain the new root node.
             * New root node points to two children.
             */

            var rootPage = _pager.Get(rootPageNumber);
            var (leftChild, leftChildPageNum) = _pager.GetUnusedPage();

            // Left child has data copied from old root
            Array.Copy(rootPage.Data, leftChild.Data, Pager.PageSize);
            var leftNode = Node.Read(leftChild);
            leftNode.IsRoot = false;

            // Root node is a new internal node with one key and two children
            var rootNode = InternalNode.New(rootPage);
            rootNode.IsRoot = true;

            rootNode.KeyCount = 1;

            var leftChildMaxKey = leftNode.GetMaxKey();
            rootNode.SetCell(0, leftChildPageNum, leftChildMaxKey);
            rootNode.SetChild(1, rightChildPageNum);
        }

    }
}
