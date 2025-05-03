using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseInterractable
{
    public IEnumerable<CustomTileData> Interract(CustomTileData data = null);
}
