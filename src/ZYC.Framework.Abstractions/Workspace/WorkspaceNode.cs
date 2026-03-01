using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Represents a node within a hierarchical workspace layout tree.
///     Can be either a parent node (defining a split) or a leaf node (containing navigation state).
/// </summary>
public class WorkspaceNode
{
    /// <summary>
    ///     Unique identifier for the workspace node.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The split ratio between the left/top and right/bottom children.
    ///     Typically ranges from 0.0 to 1.0 (e.g., 0.5 represents a 50/50 split).
    /// </summary>
    public double Ratio { get; set; } = 0.5;

    /// <summary>
    ///     The left (or top) child node in the split.
    ///     If null, this node may be a leaf.
    /// </summary>
    public WorkspaceNode? Left { get; set; }

    /// <summary>
    ///     The right (or bottom) child node in the split.
    ///     If null, this node may be a leaf.
    /// </summary>
    public WorkspaceNode? Right { get; set; }

    /// <summary>
    ///     Determines the orientation of the split.
    ///     True for horizontal (side-by-side); False for vertical (stacked).
    /// </summary>
    public bool IsHorizontal { get; set; } = true;

    /// <summary>
    ///     Holds the UI or navigation data associated with this node.
    ///     Usually relevant when the node is a leaf (no children).
    /// </summary>
    public NavigationState NavigationState { get; set; } = new();

    /// <summary>
    ///     The display or logical order index of the node within its parent context.
    /// </summary>
    public int Index { get; set; }
}

/// <summary>
///     Provides extension methods for the <see cref="WorkspaceNode" /> class to facilitate
///     tree traversal, searching, indexing, and debugging of the workspace layout.
/// </summary>
public static class WorkspaceNodeEx
{
    /// <summary>
    ///     Traverses the entire tree to find and return the smallest non-negative
    ///     integer index that is not currently assigned to any node.
    /// </summary>
    /// <param name="root">The root node of the workspace tree.</param>
    /// <returns>The next available unique index (0 or greater).</returns>
    public static int AllocateNextIndex(this WorkspaceNode root)
    {
        var used = new HashSet<int>();
        var stack = new Stack<WorkspaceNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var n = stack.Pop();
            if (n.Index >= 0)
            {
                used.Add(n.Index);
            }

            if (n.Left is not null)
            {
                stack.Push(n.Left);
            }

            if (n.Right is not null)
            {
                stack.Push(n.Right);
            }
        }

        var i = 0;
        while (used.Contains(i))
        {
            i++;
        }

        return i;
    }

    /// <summary>
    ///     Recursively prints a visual tree structure of the workspace to the System.Diagnostics.Trace output.
    ///     Useful for visualizing splits and hierarchy during debugging.
    /// </summary>
    /// <param name="workspaceNode">The starting node to trace.</param>
    /// <param name="indent">The current indentation string for the tree level.</param>
    /// <param name="isLeft">Determines which tree branch character to use (TEE vs ELBOW).</param>
    public static void Trace(this WorkspaceNode workspaceNode, string indent = "", bool isLeft = true)
    {
#if DEBUG
        System.Diagnostics.Trace.WriteLine(
            $"{indent}{(isLeft ? "├──" : "└──")} [Id={workspaceNode.Id.ToString()[..8]}] Ratio={workspaceNode.Ratio}, {(workspaceNode.IsHorizontal ? "Horizontal" : "Vertical")}");

        var childIndent = indent + (isLeft ? "│   " : "    ");
        if (workspaceNode.Left != null)
        {
            workspaceNode.Left.Trace(childIndent);
        }

        if (workspaceNode.Right != null)
        {
            workspaceNode.Right.Trace(childIndent, false);
        }
#endif
    }

    /// <summary>
    ///     Performs a depth-first search to find the first node in the tree
    ///     where the <see cref="WorkspaceNode.Left" /> child is null.
    /// </summary>
    /// <param name="workspaceNode">The node to start searching from.</param>
    /// <returns>The first node found with a missing left child, or null if not found.</returns>
    public static WorkspaceNode? FindFirstLeftNull(this WorkspaceNode workspaceNode)
    {
        if (workspaceNode.Left == null)
        {
            return workspaceNode;
        }

        // First search the left subtree
        var leftResult = workspaceNode.Left.FindFirstLeftNull();
        if (leftResult != null)
        {
            return leftResult;
        }

        // Then search the right subtree
        if (workspaceNode.Right != null)
        {
            var rightResult = workspaceNode.Right.FindFirstLeftNull();
            if (rightResult != null)
            {
                return rightResult;
            }
        }

        return null;
    }

    /// <summary>
    ///     Performs a depth-first search (prioritizing the right branch) to find the
    ///     first node in the tree where the <see cref="WorkspaceNode.Right" /> child is null.
    /// </summary>
    /// <param name="workspaceNode">The node to start searching from.</param>
    /// <returns>The first node found with a missing right child, or null if not found.</returns>
    public static WorkspaceNode? FindFirstRightNull(this WorkspaceNode workspaceNode)
    {
        if (workspaceNode.Right == null)
        {
            return workspaceNode;
        }

        // First search the right subtree
        var rightResult = workspaceNode.Right.FindFirstRightNull();
        if (rightResult != null)
        {
            return rightResult;
        }

        // Then search the left subtree
        if (workspaceNode.Left != null)
        {
            var leftResult = workspaceNode.Left.FindFirstRightNull();
            if (leftResult != null)
            {
                return leftResult;
            }
        }

        return null;
    }

    /// <summary>
    ///     Searches for a node that matches the specified <see cref="Guid" />
    ///     and ensures that node's <see cref="WorkspaceNode.Left" /> child is null.
    /// </summary>
    /// <param name="workspaceNode">The node to start searching from.</param>
    /// <param name="id">The unique identifier to look for.</param>
    /// <returns>The matching node if found and it has no left child; otherwise, null.</returns>
    public static WorkspaceNode? FindNodeWithIdAndLeftNull(this WorkspaceNode workspaceNode, Guid id)
    {
        if (workspaceNode.Id == id
            && workspaceNode.Left == null)
        {
            return workspaceNode;
        }

        WorkspaceNode? found = null;
        if (workspaceNode.Left != null)
        {
            found = workspaceNode.Left.FindNodeWithIdAndLeftNull(id);
        }

        if (found == null
            && workspaceNode.Right != null)
        {
            found = workspaceNode.Right.FindNodeWithIdAndLeftNull(id);
        }

        return found;
    }

    /// <summary>
    ///     Uses a Breadth-First Search (BFS) to find the leaf node (a node with no children)
    ///     that is closest to the root of the tree.
    /// </summary>
    /// <param name="root">The root of the workspace tree.</param>
    /// <returns>The shallowest leaf node found, or null if the root is null.</returns>
    public static WorkspaceNode? FindShallowestLeaf(this WorkspaceNode? root)
    {
        if (root == null)
        {
            return null;
        }

        var q = new Queue<WorkspaceNode>();
        q.Enqueue(root);

        while (q.Count > 0)
        {
            var node = q.Dequeue();

            if (node.Left == null && node.Right == null)
            {
                return node;
            }

            if (node.Left != null)
            {
                q.Enqueue(node.Left);
            }

            if (node.Right != null)
            {
                q.Enqueue(node.Right);
            }
        }

        return null;
    }
}