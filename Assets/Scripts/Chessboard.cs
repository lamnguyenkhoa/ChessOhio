using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioSource audioSource;
    public AudioClip chessMoveSound;
    public float dragOffset = 1.5f;
    public GameObject victoryScreen;
    public TextMeshProUGUI turnCountUI;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;
    [SerializeField] private GameObject enmityLinePrefab;
    [SerializeField] private List<EnmityLine> enmityLines;


    [Header("Logic")]
    public bool isLocalGame = false;
    public bool gameStarted = false;
    public int disableRaycastCount = 0; // Totally stop the Update(). Count to for multiple disable still in effect
    public bool pauseGame = false; // Prevent modify pieces (moving, promote, ..). Can still use Info button.
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    public ChessPiece currentlyHovering;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    public const int TILE_COUNT_X = 8;
    public const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentTileHover;
    private Vector3 bounds;
    private List<SpecialMove> specialMoves = new List<SpecialMove>();
    // Record moves history, with format array of {start position, end position}
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    public static Chessboard instance;
    public int turnCount;
    public bool combineMode = false;
    public bool gameFinished = false;
    public UnityEvent onEndTurn;


    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }

    public void StartGame(bool isLocal)
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
        isLocalGame = isLocal;
        gameStarted = true;
        gameFinished = false;
        turnCount = 1;
    }

    private void Update()
    {
        if (!gameStarted || disableRaycastCount > 0)
        {
            return;
        }
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if (combineMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameRule.instance.ExitCombineMode();
            }
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the index of the tile I hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentTileHover == -Vector2Int.one)
            {
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                currentTileHover = hitPosition;
            }

            // If we were already hovering a tile, change the previous one
            if (currentTileHover != hitPosition)
            {
                tiles[currentTileHover.x, currentTileHover.y].layer = (ContainsValidMove(ref availableMoves, currentTileHover))
                ? LayerMask.NameToLayer("Highlight")
                : LayerMask.NameToLayer("Tile");
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                currentTileHover = hitPosition;
            }



            // If we press right click
            if (Input.GetMouseButtonDown(1) && !combineMode && !currentlyDragging)
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    ChessPiece cpClicked = chessPieces[hitPosition.x, hitPosition.y];
                    GameManager.instance.OpenActionMenu(Input.mousePosition + new Vector3(80, 0, 0), cpClicked);
                }
            }

            // If we press left click
            if (Input.GetMouseButtonDown(0) && !pauseGame)
            {
                // If empty hand
                if (!currentlyDragging)
                {
                    // If clicked a piece
                    if (chessPieces[hitPosition.x, hitPosition.y] != null)
                    {
                        ChessPiece hitCp = chessPieces[hitPosition.x, hitPosition.y];
                        if (IsPieceLegalToInteract(hitCp))
                        {
                            // Then pick up a piece
                            // If in combine mode, left click does not pick up piece for moving,
                            // but select piece for combining instead.
                            if (combineMode)
                            {
                                ChessPiece selectedCp = hitCp;
                                GameRule.instance.AddOrRemovePiecesToCombine(selectedCp);
                            }
                            else
                            {
                                SetCurrentlyDraggingPiece(hitCp);
                            }
                        }
                    }
                }
                // If already dragging, then check the valid of the move to release
                else
                {
                    Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
                    bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                    if (validMove)
                    {
                        PlayPiecePlacementSound();
                        currentlyDragging.UpdateTurnMoved();
                        if (!isLocalGame)
                        {
                            GameManager.instance.NotifyMadeAMove(previousPosition, hitPosition);
                        }
                        TestIfCheck(currentlyDragging.team);
                        TestIfEnemyCantMakeAnyMove(currentlyDragging.team);
                    }
                    else
                    {
                        // Not valid move, return the piece to original position
                        currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                    }
                    currentlyDragging = null;
                    RemoveHighlightTiles();
                }
            }

            // Tooltip when we hover above a piece
            if (chessPieces[hitPosition.x, hitPosition.y] != null)
            {
                if (GameSetting.instance.showToolTip)
                {
                    ChessPiece hitCp = chessPieces[hitPosition.x, hitPosition.y];
                    GameManager.instance.ShowTextToolTip(hitCp.profile.pieceName, hitCp.transform.position);
                }
            }
            else
            {
                GameManager.instance.HideTextToolTip();
            }
        }
        else
        {
            // If we don't hover on anything
            // Reset tiles highlight
            if (currentTileHover != -Vector2Int.one)
            {
                tiles[currentTileHover.x, currentTileHover.y].layer = (ContainsValidMove(ref availableMoves, currentTileHover))
                ? LayerMask.NameToLayer("Highlight")
                : LayerMask.NameToLayer("Tile");
                currentTileHover = -Vector2Int.one;
                currentlyHovering = null;
            }
            // If we release the dragging piece here, revert it
            if (currentlyDragging && Input.GetMouseButtonDown(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        // If we're dragging a piece
        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * (transform.position.y + yOffset));
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                Vector3 localPos = transform.InverseTransformPoint(ray.GetPoint(distance));
                currentlyDragging.SetPosition(localPos + Vector3.up * dragOffset);
            }
        }

        // Highlight normal movement range
        if (currentTileHover != -Vector2Int.one)
        {
            if (currentlyHovering != chessPieces[currentTileHover.x, currentTileHover.y] &&
                currentlyDragging == null && !combineMode)
            {
                RemoveHighlightTiles();
                currentlyHovering = chessPieces[currentTileHover.x, currentTileHover.y];
                if (currentlyHovering != null)
                {
                    availableMoves = currentlyHovering.GetAvailableMoves();
                    HighlightTiles();
                }
            }
        }
        else
        {
            if (!combineMode && !currentlyDragging)
            {
                RemoveHighlightTiles();
            }
        }
    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
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
        tileObject.transform.localPosition = Vector3.zero;

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

        if (cp.GetComponent<MeshRenderer>())
        {
            cp.GetComponent<MeshRenderer>().material = teamMaterials[(int)team];
        }
        // Also change material for all child model
        foreach (Transform child in cp.transform)
        {
            if (child.name == "Model")
                child.GetComponent<MeshRenderer>().material = teamMaterials[(int)team];
        }

        return cp;
    }

    public bool TrySpawnSinglePieceAt(PieceType type, PieceTeam team, Vector2Int pos, bool force = false)
    {
        if (chessPieces[pos.x, pos.y] != null && !force)
        {
            return false;
        }
        chessPieces[pos.x, pos.y] = SpawnSinglePiece(type, team);
        PositionSinglePiece(pos.x, pos.y, true);
        return true;
    }


    // Positioning
    public void PositionAllPieces(bool instant = true)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    PositionSinglePiece(x, y, instant);
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
    public Vector3 GetTileCenter(int x, int y, bool worldSpace = false)
    {
        Vector3 localPosition = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
        if (worldSpace)
        {
            Vector3 worldPosition = transform.TransformPoint(localPosition);
            return worldPosition;
        }
        return localPosition;
    }

    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    public void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }


    /// <summary>
    /// Checkmate
    /// </summary>
    /// <param name="team">Team that got checkmated</param>
    public void CheckMate(PieceTeam team)
    {
        if (team == PieceTeam.WHITE)
            DisplayVictory(PieceTeam.BLACK);
        else
            DisplayVictory(PieceTeam.WHITE);
        gameFinished = true;
    }
    private void DisplayVictory(PieceTeam winningTeam)
    {
        TextMeshProUGUI victoryText = victoryScreen.transform.Find("VictoryText").GetComponent<TextMeshProUGUI>();
        victoryText.text = winningTeam == PieceTeam.WHITE ? "White team wins" : "Black team win";
        victoryScreen.SetActive(true);
        pauseGame = true;
        disableRaycastCount += 1;
    }

    private void DisplayDraw()
    {
        TextMeshProUGUI victoryText = victoryScreen.transform.Find("VictoryText").GetComponent<TextMeshProUGUI>();
        victoryText.text = "Draw!";
        victoryScreen.SetActive(true);
        pauseGame = true;
        disableRaycastCount += 1;
    }

    public void OnResetButton()
    {
        if (isLocalGame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            GameManager.instance.ResetLANGame();
    }

    public void EndTurn(bool sendNotification = false)
    {
        ChangeLockControlAllPiece(false);

        Chessboard.instance.IncreaseTurnCount();
        if (!Chessboard.instance.isLocalGame && sendNotification)
        {
            GameManager.instance.NotifyEndTurn();
        }

        // Observer pattern
        onEndTurn.Invoke();
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
        // For some unit that can move again if capture enemy piece
        bool canMoveAgain = false;

        if (!otherPlayer && !ContainsValidMove(ref availableMoves, new Vector2(x, y)))
        {
            return false;
        }

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        // Is there another piece on the target position
        if (chessPieces[x, y] != null)
        {
            ChessPiece otherCp = chessPieces[x, y];
            // Check if the other piece is our team or enemy team, and whether
            // we can capture it.
            if (!cp.CanCaptureAlly() && cp.team == otherCp.team)
            {
                return false;
            }
            else
            {
                // If Essential piece (such as King) then checkmate
                if (otherCp.IsEssential())
                {
                    CheckMate(otherCp.team);
                }
                AddToDeadList(otherCp);
                cp.UpdateStatCaptureHistory(otherCp.type);
                canMoveAgain = cp.CanMoveAgainAfterCapture();
            }
        }

        chessPieces[x, y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;
        PositionSinglePiece(x, y);

        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });
        bool dontEndTurn = SpecialMoveHandler.instance.ProcessSpecialMoves(ref moveList, ref specialMoves, ref chessPieces, otherPlayer);

        cp.ResolveAfterMove();

        // If nothing special, end the turn
        if (!otherPlayer && !dontEndTurn && !canMoveAgain)
        {
            cp.timeMoveAgain = 0;
            EndTurn(true);
        }

        if (canMoveAgain)
        {
            ChangeLockControlAllPiece(true);
            cp.timeMoveAgain += 1;
            cp.lockedControl = false;
            // Check if any available move again to prevent stuck
            List<Vector2Int> tmpMoves = cp.GetAvailableMoves();
            if (tmpMoves.Count == 0)
            {
                EndTurn(true);
            }
        }

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
        cp.UpdateTurnMoved();
        PlayPiecePlacementSound();

        // Guaranteed to be valid, but check just in case
        bool validMove = MoveTo(cp, after.x, after.y, true);
        if (!validMove)
        {
            Debug.LogError("Invalid move. Impossible, this should not happen.");
        }
        TestIfCheck(cp.team);
        TestIfEnemyCantMakeAnyMove(cp.team);
    }

    /// <summary>
    /// Change or Delete a piece from chessboard.
    /// </summary>
    /// <param name="pos">Position in board of piece</param>
    /// <param name="type">Type of unit to change to. NONE to delete</param>
    /// <param name="sendNotification">Notify other player that you changed this chess piece</param>
    public void ChangePiece(Vector2Int pos, PieceType type, bool sendNotification = false)
    {
        ChessPiece currentPiece = chessPieces[pos.x, pos.y];
        if (currentPiece == null)
        {
            Debug.Log($"ChangePiece but {pos} is null");
            return;
        }
        Destroy(chessPieces[pos.x, pos.y].gameObject);
        if (type != PieceType.NONE)
        {
            ChessPiece newPiece = Chessboard.instance.SpawnSinglePiece(type, currentPiece.team);
            chessPieces[pos.x, pos.y] = newPiece;
            Chessboard.instance.PositionSinglePiece(pos.x, pos.y, true);
        }
        else
        {
            chessPieces[pos.x, pos.y] = null;
        }
        if (!Chessboard.instance.isLocalGame && sendNotification)
        {
            GameManager.instance.NotifyChangePiece(pos, type);
        }
    }

    public void AddToDeadList(ChessPiece otherCp)
    {
        otherCp.dead = true;
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

    public ref ChessPiece[,] GetBoardRef()
    {
        return ref chessPieces;
    }

    public ref List<Vector2Int[]> GetMoveListRef()
    {
        return ref moveList;
    }

    public ref List<SpecialMove> GetSpecialMovesRef()
    {
        return ref specialMoves;
    }

    public void IncreaseTurnCount()
    {
        turnCount += 1;
        turnCountUI.text = $"Turn: {turnCount}";
        // Check for new rule
        if ((turnCount - 1) % GameSetting.instance.turnForNewRule == 0 && !gameFinished)
        {
            GameRule.instance.OpenRuleCardMenu();
        }
    }

    /// <summary>
    /// Useful for when you want a piece move multiple time a turn. You lock
    /// all other pieces and unlock that piece only.
    /// </summary>
    /// <param name="doLock">Lock or Unlock</param>
    public void ChangeLockControlAllPiece(bool doLock = true)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    chessPieces[x, y].lockedControl = doLock;
                }
            }
        }
    }

    public void PlayPiecePlacementSound()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(chessMoveSound);
    }

    public void SetCurrentlyDraggingPiece(ChessPiece piece)
    {
        if (IsPieceLegalToInteract(piece) && !combineMode)
        {
            currentlyDragging = piece;
            availableMoves = currentlyDragging.GetAvailableMoves();
            HighlightTiles();
        }
    }

    private bool IsPieceLegalToInteract(ChessPiece piece)
    {
        // Is piece not locked?
        if (!piece.lockedControl &&
            // Is it my turn?
            piece.team == GameManager.instance.teamTurn &&
            // Am I the correct player (for LAN game)
            (isLocalGame || GameManager.instance.teamTurn == GameManager.instance.GetCurrentPlayer().team)
            )
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Test if after this move, are we going to check the enemy essential piece?
    /// </summary>
    /// <returns></returns>
    public bool TestIfCheck(PieceTeam teamJustMoved, bool showDavid = true)
    {
        // Update enmity line
        foreach (EnmityLine line in enmityLines)
        {
            if (!line.CheckIfStillRelevent())
            {
                Destroy(line.gameObject, 0.1f);
            }
        }
        enmityLines.RemoveAll(line => !line.CheckIfStillRelevent());

        PieceTeam otherTeam = teamJustMoved == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        ChessPiece enemyTarget = FindEssentialPiece(otherTeam);
        bool isDangerous = false;
        if (enemyTarget != null)
            isDangerous = IsThisTileDangerous(new Vector2Int(enemyTarget.currentX, enemyTarget.currentY), otherTeam);
        if (isDangerous && showDavid)
        {
            GameManager.instance.ShowDavieCheck();
            List<ChessPiece> attackers = WhoTargetThisPiece(enemyTarget);
            foreach (ChessPiece attacker in attackers)
            {
                if (!attacker.hasEnmityLine)
                    SpawnEnmityLine(attacker, enemyTarget);
            }
        }

        return isDangerous;
    }

    public void TestIfEnemyCantMakeAnyMove(PieceTeam teamJustMoved)
    {
        bool canMove = false;
        PieceTeam otherTeam = teamJustMoved == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            if (canMove) break;
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (canMove) break;
                if (chessPieces[x, y] != null && chessPieces[x, y].team == otherTeam)
                {
                    List<Vector2Int> moves = chessPieces[x, y].GetAvailableMoves();
                    if (moves.Count > 0)
                    {
                        canMove = true;
                    }

                }
            }
        }


        if (!canMove)
        {
            gameFinished = true;
            DisplayDraw();
        }
    }

    /// <summary>
    /// Get tiles that the King shouldn't move to.
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetDangerousTiles(PieceTeam teamToMove)
    {
        PieceTeam otherTeam = teamToMove == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        List<Vector2Int> dangerousTiles = new List<Vector2Int>();

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null && chessPieces[x, y].team == otherTeam)
                {
                    List<Vector2Int> moves = chessPieces[x, y].GetAttackMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                    foreach (Vector2Int move in moves)
                    {
                        if (!dangerousTiles.Contains(move))
                            dangerousTiles.Add(move);
                    }
                }
            }
        }

        return dangerousTiles;
    }

    public ChessPiece FindEssentialPiece(PieceTeam team)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null &&
                    chessPieces[x, y].team == team &&
                    chessPieces[x, y].IsEssential())
                {
                    return chessPieces[x, y];
                }
            }
        }
        return null;
    }

    public bool IsThisTileDangerous(Vector2Int pos, PieceTeam teamToMove)
    {
        PieceTeam otherTeam = teamToMove == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        ChessPiece tmp = chessPieces[pos.x, pos.y];

        // If this tile is the enemy essential piece (which mean moving here will 
        // win the game) then it's not dangerous anymore.
        if (tmp != null && tmp.IsEssential() && tmp.team == otherTeam)
        {
            return false;
        }

        // Temporary remove unit
        chessPieces[pos.x, pos.y] = null;

        List<Vector2Int> dangerouseTiles = GetDangerousTiles(teamToMove);
        if (dangerouseTiles.Contains(pos))
        {
            chessPieces[pos.x, pos.y] = tmp;
            return true;
        }

        // Return piece back to board
        chessPieces[pos.x, pos.y] = tmp;

        return false;
    }

    public List<ChessPiece> WhoTargetThisPiece(ChessPiece targetedPiece)
    {
        List<ChessPiece> attackers = new List<ChessPiece>();
        PieceTeam otherTeam = targetedPiece.team == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null &&
                    chessPieces[x, y].team == otherTeam)
                {
                    List<Vector2Int> moves = chessPieces[x, y].GetAvailableMoves();
                    if (moves.Contains(new Vector2Int(targetedPiece.currentX, targetedPiece.currentY)))
                    {
                        attackers.Add(chessPieces[x, y]);
                    }
                }
            }
        }
        return attackers;
    }

    public void SpawnEnmityLine(ChessPiece attacker, ChessPiece receiver)
    {
        EnmityLine line = Instantiate(enmityLinePrefab, attacker.transform).GetComponent<EnmityLine>();
        enmityLines.Add(line);
        line.attacker = attacker;
        line.receiver = receiver;
        line.RefreshData();
    }

    public void HighlightCombinableChessPieces(PieceType type, PieceTeam team)
    {
        RemoveHighlightTiles();
        // Find requested pieces
        List<ChessPiece> pieces = new List<ChessPiece>();
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null &&
                    chessPieces[x, y].team == team &&
                    chessPieces[x, y].type == type)
                {
                    pieces.Add(chessPieces[x, y]);
                }
            }
        }

        // Highlight tile of those pieces
        foreach (ChessPiece piece in pieces)
        {
            availableMoves.Add(new Vector2Int(piece.currentX, piece.currentY));
            HighlightTiles();
        }
    }

    public void GruMinifyPiece(int x, int y)
    {
        if (chessPieces[x, y] != null && !chessPieces[x, y].isGruMinified)
        {
            chessPieces[x, y].isGruMinified = true;
            float targetScale = 0.3f;
            chessPieces[x, y].SetScale(new Vector3(targetScale, targetScale, targetScale));
        }
    }
}
