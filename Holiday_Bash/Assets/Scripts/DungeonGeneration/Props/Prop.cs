using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "Scriptable Objects/Prop")]
public class Prop : ScriptableObject
{
    [Header("Prop Data")]
    public Sprite propSprite;

    public Vector2Int propSize = Vector2Int.one;

    [Space, Header("Placement Type")]
    public bool corner = true;
    public bool nearUpWall = true;
    public bool nearDownWall = true;
    public bool nearRightWall = true;
    public bool nearLeftWall = true;
    public bool innerMiddle = true;
    [Min(1)]
    public int placementQuantityMin = 1;
    [Min(1)]
    public int placementQuantityMax = 1;

    [Space, Header("Group Placement")]
    public bool placeAsGroup = false;
    [Min(1)]
    public int groupQuantityMin = 1;
    [Min(1)]
    public int groupQuantityMax = 1;
}
