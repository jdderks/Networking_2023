using UnityEngine;

public class TileSelector : MonoBehaviour
{
    //public Color highlightColor = Color.green;  // Color to highlight the selected tile

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Check if the object has a Tile script attached
                CubeClicked(hitObject);
            }
        }
    }

    private static void CubeClicked(GameObject hitObject)
    {
        Tile tileScript = hitObject.GetComponent<Tile>();

        if (tileScript != null)
        {
            // Change the color of the tile
            Renderer renderer = hitObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = TeamManager.GetTeamColor(GameManager.Instance.teamManager.CurrentTeam);
            }
        }
    }
}
