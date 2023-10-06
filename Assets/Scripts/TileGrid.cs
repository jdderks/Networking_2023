using UnityEngine;
using NaughtyAttributes;
public class TileGrid : MonoBehaviour
{
    public GameObject tilePrefab;  // The prefab for the tile game object
    public int gridSizeX = 5;       // Number of tiles in the horizontal grid
    public int gridSizeY = 5;       // Number of tiles in the vertical grid
    public float spacing = 2.0f;   // Spacing between tiles

    void Start()
    {
        // Create the grid of tiles
        //CreateTileGrid();
    }

    [Button]
    public void CreateTileGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeY; z++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform); // Create a new tile

                // Position the tile based on the index and spacing
                float xPos = x * spacing;
                float zPos = z * spacing;
                newTile.transform.position = new Vector3(xPos, 0, zPos);

                // Assuming Tile script has a method to set its properties
                //Tile tileScript = newTile.GetComponent<Tile>();
                //if (tileScript != null)
                //{
                //    tileScript.SetProperties(); // Set any properties of the tile
                //}
            }
        }
    }
}
