using Godot;

namespace ProjectA.Game.Utils;

public static class NodeUtils
{
    public static void MoveToParent<T>(this T node, Node newParent)
        where T : Node
    {
        if (node.GetParent() == null)
            newParent.AddChild(node);
        else
            node.Reparent(newParent);
    }
}
