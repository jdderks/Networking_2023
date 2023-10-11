using UnityEngine;
using NaughtyAttributes;
using System;
using Unity.Networking.Transport;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

public class TileGrid : MonoBehaviour
{
    public GameObject tilePrefab;   // The prefab for the tile game object
    public int gridSizeX = 5;       // Number of tiles in the horizontal grid
    public int gridSizeY = 5;       // Number of tiles in the vertical grid
    public float spacing = 2.0f;    // Spacing between tiles

    public List<Tile> tiles = new();

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
                Tile tile = hitObject.GetComponentInParent<Tile>();
                if (tile.OwnedByTeam == Team.None && GameManager.Instance.teamManager.CurrentTeam == GameManager.Instance.teamManager.CurrentTeamTurn)
                {
                    CubeClicked(tile);
                }
            }
        }
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
                var tile = newTile.GetComponentInChildren<Tile>();
                tile.XPosition = x;
                tile.ZPosition = z;
                tile.CubeObject = newTile;
                tiles.Add(tile);
            }
        }
    }

    [Button]
    public void RemoveTileGrid()
    {
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile.gameObject);
        }
        tiles.Clear();
    }

    private Tile GetTileFromCoord(int x, int z)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].XPosition == x && tiles[i].ZPosition == z)
            {
                return tiles[i];
            }
        }
        return null;
    }

    private void CubeClicked(Tile tile)
    {
        //Tile tile = hitObject.GetComponent<Tile>();
        //Assert.IsNotNull(tile, "make sure the hit object has a Tile component!");

        NetCubeClicked cubeClickedNM = new NetCubeClicked();
        cubeClickedNM.xPosition = tile.XPosition;
        cubeClickedNM.zPosition = tile.ZPosition;
        cubeClickedNM.team = (int)GameManager.Instance.teamManager.CurrentTeam;

        Client.Instance.SendToServer(cubeClickedNM);

        //Go to next team
        var assignTeamNM = new NetAssignTeam();
        //assignTeamNM.randomTeam = false;
        if (GameManager.Instance.teamManager.CurrentTeamTurn == Team.Red)
            assignTeamNM.teamToAssign = (int)Team.Blue;
        else
            assignTeamNM.teamToAssign = (int)Team.Red;
        Client.Instance.SendToServer(assignTeamNM);
    }

    public bool CheckForFourInARow(Team teamToCheck)
    {
        int gridWidth = 5;
        int gridHeight = 5;

        // Check horizontal
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth - 3; col++)
            {
                Tile firstTile = GetTileFromCoord(row, col);
                if (firstTile != null && firstTile.OwnedByTeam == teamToCheck)
                {
                    bool match = true;
                    for (int i = 1; i < 4; i++)
                    {
                        Tile nextTile = GetTileFromCoord(row, col + i);
                        if (nextTile == null || nextTile.OwnedByTeam != teamToCheck)
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return true;
                }
            }
        }

        // Check vertical
        for (int col = 0; col < gridWidth; col++)
        {
            for (int row = 0; row < gridHeight - 3; row++)
            {
                Tile firstTile = GetTileFromCoord(row, col);
                if (firstTile != null && firstTile.OwnedByTeam == teamToCheck)
                {
                    bool match = true;
                    for (int i = 1; i < 4; i++)
                    {
                        Tile nextTile = GetTileFromCoord(row + i, col);
                        if (nextTile == null || nextTile.OwnedByTeam != teamToCheck)
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return true;
                }
            }
        }

        // Check diagonals (from top-right to bottom-left)
        for (int row = 0; row < gridHeight - 3; row++)
        {
            for (int col = 3; col < gridWidth; col++)
            {
                Tile firstTile = GetTileFromCoord(row, col);
                if (firstTile != null && firstTile.OwnedByTeam != Team.None)
                {
                    bool match = true;
                    for (int i = 1; i < 4; i++)
                    {
                        Tile nextTile = GetTileFromCoord(row + i, col - i);
                        if (nextTile == null || nextTile.OwnedByTeam != firstTile.OwnedByTeam)
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return true;
                }
            }
        }

        // Check diagonals (from top-left to bottom-right)
        for (int row = 0; row < gridHeight - 3; row++)
        {
            for (int col = 0; col < gridWidth - 3; col++)
            {
                Tile firstTile = GetTileFromCoord(row, col);
                if (firstTile != null && firstTile.OwnedByTeam != Team.None)
                {
                    bool match = true;
                    for (int i = 1; i < 4; i++)
                    {
                        Tile nextTile = GetTileFromCoord(row + i, col + i);
                        if (nextTile == null || nextTile.OwnedByTeam != firstTile.OwnedByTeam)
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                        return true;
                }
            }
        }




        return false;
    }


    #region networking related stuff
    private void Awake()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        NetUtility.S_CUBE_CLICKED += OnCubeClickedServer;

        NetUtility.C_CUBE_CLICKED += OnCubeClickedClient;

    }

    private void UnregisterEvents()
    {
        NetUtility.S_CUBE_CLICKED -= OnCubeClickedServer;

        NetUtility.C_CUBE_CLICKED -= OnCubeClickedClient;
    }

    private void OnCubeClickedClient(NetMessage msg)
    {
        NetCubeClicked nw = msg as NetCubeClicked;
        Tile tile = GetTileFromCoord(nw.xPosition, nw.zPosition);

        if (tile != null)
        {
            // Change the color of the tile
            Assert.IsNotNull(tile.CubeObject, "did you forget to set the CubeObject in the Tile prefab?");
            tile.OwnedByTeam = (Team)nw.team;
            Renderer renderer = tile.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = TeamManager.GetTeamColor((Team)nw.team);
            }
        }

        if (CheckForFourInARow((Team)nw.team))
        {
            NetPlayerWin playerWinNM = new NetPlayerWin();
            playerWinNM.teamWon = (byte)nw.team;
            Client.Instance.SendToServer(playerWinNM);
        }

        //If no tile is owned by no one anymore
        if (!tiles.Any(tile => tile.OwnedByTeam == Team.None))
        {
            NetPlayerWin playerDraw = new NetPlayerWin();
            playerDraw.teamWon = (int)Team.None;
            Client.Instance.SendToServer(playerDraw);
        }

    }

    private void OnCubeClickedServer(NetMessage msg, NetworkConnection cnn)
    {
        var nw = msg as NetCubeClicked;
        Server.Instance.Broadcast(nw);
    }
    #endregion
}
