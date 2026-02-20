using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Abstractions.Workspace;

public class WorkspaceNode
{
    public Guid Id { get; set; }

    public double Ratio { get; set; } = 0.5;

    public WorkspaceNode? Left { get; set; }

    public WorkspaceNode? Right { get; set; }

    public bool IsHorizontal { get; set; } = true;

    public NavigationState NavigationState { get; set; } = new();

    public int Index { get; set; }
}

public static class WorkspaceNodeEx
{
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