using Godot;

namespace GodotUtils;

public static class RayCast3DExtensions
{
    public static int GetRaycastsColliding(this RayCast3D[] raycasts)
    {
        int numRaycastsColliding = 0;

        foreach (RayCast3D raycast in raycasts)
        {
            if (raycast.IsColliding())
            {
                numRaycastsColliding++;
            }
        }

        return numRaycastsColliding;
    }

    public static void ExcludeRaycastParents(this RayCast3D raycast)
    {
        ExcludeParents(raycast, raycast.GetParent());
    }

    private static void ExcludeParents(RayCast3D raycast, Node parent)
    {
        if (parent != null)
        {
            if (parent is CollisionObject3D collision)
            {
                raycast.AddException(collision);
            }

            ExcludeParents(raycast, parent.GetParentOrNull<Node>());
        }
    }
}

