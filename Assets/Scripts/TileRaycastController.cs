using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileRaycastController : MonoBehaviour
{
    [SerializeField]
    private LayerMask mask;

    private Camera cam;


    private CustomTileData lastTile;
    private List<CustomTileData> highlighted = new List<CustomTileData>();

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    private void ClickTile()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100, mask))
            return;

        var behaviours = hit.transform.GetComponents<MonoBehaviour>();
        foreach (var behaviour in behaviours)
        {
            var temp = behaviour as IMouseInterractable;

            if (temp == null)
                continue;

            var recentTile = temp as CustomTileData;

            var tList = temp.Interract(lastTile);

            if (recentTile != null)
            {
                lastTile = recentTile;
            }

            if (tList != null)
            {
                foreach (var tile in highlighted)
                {
                    if (tList.Contains(tile))
                        continue;
                    Destroy(tile.gameObject.GetComponent<Outline>());
                }
                highlighted = tList.ToList();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);

        if (Input.GetMouseButtonUp(0))
        {
            ClickTile();
        }
    }
}
