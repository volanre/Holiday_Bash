using UnityEngine;

public static class MathFunctions
{
    public static float? GetCollisionTime(Vector3 rA, Vector3 vA, Vector3 rB, float vB)
    {
        Vector3 R = rA - rB;
        float a = vA.sqrMagnitude - vB * vB;
        float b = 2f * Vector3.Dot(vA, R);
        float c = R.sqrMagnitude;

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) return null; // No real solution — no collision

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        return t >= 0f ? t : (float?)null;
    }
    public static Vector3? GetThrowDirection(Vector3 rA, Vector3 vA, Vector3 rB, float vB)
    {
        float? t = GetCollisionTime(rA, vA, rB, vB);
        if (t == null) return null;

        Vector3 direction = (rA + vA * t.Value - rB).normalized;
        return direction;
    }
}
