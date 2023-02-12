using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Art Assets")]
    [SerializeField] private Material tileMaterial;
    // Change it depend on art asset
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.5f;
    [SerializeField] private float deathSpacing = 0.7f;
    [SerializeField] private float dragOffset = 1.5f;
    [SerializeField] private GameObject victoryScreen;


    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    [Header("Logic")]
    public bool isLocalGame = false;
    public bool gameStarted = false;
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    public const int TILE_COUNT_X = 8;
    public const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private List<SpecialMove> specialMoves = new List<SpecialMove>();
    // Record moves history, with format array of {start position, end position}
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    public static Chessboard instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void StartGame(bool isLocal)
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
        isLocalGame = isLocal;
        gameStarted = true;
    }


    private void Update()
    {
        if (!gameStarted)
        {
            return;
        }
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }


        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the index of the tile I hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                currentHover = hitPosition;
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover))
                ? LayerMask.NameToLayer("Highlight")
                : LayerMask.NameToLayer("Tile");
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                currentHover = hitPosition;
            }

            // If we press left click
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    // Is it our turn?
                    if (chessPieces[hitPosition.x, hitPosition.y].team == GameManager.instance.teamTurn.Value)
                    {
                        if (isLocalGame || GameManager.instance.teamTurn.Value == GameManager.instance.GetCurrentPlayer().team)
                        {
                            currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];
                            // A list of basic movement of this piece
                            availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                            // Get a list of special move
                            specialMoves = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);
                            HighlightTiles();
                        }

                    }
                }
            }

            // If we release left click
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if (validMove)
                {
                    if (!isLocalGame)
                    {
                        GameManager.instance.NotifyMadeAMove(previousPosition, hitPosition);
                    }
                }
                else
                {
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                }
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover))
                ? LayerMask.NameToLayer("Highlight")
                : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        // If we're dragging a piece
        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
            }
        }
    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;
        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject($"X:{x} Y:{y}");
        tileObject.transform.SetParent(transform);

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }


    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        //White team
        chessPieces[0, 0] = SpawnSinglePiece(PieceType.ROOK, PieceTeam.WHITE);
        chessPieces[1, 0] = SpawnSinglePiece(PieceType.KNIGHT, PieceTeam.WHITE);
        chessPieces[2, 0] = SpawnSinglePiece(PieceType.BISHOP, PieceTeam.WHITE);
        chessPieces[3, 0] = SpawnSinglePiece(PieceType.QUEEN, PieceTeam.WHITE);
        chessPieces[4, 0] = SpawnSinglePiece(PieceType.KING, PieceTeam.WHITE);
        chessPieces[5, 0] = SpawnSinglePiece(PieceType.BISHOP, PieceTeam.WHITE);
        chessPieces[6, 0] = SpawnSinglePiece(PieceType.KNIGHT, PieceTeam.WHITE);
        chessPieces[7, 0] = SpawnSinglePiece(PieceType.ROOK, PieceTeam.WHITE);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(PieceType.PAWN, PieceTeam.WHITE);
        }

        // Black team
        chessPieces[0, 7] = SpawnSinglePiece(PieceType.ROOK, PieceTeam.BLACK);
        chessPieces[1, 7] = SpawnSinglePiece(PieceType.KNIGHT, PieceTeam.BLACK);
        chessPieces[2, 7] = SpawnSinglePiece(PieceType.BISHOP, PieceTeam.BLACK);
        chessPieces[3, 7] = SpawnSinglePiece(PieceType.QUEEN, PieceTeam.BLACK);
        chessPieces[4, 7] = SpawnSinglePiece(PieceType.KING, PieceTeam.BLACK);
        chessPieces[5, 7] = SpawnSinglePiece(PieceType.BISHOP, PieceTeam.BLACK);
        chessPieces[6, 7] = SpawnSinglePiece(PieceType.KNIGHT, PieceTeam.BLACK);
        chessPieces[7, 7] = SpawnSinglePiece(PieceType.ROOK, PieceTeam.BLACK);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(PieceType.PAWN, PieceTeam.BLACK);
        }
    }
    public ChessPiece SpawnSinglePiece(PieceType type, PieceTeam team)
    {
        GameObject gameObject = Instantiate(prefabs[(int)type - 1], transform);
        ChessPiece cp = gameObject.GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[(int)team];
        return cp;
    }


    // Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }
    /// <summary>
    /// Move the piece in chessPieces[x,y] to actual position on the physical chess board.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="instant"></param>
    public void PositionSinglePiece(int x, int y, bool instant = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), instant);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }


    // Checkmate
    private void CheckMate(PieceTeam team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(PieceTeam winningTeam)
    {
        TextMeshProUGUI victoryText = victoryScreen.transform.Find("VictoryText").GetComponent<TextMeshProUGUI>();
        victoryText.text = winningTeam == PieceTeam.WHITE ? "White team wins" : "Black team win";
        victoryScreen.SetActive(true);
    }
    public void OnResetButton()
    {
        // Fields reset
        currentlyDragging = null;
        availableMoves.Clear();
        specialMoves.Clear();
        moveList.Clear();

        // UI
        victoryScreen.SetActive(false);

        // Cleanup
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                    Destroy(chessPieces[x, y].gameObject);
                chessPieces[x, y] = null;
            }
        }

        for (int i = 0; i < deadWhites.Count; i++)
        {
            Destroy(deadWhites[i].gameObject);
        }
        for (int i = 0; i < deadBlacks.Count; i++)
        {
            Destroy(deadBlacks[i].gameObject);
        }

        deadWhites.Clear();
        deadBlacks.Clear();

        // Respawn
        SpawnAllPieces();
        PositionAllPieces();
        GameManager.instance.ResetGame();
    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    // Operation
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == hitInfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one; // Invalid
    }
    /// <summary>
    /// Move the chess piece to the target position.
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="otherPlayer">Used when other player made a move. Skip availableMoves check</param>
    /// <returns></returns>
    private bool MoveTo(ChessPiece cp, int x, int y, bool otherPlayer = false)
    {
        if (!otherPlayer && !ContainsValidMove(ref availableMoves, new Vector2(x, y)))
        {
            return false;
        }

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        // Is there another piece on the target position
        if (chessPieces[x, y] != null)
        {
            ChessPiece otherCp = chessPieces[x, y];
            if (cp.team == otherCp.team)
            {
                return false;
            }
            else
            {

                // If it's the enemy team
                if (otherCp.type == PieceType.KING)
                {
                    CheckMate(cp.team);
                }
                AddToDeadList(otherCp);
            }
        }

        chessPieces[x, y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        if (!otherPlayer)
            GameManager.instance.SwitchTurnServerRpc();

        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });

        SpecialMoveHandler.ProcessSpecialMoves(ref moveList, ref specialMoves, ref chessPieces);

        return true;
    }

    /// <summary>
    /// Used by other player, either network or AI
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    public void MovePiece(Vector2Int before, Vector2Int after)
    {
        Debug.Log($"Network: Other player made a move {before} to {after}");
        ChessPiece cp = chessPieces[before.x, before.y];
        // Guaranteed to be valid
        bool validMove = MoveTo(cp, after.x, after.y, true);
        if (!validMove)
        {
            Debug.LogError("Invalid move. Impossible, this should not happen.");
        }
    }

    public void AddToDeadList(ChessPiece otherCp)
    {
        if (otherCp.team == PieceTeam.WHITE)
        {
            deadWhites.Add(otherCp);
            otherCp.SetScale(Vector3.one * deathSize);
            otherCp.SetPosition(new Vector3(TILE_COUNT_X * tileSize, yOffset, -1 * tileSize)
                - bounds
                + new Vector3(tileSize / 2, 0, tileSize / 2)
                + (Vector3.forward * deathSpacing) * deadWhites.Count);
        }
        else
        {
            deadBlacks.Add(otherCp);
            otherCp.SetScale(Vector3.one * deathSize);
            otherCp.SetPosition(new Vector3(-1 * tileSize, yOffset, TILE_COUNT_X * tileSize)
                - bounds
                + new Vector3(tileSize / 2, 0, tileSize / 2)
                + (Vector3.back * deathSpacing) * deadBlacks.Count);
        }
    }

}
