using UnityEngine;

public class Cell
{
    public float height { get; private set; }
    public Vector2Int pos { get; private set; }
    public bool isWater {  get; private set; }
    public bool isHill { get; private set; }
    public bool isMountain { get; private set; }

    public Cell(Vector2Int pos, bool isWater, bool isHill, bool isMountain, float height) 
    {
        this.pos = pos;
        this.isWater = isWater;
        this.isHill = isHill;
        this.isMountain = isMountain;
        this.height = height;
    }
}