namespace Template.Platformer2D;

public static class Utils
{
    public static float ClampAndDampen(float horzVelocity, float dampening, float maxSpeedGround)
    {
        if (Mathf.Abs(horzVelocity) <= dampening)
            return 0;
        else if (horzVelocity > 0)
            return Mathf.Min(horzVelocity - dampening, maxSpeedGround);
        else
            return Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
    }
}
