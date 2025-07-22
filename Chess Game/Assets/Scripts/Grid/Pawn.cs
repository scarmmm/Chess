using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static GameManager;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Pawn : MonoBehaviour
{
    public GameOver GameOverScreen;
    public UI_Promotion PromotionScreen;
    [SerializeField] private GridLocations teamStartingPositions; 
    [SerializeField] private int _playerNumber; // Player number for the pawn
    [SerializeField] public CharacterController controller; // Reference to the CharacterController component
    [SerializeField] public Grid _grid;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private GameObject _pawn;
    [SerializeField] private GameObject _rook;
    [SerializeField] private GameObject _bishop;
    [SerializeField] private GameObject _knight;
    [SerializeField] private GameObject _queen;
    [SerializeField] private GameObject _king;
    [SerializeField] private GameObject _position;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private InputReader inputReader;
    [SerializeField] public Material team1; 
    [SerializeField] public Material team2;
    public GameObject selectedPawn;
    [SerializeField] public GameObject lastSelectedPiece;
    public AudioSource audioSource;
    public AudioClip piecePlaced; 
    private List<GameObject> pawns = new List<GameObject>(); // List to store pawns
    private List<GameObject> rooks = new List<GameObject>(); // List to store rooks
    private List<GameObject> pawns2 = new List<GameObject>();
    private List<GameObject> rooks2 = new List<GameObject>();
    private List<GameObject> bishops = new List<GameObject>(); // List to store bishops
    private List<GameObject> bishops2 = new List<GameObject>();
    private List<GameObject> knights = new List<GameObject>();
    private List<GameObject> knights2 = new List<GameObject>();
    private List<GameObject> queens = new List<GameObject>();
    private List<GameObject> queens2 = new List<GameObject>();
    [SerializeField]private List<GameObject> kings = new List<GameObject>();
    [SerializeField]private List<GameObject> kings2 = new List<GameObject>();
    [SerializeField]private List<GameObject> allPieces = new List<GameObject>();
    private List<GameObject> Team1 = new List<GameObject>();
    private List<GameObject> Team2 = new List<GameObject>();
    private Vector3 playerVelocity;
    bool player1KingCaptured = false;
    bool player2KingCaptured = false;
    private bool _validPiece = false;
    public string eliminatedPlayer = "Eliminated";
    ///public string playerTag1 = "Player1";
    Camera Camera;
    private bool stateHasChanged = false;
    private bool firstTurn = true;
   [SerializeField] private GameObject _referenceGameObject;
   [SerializeField] private bool canCastle = false;
   [SerializeField] private bool hasCastled = false;
    //below needs to become a component
   [SerializeField] private bool weAreInCheck = false; 
   [SerializeField] private UI_SelectedPiece selectedPieceUI;
   private bool isSelecting = true;
    // Start is called before the first frame update
    private void Start()
    {
        GameStates state = GameStates.SelectPiece;
        //player 1 pawns
        //could have handled this better was not thinking
        for (int i = 0; i < 8; i++)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(0, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.0f;
            transform.position = worldPosition2; // Set the position of the pawn to the world position
            GameObject pawnInstance = Instantiate(_pawn, worldPosition2, Quaternion.identity);
            pawnInstance.GetComponent<Renderer>().material = team1;
            pawnInstance.transform.localScale = new Vector3(20f, 20f, 20f);
            BoxCollider boxCollider = pawnInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            Rigidbody rigidbody = pawnInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            pawnInstance.AddComponent<Overlap>();
            Targeter targeter = pawnInstance.AddComponent<Targeter>();
            targeter.renderer = pawnInstance.GetComponent<Renderer>();
            PieceIdentity id = pawnInstance.AddComponent<PieceIdentity>();
            PawnMove pawnComponent = pawnInstance.AddComponent<PawnMove>();
            id.pieceType = ChessPieceType.Player1Pawn;
            pawnInstance.tag = "Player1";
            pawnInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            pawnInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            pawns.Add(pawnInstance);
            Team1.Add(pawnInstance);
            allPieces.Add(pawnInstance);

        }
        //player 1 rooks
        for (int i = 0; i < 9; i += 7)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.128f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject rookInstance = Instantiate(_rook, worldPosition2, Quaternion.identity);
            rookInstance.GetComponent<Renderer>().material = team1;
            BoxCollider boxCollider = rookInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            Targeter targeter = rookInstance.AddComponent<Targeter>();
            targeter.renderer = rookInstance.GetComponent<Renderer>();
            Rigidbody rigidbody = rookInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rookInstance.AddComponent<Overlap>();
            rookInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            PieceIdentity id = rookInstance.AddComponent<PieceIdentity>();
            RookMove pawnComponent = rookInstance.AddComponent<RookMove>();
            id.pieceType = ChessPieceType.Player1Rook;
            rookInstance.tag = "Player1";
            rookInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            rookInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            rooks.Add(rookInstance);
            Team1.Add(rookInstance);
            allPieces.Add(rookInstance);
        }
        //player 1 bishops
        for (int i = 2; i < 6; i += 3)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = .191f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject bishopInstance = Instantiate(_bishop, worldPosition2, Quaternion.identity);
            bishopInstance.GetComponent<Renderer>().material = team1;
            BoxCollider boxCollider = bishopInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            Targeter targeter = bishopInstance.AddComponent<Targeter>();
            targeter.renderer = bishopInstance.GetComponent<Renderer>();
            bishopInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            Rigidbody rigidbody = bishopInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            bishopInstance.AddComponent<Overlap>();
            PieceIdentity id = bishopInstance.AddComponent<PieceIdentity>();
            id.pieceType = ChessPieceType.Player1Bishop;
            bishopInstance.tag = "Player1";
            bishopInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            bishopInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            bishops.Add(bishopInstance);
            Team1.Add(bishopInstance);
            allPieces.Add(bishopInstance);
        }
        //player 1 knights
        for (int i = 1; i < 8; i += 5)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = .212f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject knightInstance = Instantiate(_knight, worldPosition2, Quaternion.identity);
            knightInstance.GetComponent<Renderer>().material = team1;
            BoxCollider boxCollider = knightInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            Targeter targeter = knightInstance.AddComponent<Targeter>();
            targeter.renderer = knightInstance.GetComponent<Renderer>();
            knightInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            PieceIdentity id = knightInstance.AddComponent<PieceIdentity>();
            knightInstance.AddComponent<Overlap>();
            Rigidbody rigidbody = knightInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            id.pieceType = ChessPieceType.Player1Knight;
            knightInstance.tag = "Player1";
            knightInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            knightInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            knights.Add(knightInstance);
            Team1.Add(knightInstance);
            allPieces.Add(knightInstance);
        }
        //player 1 queen
        for (int i = 1; i < 2; i++)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, 3, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.202f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject queenInstance = Instantiate(_queen, worldPosition2, Quaternion.identity);
            queenInstance.GetComponent<Renderer>().material = team1;
            BoxCollider boxCollider = queenInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            Targeter targeter = queenInstance.AddComponent<Targeter>();
            targeter.renderer = queenInstance.GetComponent<Renderer>();
            queenInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            PieceIdentity id = queenInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = queenInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            queenInstance.AddComponent<Overlap>();
            id.pieceType = ChessPieceType.Player1Queen;
            queenInstance.tag = "Player1";
            queenInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            queenInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            queens.Add(queenInstance);
            Team1.Add(queenInstance);
            allPieces.Add(queenInstance);
        }
        //player 1 king 
        for (int i = 1; i < 2; i++)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, 4, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.258f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject kingInstance = Instantiate(_king, worldPosition2, Quaternion.identity);
            kingInstance.GetComponent<Renderer>().material = team1;
            BoxCollider boxCollider = kingInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            Targeter targeter = kingInstance.AddComponent<Targeter>();
            targeter.renderer = kingInstance.GetComponent<Renderer>();
            kingInstance.AddComponent<Overlap>();
            kingInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            PieceIdentity id = kingInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = kingInstance.AddComponent<Rigidbody>();
            KingMove kingComponenet = kingInstance.AddComponent<KingMove>();
            IsInCheck inCheckComponent = kingInstance.AddComponent<IsInCheck>(); 
            rigidbody.useGravity = false;
            kingInstance.AddComponent<Overlap>();
            id.pieceType = ChessPieceType.Player1King;
            kingInstance.tag = "Player1";
            kingInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            kingInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            kings.Add(kingInstance);
            Team1.Add(kingInstance);
            allPieces.Add(kingInstance);
        }



        //player 2 pawns
        for (int i = 0; i < 8; i++)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(-5, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = .01f;
            transform.position = worldPosition2; // Set the position of the pawn to the world position
            GameObject pawnInstance = Instantiate(_pawn, worldPosition2, Quaternion.identity);
            pawnInstance.transform.localScale = new Vector3(20f, 20f, 20f);
            pawnInstance.GetComponent<Renderer>().material = team2;
            BoxCollider boxCollider = pawnInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            Targeter targeter = pawnInstance.AddComponent<Targeter>();
            targeter.renderer = pawnInstance.GetComponent<Renderer>();
            PieceIdentity id = pawnInstance.AddComponent<PieceIdentity>();
            PawnMove pawnComponent = pawnInstance.AddComponent<PawnMove>();
            Rigidbody rigidbody = pawnInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            pawnInstance.AddComponent<Overlap>();
            id.pieceType = ChessPieceType.Player2Pawn;
            pawnInstance.tag = "Player2";
            pawnInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            pawnInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            pawns2.Add(pawnInstance);
            Team2.Add(pawnInstance);
            allPieces.Add(pawnInstance);
        }
        //player 2 rooks
        for (int i = 0; i < 9; i += 7)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(-6, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.137f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject rookInstance = Instantiate(_rook, worldPosition2, Quaternion.identity);
            BoxCollider boxCollider = rookInstance.AddComponent<BoxCollider>();
            rookInstance.GetComponent<Renderer>().material = team2;
            boxCollider.isTrigger = true;
            Targeter targeter = rookInstance.AddComponent<Targeter>();
            targeter.renderer = rookInstance.GetComponent<Renderer>();
            rookInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.06f, .06f, .06f);
            PieceIdentity id = rookInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = rookInstance.AddComponent<Rigidbody>();
            RookMove pawnComponent = rookInstance.AddComponent<RookMove>();
            rigidbody.useGravity = false;
            rookInstance.AddComponent<Overlap>();
            id.pieceType = ChessPieceType.Player2Rook;
            rookInstance.tag = "Player2";
            rookInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            rookInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            rooks2.Add(rookInstance);
            Team2.Add(rookInstance);
            allPieces.Add(rookInstance);
        }
        //player2 bishops
        for (int i = 2; i < 6; i += 3)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(-6, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.172f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject bishopInstance = Instantiate(_bishop, worldPosition2, Quaternion.identity);
            BoxCollider boxCollider = bishopInstance.AddComponent<BoxCollider>();
            bishopInstance.GetComponent<Renderer>().material = team2;
            boxCollider.isTrigger = true;
            Targeter targeter = bishopInstance.AddComponent<Targeter>();
            bishopInstance.AddComponent<Overlap>();
            targeter.renderer = bishopInstance.GetComponent<Renderer>();
            bishopInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            PieceIdentity id = bishopInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = bishopInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            bishopInstance.AddComponent<Overlap>();
            id.pieceType = ChessPieceType.Player2Bishop;
            bishopInstance.tag = "Player2";
            bishopInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            bishopInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            bishops2.Add(bishopInstance);
            Team2.Add(bishopInstance);
            allPieces.Add(bishopInstance);
        }
        //player 2 knights
        for (int i = 1; i < 8; i += 5)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(-6, i, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = 0.176f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject knightInstance = Instantiate(_knight, worldPosition2, Quaternion.identity);
            BoxCollider boxCollider = knightInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            knightInstance.GetComponent<Renderer>().material = team2;
            Targeter targeter = knightInstance.AddComponent<Targeter>();
            targeter.renderer = knightInstance.GetComponent<Renderer>();
            knightInstance.AddComponent<Overlap>();
            knightInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.06f, .06f, .06f);
            PieceIdentity id = knightInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = knightInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            id.pieceType = ChessPieceType.Player2Knight;
            knightInstance.tag = "Player2";
            knightInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            knightInstance.transform.rotation = Quaternion.Euler(-90, -90, 0);
            queens.Add(knightInstance);
            Team2.Add(knightInstance);
            allPieces.Add(knightInstance);
        }
        //player 2 queen
        for (int i = 1; i < 2; i++)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(-6, 3, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = .227f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject queenInstance = Instantiate(_queen, worldPosition2, Quaternion.identity);
            BoxCollider boxCollider = queenInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            queenInstance.GetComponent<Renderer>().material = team2;
            Targeter targeter = queenInstance.AddComponent<Targeter>();
            targeter.renderer = queenInstance.GetComponent<Renderer>();
            queenInstance.AddComponent<Overlap>();
            queenInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.04f, .04f, .04f);
            PieceIdentity id = queenInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = queenInstance.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            id.pieceType = ChessPieceType.Player2Queen;
            queenInstance.tag = "Player2";
            queenInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            queenInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            queens2.Add(queenInstance);
            Team2.Add(queenInstance);
            allPieces.Add(queenInstance);
        }
        //player 2 king 
        for (int i = 1; i < 2; i++)
        {
            Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(-6, 4, -1));
            Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
            worldPosition2.y = .259f;
            transform.position = worldPosition2; // Set the position of the rook to the world position
            GameObject kingInstance = Instantiate(_king, worldPosition2, Quaternion.identity);
            BoxCollider boxCollider = kingInstance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            kingInstance.GetComponent<Renderer>().material = team2;
            Targeter targeter = kingInstance.AddComponent<Targeter>();
            targeter.renderer = kingInstance.GetComponent<Renderer>();
            kingInstance.AddComponent<Overlap>();
            kingInstance.transform.localScale = new Vector3(15f, 15f, 15f);
            boxCollider.size = new Vector3(.06f, .06f, .06f);
            PieceIdentity id = kingInstance.AddComponent<PieceIdentity>();
            Rigidbody rigidbody = kingInstance.AddComponent<Rigidbody>();
            KingMove kingComponenet = kingInstance.AddComponent<KingMove>();
            IsInCheck inCheckComponent = kingInstance.AddComponent<IsInCheck>(); 
            rigidbody.useGravity = false;
            id.pieceType = ChessPieceType.Player2King;
            kingInstance.tag = "Player2";
            kingInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
            kingInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            kings2.Add(kingInstance);
            Team2.Add(kingInstance);
            allPieces.Add(kingInstance);
        }
        
    }
    
    private void Update()
    {

        if (kings.Count == 0 || kings2.Count == 0 || kings[0] == null || kings2[0] == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        SetPosition();

    }
    
    private void SetPosition()
    {
        //this code will actively track the mouse position and convert it to a grid position
        //screen position function did not work had to use raycast to get the world space position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Chessboard");
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) return;
        if (!Input.GetMouseButtonDown(0)) return;
        //Debug.Log(hit.collider.gameObject == gameObject.CompareTag("ChessBoard"));
        if (selectedPawn == null)
        {
            if (hit.collider == null || hit.collider.gameObject == gameObject.CompareTag("Untagged")) return;
        }

        Vector3 worldMousePosition = hit.point; //get the world position of the mouse
        //below is the hitPosition
        Vector3Int gridPosition = _grid.WorldToCell(worldMousePosition); //convert the world position to a grid position(this works)
        //Debug.Log("Mouse0 Clicked");
        //Debug.Log("Hit Position" + gridPosition);
        if (selectedPawn != null){
            if (gridPosition == GetGridPosition(selectedPawn))
            {
                Debug.Log("Can't go to the same position");
                return;
            }
        }

        // this if statement is to select the piece 
        if (selectedPawn == null) // we currently don't have a piece selected
        {
            switch (GameManager.Instance.state)
            {
                //Debug.Log("Game State: " + GameManager.instance.State);
                case GameStates.PlayerTurn1:
                    EnableTargeter(Team1);
                    DisableTargeter(Team2);
                    PlayerTurn(hit, gridPosition, "Player1"); // we send the hit when mouse0 is clicked and required piece tag
                    return;
                case GameStates.PlayerTurn2:
                    EnableTargeter(Team2);
                    DisableTargeter(Team1);
                    PlayerTurn(hit, gridPosition, "Player2");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        // move the selected pawn to the new position, if the position is valid
        //processes second click
        else
        {
            bool isMovingPiece = false; 
            var id = selectedPawn.GetComponent<PieceIdentity>();
            var enemyList = GameManager.Instance.state == GameStates.PlayerTurn1 ? Team2 : Team1;
            var teamKing = GameManager.Instance.state == GameStates.PlayerTurn1 ? kings : kings2;
            GameObject kingGameObject = teamKing[0];
            Vector3Int kingPosition = GetGridPosition(kingGameObject);
            //we will do validation of the move here to check for our condition 
            //Debug.Log("We have our destination hit: " + gridPosition);
            
            //here we will check if this move with our king will place us in check
            if (id.pieceType == ChessPieceType.Player1King || id.pieceType == ChessPieceType.Player2King )
            {
               // Debug.Log("We have a king selected and are checking if we can move it");
                if (WillKingMovePlaceUsInCheck(selectedPawn, gridPosition) == true)
                {
                    Debug.Log("This move with our king will place us in check");
                    selectedPawn = null; 
                    return;
                }
            }

            // we can check if this move will get us out of check if it does we can proceed
            //Debug.Log("Entering: WillMovePlaceOurKingInCheck ");
            if (WillMovePlaceOurKingInCheck(selectedPawn, enemyList, gridPosition, kingPosition) == true && 
                InCheck(kingGameObject,enemyList) == false)
            {
                Debug.Log("This move will place our king in check so no!");
                selectedPawn = null; 
                return;
            }
            
            Debug.Log("are we in check: " + InCheck(kingGameObject,enemyList));
            if (InCheck(kingGameObject,enemyList) == true)
            {
                Debug.Log("We are in check");
                if (WillPieceMoveGetUsOutOfCheck(selectedPawn, enemyList, gridPosition, kingPosition) == true)
                {
                    Debug.Log("We are in check but got out");
                }
                //if the piece move does not get us out of check then we need to move the king
                else
                {
                    Debug.Log("we have use our king");
                    selectedPawn = kingGameObject;
                }
            } 
            
            var currentPieceComp = selectedPawn.GetComponent<PieceIdentity>();
            var currentKing = GameManager.Instance.state == GameStates.PlayerTurn1 ? kings[0] : kings2[0];
            var kingCheckComponent = currentKing.GetComponent<IsInCheck>();
            if (currentPieceComp != null && kingCheckComponent.isInCheck == false)
            {
                var isTargetType = currentPieceComp.pieceType switch
                {
                    ChessPieceType.Player1Rook or ChessPieceType.Player2Rook or
                        ChessPieceType.Player1King or ChessPieceType.Player2King => true,
                    _ => false
                };
                if (isTargetType)
                {
                    _referenceGameObject = hit.collider.gameObject;
                }
            }
            //Debug.Log(_referenceGameObject);
            var grid = GetGridPosition(selectedPawn);
            gridPosition.z = 0; 
            var currentPiece = selectedPawn;
            
            //handle moving the currently selected piece (need to refactor this)
            isMovingPiece = true; 
            if (IsValidPosition(currentPiece,grid, gridPosition, isMovingPiece))
            {
                
                //check if the piece is able to castle
                if (canCastle == false)
                {
                    selectedPawn.transform.position = _grid.GetCellCenterWorld(gridPosition);
                    AdjustPieceHeight(selectedPawn);
                }
                
                //check if can be promoted
                if (id.pieceType is ChessPieceType.Player1Pawn or ChessPieceType.Player2Pawn)
                {
                    HandlePromotion(currentPiece);
                }
                
                if (selectedPawn != null)
                {
                    lastSelectedPiece = selectedPawn;
                }
                
                selectedPawn = null; // deselect the pawn after moving
                audioSource.clip = piecePlaced;
                audioSource.Play();
                GameManager.Instance.UpdateGameState(GameManager.Instance.state == GameStates.PlayerTurn1 ? GameStates.PlayerTurn2 : GameStates.PlayerTurn1);
                selectedPieceUI.ClearSelectedPiece();
            }
            if (selectedPawn != null)
            {
                lastSelectedPiece = selectedPawn;
            }
            selectedPawn = null;
            _referenceGameObject = null;
            canCastle = false;
            kingCheckComponent.isInCheck = false;
            isMovingPiece = false; 
        }
    }
    private void PlayerTurn(RaycastHit hit, Vector3Int gridPosition, string player)
    {
        PieceIdentity piece = hit.collider.gameObject.GetComponent<PieceIdentity>();
        GameObject pieceGameObject = hit.collider.gameObject; 
        if (piece == null)
        {
            return; // Do nothing if no piece is clicked
        }
    
        //this is where the actual selection of the piece takes place
        if (selectedPawn == null) // Select piece
        {
            GameObject kingGameObject = null;
            var teamKing = GameManager.Instance.state == GameStates.PlayerTurn1 ? kings : kings2;
            var ourList = GameManager.Instance.state == GameStates.PlayerTurn1 ? Team2 : Team1;
            var teamPieces = GameManager.Instance.state == GameStates.PlayerTurn1 ? Team1 : Team2; 
            kingGameObject = teamKing[0];
            Vector3Int kingPosition = GetGridPosition(kingGameObject);
            //below is returning false. Why?
            //Debug.Log("Will this move place us in check: " + WillMovePlaceOurKingInCheck(pieceGameObject, ourList, gridPosition,kingPosition));
            
            //we are not in check and our move will not place our king in check
            if (piece != null && piece.pieceType.ToString().Contains(player))
            {
                selectedPawn = hit.collider.gameObject;
                var id = selectedPawn.GetComponent<PieceIdentity>();
                string displayName = id != null ? id.pieceType.ToString() : "Unknown";
                selectedPieceUI.SetPieceName($"Selected: {displayName}");
            }
            
            else
            {
                //Debug.Log("Invalid selection: Not your piece.");
                return;
            }
        }

        /*if (selectedPawn != null)
        {
            Debug.Log("peice position: " + GetGridPosition(selectedPawn));
        }*/

        _validPiece = true; //ensure that we have selected a piece
    }

    private bool IsValidPosition(GameObject currentPiece, Vector3Int currentPosition, Vector3Int destinationPosition,
        bool isThisMovingThePiece)
    {
        var isValid = false;
        var isBlocked = false;
        var isPathBlocked = false;
        isBlocked = IsBlocked(currentPiece, destinationPosition);
        isPathBlocked = PathIsBlocked(currentPiece, destinationPosition);
        //Debug.Log("IspathBlocked: " + PathIsBlocked(currentPiece, destinationPosition));
        //Debug.Log("Piece Identity: " + currentPiece );
        PieceIdentity piece = currentPiece.GetComponent<PieceIdentity>();
        switch (piece.pieceType)
        {
            //Debug.Log("Selected Position" + gridPosition);
            case ChessPieceType.Player1Pawn:
            case ChessPieceType.Player2Pawn:
            {
                //the following is the move logic for the player pawn
                var selectedPawnGridPos = GetGridPosition(currentPiece);
                var pawnComponent = currentPiece.GetComponent<PawnMove>();
                var isDiagnol = PieceIsDiagnol(currentPiece, destinationPosition);
                
                if (isDiagnol)
                {
                    Vector3Int diff = destinationPosition - selectedPawnGridPos;
                    if (Mathf.Abs(diff.x) == 1 && Mathf.Abs(diff.y) == 1)
                    {
                        return true;
                    }
                    
                    if (destinationPosition.x == selectedPawnGridPos.x - 1 && destinationPosition.y == selectedPawnGridPos.y + 1)
                    {
                        return  true;
                    }
                    return false;
                }

                if (pawnComponent != null && pawnComponent.isFirstMove == true)
                {
                    if (Mathf.Abs(destinationPosition.x - selectedPawnGridPos.x) == 2 && destinationPosition.y == selectedPawnGridPos.y && !isBlocked && isThisMovingThePiece == true)
                    {
                        pawnComponent.isFirstMove = false;
                        return true;
                    }
                    if (Mathf.Abs(destinationPosition.x - selectedPawnGridPos.x) == 2 && destinationPosition.y == selectedPawnGridPos.y && !isBlocked && isThisMovingThePiece == false)
                    {
                        //pawnComponent.isFirstMove = false;
                        return true;
                    }
                    //don't need this
                    switch (piece.pieceType)
                    {
                        case ChessPieceType.Player1Pawn:
                        {
                            if ( selectedPawnGridPos.x - destinationPosition.x == 1 &&
                                 destinationPosition.y == selectedPawnGridPos.y && !isBlocked)
                            {
                                return true;
                            }

                            break;
                        }
                        case ChessPieceType.Player2Pawn:
                        {
                            if (destinationPosition.x - selectedPawnGridPos.x == 1 &&
                                destinationPosition.y == selectedPawnGridPos.y && !isBlocked)
                            {
                                return true; 
                            }

                            break;
                        }
                        default:
                            return false;
                    }
                }
                switch (piece.pieceType)
                {
                    case ChessPieceType.Player1Pawn:
                    {
                        if ( selectedPawnGridPos.x - destinationPosition.x == 1 &&
                             destinationPosition.y == selectedPawnGridPos.y && !isBlocked)
                        {
                            return true;
                        }

                        break;
                    }
                    case ChessPieceType.Player2Pawn:
                    {
                        if (destinationPosition.x - selectedPawnGridPos.x == 1 &&
                            destinationPosition.y == selectedPawnGridPos.y && !isBlocked)
                        {
                            return true; 
                        }

                        break;
                    }
                    /*else if (Mathf.Abs(destinationPosition.x - selectedPawnGridPos.x) == 1 && destinationPosition.y == selectedPawnGridPos.y && !isBlocked)
                {
                    return true;
                }*/
                    default:
                        return false;
                }

                return false;
            }

            case ChessPieceType.Player1Rook:
            case ChessPieceType.Player2Rook:
            {
                //let's check for castling (get the hit) 
                var selectedRookGridPos = GetGridPosition(currentPiece);
                if (_referenceGameObject != null && hasCastled == false)
                {
                    var hitPieceIdentity = _referenceGameObject.GetComponent<PieceIdentity>();
                    if (hitPieceIdentity == null)
                    {
                        Debug.Log("A game piece was not the reference object");
                    }
                    else
                    {
                        var identity = hitPieceIdentity.pieceType;
                        if (identity == ChessPieceType.Player1King || identity == ChessPieceType.Player2King)
                        {
                            Castle(currentPiece, _referenceGameObject);
                            AdjustPieceHeight(currentPiece);
                            AdjustPieceHeight(_referenceGameObject);
                            if (canCastle == true) return true;
                        }
                    }
                }
                if (destinationPosition.y is > 7 or < 0)
                {
                    return false;
                }

                if (destinationPosition.x is < -6 or > 1)
                {
                    return false;
                }
                // Check if target is adjacent on X and remains on the same Y
                if (selectedRookGridPos.x != destinationPosition.x && selectedRookGridPos.y != destinationPosition.y)
                {
                    //return isValid = true;
                }
                else if (isBlocked == false && isPathBlocked == false)
                {
                    var rookComponent = currentPiece.GetComponent<RookMove>();
                    rookComponent.hasMoved = true; 
                    isValid = true;
                }

                return isValid;
        }
            
            case ChessPieceType.Player1Bishop:
            case ChessPieceType.Player2Bishop:
            { 
                
                //Debug.Log("isBlocked:" + isBlocked);
                //Debug.Log("isPathBlocked:" + isPathBlocked);
                
                if (destinationPosition.y is > 7 or < 0 || destinationPosition.x is < -6 or > 1)
                {
                    return false;
                }

                return isBlocked == false && isPathBlocked == false;
                break;
            }
            
            case ChessPieceType.Player1Knight:
            case ChessPieceType.Player2Knight:
            {
                if (destinationPosition.y is > 7 or < 0)
                {
                    return false;
                }
                if (destinationPosition.x is < -6 or > 1)
                {
                    return false;
                }
                if (Mathf.Abs(destinationPosition.x - currentPosition.x) == 2 && Mathf.Abs(destinationPosition.y - currentPosition.y) == 1)
                {
                    return true;
                }
                return Mathf.Abs(destinationPosition.x - currentPosition.x) == 1 && Mathf.Abs(destinationPosition.y - currentPosition.y) == 2;
            }
            case ChessPieceType.Player1Queen:
            case ChessPieceType.Player2Queen:
            {
                if (destinationPosition.y is > 7 or < 0 || destinationPosition.x is < -6 or > 1)
                {
                    return false;
                }

                if (isThisMovingThePiece == true)
                {
                    Debug.Log("isBlocked:" + isBlocked);
                    Debug.Log("isPathBlocked:" + isPathBlocked);
                }

                if (isBlocked == false && isPathBlocked == false)
                {
                    return true;
                }
                break;
            }
            case ChessPieceType.Player1King:
            case ChessPieceType.Player2King:
            {
                KingMove king = currentPiece.GetComponent<KingMove>();
                IsInCheck inCheckComponent = currentPiece.GetComponent<IsInCheck>();
                var weAreInCheck2 = inCheckComponent.isInCheck;
                if (  (destinationPosition.x < -6 || destinationPosition.x > 1) ||
                       (destinationPosition.y < 0  ||  destinationPosition.y >7)    )
                {
                    //Debug.Log("King move is out of bounds");
                    return false; 
                }
                if (_referenceGameObject != null && hasCastled == false && weAreInCheck2 == false)
                {
                    //Debug.Log("Did we enter isValid logic to call Castle()?");
                    var hitPieceIdentity = _referenceGameObject.GetComponent<PieceIdentity>();
                    if (hitPieceIdentity == null)
                    {
                        return false;
                    }

                    var identity =  hitPieceIdentity.pieceType;
                    if (identity == ChessPieceType.Player1Rook || identity == ChessPieceType.Player2Rook)
                    {
                        Castle(currentPiece, _referenceGameObject);
                        AdjustPieceHeight(currentPiece);
                        AdjustPieceHeight(_referenceGameObject);
                        if (canCastle) return true; 
                    }
                }

                
                //the following code will handle the logic if the king is in check (moves and end condition)
                if (weAreInCheck2 && selectedPawn == currentPiece)
                {
                    //lets create a check for whether there is a valid move remaining and then we should be done with the chess game
                    List<Vector3Int> allValidPositions = new List<Vector3Int>();
                    var currentPositionofKing = GetGridPosition(currentPiece);
                    allValidPositions= GetAllValidPositionsforKing(allValidPositions, currentPositionofKing);
                    Debug.Log("Can we get out of check: " + CanWeGetOutofCheck(currentPiece, allValidPositions));
                    if (CanWeGetOutofCheck(currentPiece, allValidPositions) == false)
                    {
                        //call the function that will end the game soon
                        var currentState = Instance.state;
                        var teamID = currentState == GameStates.PlayerTurn1 ? 1 : 2;   
                        GameOver(teamID);
                        return false; 
                    }
                    //if this is true then we cannot move to this location and will return an invalid move
                    if (WillKingMovePlaceUsInCheck(currentPiece, destinationPosition))
                    {
                        Debug.Log("Move will place us in check");
                        return false;
                    }
                }
                
                if (destinationPosition.y is > 7 or < 0)
                {
                    return false;
                }
                if (destinationPosition.x is < -6 or > 1)
                {
                    return false;
                }
                float deltaX = Mathf.Abs(destinationPosition.x - currentPosition.x);
                float deltaY = Mathf.Abs(destinationPosition.y - currentPosition.y);
                if (Mathf.Approximately(Mathf.Max(deltaX, deltaY), 1) && isBlocked == false) // Or !isBlocked
                {
                    // king.hasMoved = true; 
                    return true;
                }
                if (isBlocked)
                {
                    Debug.Log("Move is blocked");
                    return false; 
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    Vector3Int GetGridPosition(GameObject pawn)
    {
        var gridPosition = _grid.WorldToCell(pawn.transform.position);
        return gridPosition;
    }


    private bool IsBlocked(GameObject piece, Vector3 destination)
    {
        //the following code checks to see if friendly piece is occupying the destination location and prevent a pawn from capturing forward
        PieceIdentity pieceIdentity = piece.GetComponent<PieceIdentity>();
        //check if a friendly piece is blocking the move
        if (pieceIdentity.pieceType is ChessPieceType.Player1Pawn or ChessPieceType.Player1Bishop or ChessPieceType.Player1King or 
            ChessPieceType.Player1Knight or ChessPieceType.Player1Bishop or ChessPieceType.Player1Rook or ChessPieceType.Player1Queen)
        {
            //Debug.Log("hit position: " + destination);
            foreach (var t in Team1)
            {
                //get piece position
                var pieceGridLocation = GetGridPosition(t);
                //assign piece
                var pieceGo = t;
                if (Mathf.Approximately(pieceGridLocation.x, destination.x) && Mathf.Approximately(pieceGridLocation.y, destination.y) && pieceGo.activeInHierarchy)
                {
                    //Debug.Log("Piece is blocked");
                    return true;
                }
            }
            //the following code prevents a forward capture from a pawn
            if (pieceIdentity.pieceType == ChessPieceType.Player1Pawn)
            {
                var currentPos = GetGridPosition(piece);
                var isStraightMove = (Mathf.Approximately(destination.y, currentPos.y)) &&
                                      (Mathf.Approximately(destination.x, currentPos.x + 1) || Mathf.Approximately(destination.x, currentPos.x - 1));
                if (isStraightMove)
                {
                    // Loop through enemy pieces to see if one is occupying the destination.
                    foreach (var enemy in Team2)
                    {
                        var enemyPos = GetGridPosition(enemy);
                        if (Mathf.Approximately(enemyPos.x, destination.x) && Mathf.Approximately(enemyPos.y, destination.y) && enemy.activeInHierarchy)
                        {
                            //Debug.Log("forward capture is not allowed");
                            return true;
                        }
                    }
                }
            }

        }
        if (pieceIdentity.pieceType is ChessPieceType.Player2Pawn or ChessPieceType.Player2Bishop or ChessPieceType.Player2King or ChessPieceType.Player2Knight 
            or ChessPieceType.Player2Bishop or ChessPieceType.Player2Rook)
        {
            for (int i = 0; i < Team2.Count; i++)
            {
                //check if our team pieces are already occupied
                var pieceGridLocation = GetGridPosition(Team2[i]);
                var pieceGo = Team2[i];    

                if (Mathf.Approximately(pieceGridLocation.x, destination.x) && Mathf.Approximately(pieceGridLocation.y, destination.y) && pieceGo.activeInHierarchy)
                {
                     Debug.Log("Piece is blocked");
                    return true;
                }
            }
            //make sure that we are not capturing a forward facing piece
            if (pieceIdentity.pieceType == ChessPieceType.Player2Pawn )
            {
                //the following code prevents a forward capture from a pawn
                var currentPos = GetGridPosition(piece);
                var isStraightMove = (Mathf.Approximately(destination.y, currentPos.y)) &&
                                     (Mathf.Approximately(destination.x, currentPos.x + 1) || Mathf.Approximately(destination.x, currentPos.x - 1));
                if (isStraightMove)
                {
                    // Loop through enemy pieces to see if one is occupying the destination.
                    foreach (var enemy in Team1)
                    {
                        var enemyPos = GetGridPosition(enemy);
                        if (Mathf.Approximately(enemyPos.x, destination.x) && Mathf.Approximately(enemyPos.y, destination.y) && enemy.activeInHierarchy)
                        {
                            Debug.Log("forward capture is not allowed");
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool PieceIsDiagnol(GameObject currentPiece, Vector3Int hitPosition)
    {
        PieceIdentity pieceIdentity = currentPiece.GetComponent<PieceIdentity>();
        var selectedPiecePosition = GetGridPosition(currentPiece);
        if (pieceIdentity.pieceType == ChessPieceType.Player1Pawn)
        {
            List<Vector3> adjacent = new List<Vector3>();
            Vector3Int[] diagonalDirections = {
                new Vector3Int(-1, 1, 0),  // top-left
                new Vector3Int(-1, -1, 0), // bottom-left
                new Vector3Int(1, 1, 0),   // top-right
                new Vector3Int(1, -1, 0)   // bottom-right
            };
                //switch to the length of team to get all pieces
            for (var index = 0; index < Team2.Count; index++)
            {
                for (int j =0; j<diagonalDirections.Length; j++)
                {
                    var dir = diagonalDirections[j];
                    Vector3Int enemyPosition = GetGridPosition(Team2[index]);
                    Vector3Int possiblePosition = selectedPiecePosition + dir;
                    //Debug.Log("Possible Position to compare: " + possiblePosition);
                    //Debug.Log("Enemy Position to compare: " + enemyPosition);
                    //Debug.Log("Current Piece:" + Team2[index]);
                    if (enemyPosition == possiblePosition)
                    {
                        adjacent.Add(enemyPosition);
                            //check if our hitposition is the position of an enemy piece and make sure that it is active
                        if (hitPosition == enemyPosition && Team2[index].activeInHierarchy)
                        {
                            return true;
                        }
                    }
                } 
            }
            return false;
        }
        
        if (pieceIdentity.pieceType == ChessPieceType.Player2Pawn)
        {
            List<Vector3> adjacent = new List<Vector3>();
            Vector3Int[] diagonalDirections = {
                new Vector3Int(-1, 1, 0),  // top-left
                new Vector3Int(-1, -1, 0), // bottom-left
                new Vector3Int(1, 1, 0),   // top-right
                new Vector3Int(1, -1, 0)   // bottom-right
            };
            for (var index = 0; index < Team1.Count; index++)
            {
                for (int j =0; j<diagonalDirections.Length; j++)
                {
                    var dir = diagonalDirections[j];
                    Vector3Int enemyPosition = GetGridPosition(Team1[index]);
                    Vector3Int possiblePosition = selectedPiecePosition + dir;
                    //Debug.Log("Possible Position to compare: " + possiblePosition);
                    //Debug.Log("Enemy Position to compare: " + enemyPosition);
                    //Debug.Log("Current Piece:" + Team1[index]);
                    if (enemyPosition == possiblePosition)
                    {
                        adjacent.Add(enemyPosition);
                        //check if our hitposition is the position of an enemy piece and make sure that it is active
                        if (hitPosition == enemyPosition && Team1[index].activeInHierarchy)
                        {
                            return true;
                        }
                    }
                } 
            }
            return false;
        }
        
        return false;
    }

    private bool CanWeGetOutofCheck(GameObject currentPiece, List<Vector3Int> validKingMoves)
    {
        //this logic is broken
        for (int i = 0; i < validKingMoves.Count; i++)
        {
            //check if any of these moves will places us in check
            // the location cannot be taken
            if (WillKingMovePlaceUsInCheck(currentPiece, validKingMoves[i])  == false)
            {
                //if at least one move is valid we can still play
                Debug.Log(validKingMoves[i]);
                return true;
            }
        }
        return false; 
    }
    private bool PathIsBlocked(GameObject selectedPiece, Vector3Int hit)
    {
        var currentPiecePosition = GetGridPosition(selectedPiece);
        PieceIdentity pieceIdentity = selectedPiece.GetComponent<PieceIdentity>();
        List<GameObject> teamId = null;
        teamId = (selectedPiece.CompareTag("Player1")) ? Team1 : Team2;
        
        // this is to check if the path is valid NOT IF THERE IS A PIECE OCCUPYING THE HIT LOCATION
        
        if (pieceIdentity.pieceType is ChessPieceType.Player1Rook or ChessPieceType.Player2Rook)
        {
            if (hit.y == currentPiecePosition.y) // determine whether we are moving vertically or horizontally 
            {
                var minX = Mathf.Min(currentPiecePosition.x, hit.x);
                var maxX = Mathf.Max(currentPiecePosition.x, hit.x);

                for (int x = minX + 1; x < maxX; x++)
                {
                    var positionToCheck = new Vector3Int(x, currentPiecePosition.y, currentPiecePosition.z);
                    for (int i = 0; i < allPieces.Count; i++)
                    {
                        //here we check if any piece in the team list has the same location as the position we are checking
                        //think of it like we are gradually moving to the selected piece position from the hit position 
                        Vector3Int pieceGridLocation = GetGridPosition(allPieces[i]);
                        if (pieceGridLocation == positionToCheck && allPieces[i].activeInHierarchy)
                        {
                            return true;
                        }
                    }
                }
            }
            if (hit.x == currentPiecePosition.x)
            {
                var minY = Mathf.Min(currentPiecePosition.y, hit.y);
                var maxY = Mathf.Max(currentPiecePosition.y, hit.y);

                for (int y = minY + 1; y < maxY; y++)
                {
                    var positionToCheck = new Vector3Int(currentPiecePosition.x, y, currentPiecePosition.z);
                    for (int i = 0; i < allPieces.Count; i++)
                    {
                        Vector3Int pieceGridLocation = GetGridPosition(allPieces[i]);
                        if (pieceGridLocation == positionToCheck && allPieces[i].activeInHierarchy)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        else if (pieceIdentity.pieceType is ChessPieceType.Player1Queen or ChessPieceType.Player2Queen)
        {
            if (Mathf.Abs(currentPiecePosition.x - hit.x) == 1 && Mathf.Abs(currentPiecePosition.y - hit.y) == 1)
            {
                return false;
            }

            if (hit.y == currentPiecePosition.y) // we are moving horizontally  
            {
                var minX = Mathf.Min(currentPiecePosition.x, hit.x);
                var maxX = Mathf.Max(currentPiecePosition.x, hit.x);
                for (int x = minX + 1; x < maxX; x++)
                {
                    var positionToCheck = new Vector3Int(x, currentPiecePosition.y, 0);
                    for (int i = 0; i < allPieces.Count; i++)
                    {
                        //here we check if any piece in the team list has the same location as the position we are checking
                        //think of it like we are gradually moving to the selected pawn position from the hit position 
                        Vector3Int pieceGridLocation = GetGridPosition(allPieces[i]);
                        if (pieceGridLocation == positionToCheck && allPieces[i].activeInHierarchy)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (hit.x == currentPiecePosition.x) //we are moving vertically
            {
                var minY = Mathf.Min(currentPiecePosition.y, hit.y);
                var maxY = Mathf.Max(currentPiecePosition.y, hit.y);
                for (int y = minY + 1; y < maxY; y++)
                {
                    var positionToCheck = new Vector3Int(currentPiecePosition.x, y, 0);
                    for (int i = 0; i < allPieces.Count; i++)
                    {
                        Vector3Int pieceGridLocation = GetGridPosition(allPieces[i]);
                        if (pieceGridLocation == positionToCheck && allPieces[i].activeInHierarchy)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (hit.x != currentPiecePosition.x && hit.y != currentPiecePosition.y) // we are moving diagonally (this logic is wrong)   
            {
                /*Debug.Log(GetGridPosition(selectedPiece));
            Debug.Log(hit);*/
            if (hit.x == currentPiecePosition.x || hit.y == currentPiecePosition.y) return true;
                var isDiagonal = false;
                isDiagonal = Mathf.Abs(hit.x - currentPiecePosition.x) == 1 && Mathf.Abs(hit.y - currentPiecePosition.y) == 1; //adjacent move (distance of 1,1)
                if (isDiagonal == true)
                {
                    return false;
                }
                //let's make sure that we are diagonal 
                var hitX = hit.x;
                var hitY = hit.y;
                var currentX = currentPiecePosition.x;
                var currentY = currentPiecePosition.y;
                var valX = hitX - currentX;
                var valY = hitY - currentY;
                var signX = Mathf.Sign(valX);
                var signY = Mathf.Sign(valY);
                //Vector3Int[] vectorArray = new Vector3Int[10];
                List<Vector3Int> positions = new List<Vector3Int>();
                //Debug.Log("Did we enter?");
                switch ((signX, signY))
                {
                    case (1f, 1f):
                        for (int i = currentX + 1; i <= hitX; i++)
                        {
                            currentY++;
                            if (currentY == hitY && i == hitX) //we reached the hit point and therefore are diagonal  
                            {   
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(i, currentY, -1);
                            positions.Add(vec3);
                        }
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }
                        if (DiagonalPathBlocked(positions, teamId,hit) == false)
                        {
                            return false; 
                        }

                        break;
                    case (1f, -1f):
                        //this need to be fixed
                        for (int i = currentX + 1; i <= hitX; i++)
                        {
                            currentY--;
                            //Debug.Log("Current Location: " + i + " , " + currentY);
                            //Debug.Log("Hit Position: " + hitX + " , " + hitY);
                            if (currentY == hitY && i == hitX) //we reached the hit point and therefore are diagonal  
                            {
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(i, currentY, -1);
                            positions.Add(vec3);
                        }
                        
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }

                        if (DiagonalPathBlocked(positions, teamId,hit) == false)
                        {
                            return false; 
                        }
                        break;
                    case (-1f, 1f):
                        for (int i = currentY + 1; i <= hitY; i++)
                        {
                            currentX--;
                            if (i == hitY && currentX == hitX) //we reached the hit point and therefore are diagonal  
                            {
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(currentX, i, -1);
                            positions.Add(vec3);
                        }
                        for (var index = 0; index < positions.Count; index++)
                        {
                            var v = positions[index];
                            //Debug.Log("Vector: " + v);
                        }
                        //Debug.Log("all vectors accounted for!");
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }
                        //Debug.Log("diangoalpathisblocked: " + DiagonalPathBlocked(positions, teamId,hit) );
                        if (DiagonalPathBlocked(positions, teamId,hit))
                        {
                            return true; 
                        }
                        break;
                    
                    case (-1f, -1f):
                        for (int i = currentX - 1; i >= hitX; i--)
                        {
                            currentY--;
                            /*Debug.Log("Current X: " +i);
                            Debug.Log( "Current Y: " + currentY);*/
                            if (currentY == hitY && i == hitX) //we reached the hit point and therefore are diagonal  
                            {
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(i, currentY, -1);
                            positions.Add(vec3);;
                        }
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }

                        if (DiagonalPathBlocked(positions, teamId,hit) == true) 
                        {
                            return true; 
                        }
                        break;
                    default:
                        Debug.Log("I really messed up if we execute this");
                        break;
                }
            }
        }

        else if (pieceIdentity.pieceType is ChessPieceType.Player1Bishop or ChessPieceType.Player2Bishop)
        {
            /*Debug.Log(GetGridPosition(selectedPiece));
            Debug.Log(hit);*/
            if (hit.x == currentPiecePosition.x || hit.y == currentPiecePosition.y) return true;
                var isDiagonal = false;
                isDiagonal = Mathf.Abs(hit.x - currentPiecePosition.x) == 1 && Mathf.Abs(hit.y - currentPiecePosition.y) == 1; //adjacent move (distance of 1,1)
                if (isDiagonal == true)
                {
                    return false;
                }
                //let's make sure that we are diagonal 
                var hitX = hit.x;
                var hitY = hit.y;
                var currentX = currentPiecePosition.x;
                var currentY = currentPiecePosition.y;
                var valX = hitX - currentX;
                var valY = hitY - currentY;
                var signX = Mathf.Sign(valX);
                var signY = Mathf.Sign(valY);
                //Vector3Int[] vectorArray = new Vector3Int[10];
                List<Vector3Int> positions = new List<Vector3Int>();
                switch ((signX, signY))
                {
                    case (1f, 1f):
                        for (int i = currentX + 1; i <= hitX; i++)
                        {
                            currentY++;
                            if (currentY == hitY && i == hitX) //we reached the hit point and therefore are diagonal  
                            {   
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(i, currentY, -1);
                            positions.Add(vec3);
                        }
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }
                        if (DiagonalPathBlocked(positions, teamId,hit) == false)
                        {
                            return false; 
                        }

                        break;
                    case (1f, -1f):
                        for (int i = currentX + 1; i <= hitX; i++)
                        {
                            currentY--;
                            if (currentY == hitY && currentX == hitX) //we reached the hit point and therefore are diagonal  
                            {
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(i, currentY, -1);
                            positions.Add(vec3);
                        }
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }

                        if (DiagonalPathBlocked(positions, teamId,hit) == false)
                        {
                            return false; 
                        }
                        break;
                    case (-1f, 1f):
                        for (int i = currentY + 1; i <= hitY; i++)
                        {
                            currentX--;
                            if (i == hitY && currentX == hitX) //we reached the hit point and therefore are diagonal  
                            {
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(currentX, i, -1);
                            positions.Add(vec3);
                        }
                        for (var index = 0; index < positions.Count; index++)
                        {
                            var v = positions[index];
                            //Debug.Log("Vector: " + v);
                        }
                        //Debug.Log("all vectors accounted for!");
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }
                        //
                        //Debug.Log("diangoalpathisblocked: " + DiagonalPathBlocked(positions, teamId,hit) );
                        if (DiagonalPathBlocked(positions, teamId,hit))
                        {
                            return true; 
                        }
                        break;
                    case (-1f, -1f):
                        for (int i = currentX - 1; i >= hitX; i--)
                        {
                            currentY--;
                            /*Debug.Log("Current X: " +i);
                            Debug.Log( "Current Y: " + currentY);*/
                            if (currentY == hitY && i == hitX) //we reached the hit point and therefore are diagonal  
                            {
                                isDiagonal = true;
                            }
                            Vector3Int vec3 = new Vector3Int(i, currentY, -1);
                            positions.Add(vec3);;
                        }
                        if (isDiagonal == false)
                        {
                            positions.Clear();
                            return true;
                        }

                        if (DiagonalPathBlocked(positions, teamId,hit) == true) //might need to be true idk
                        {
                            return true; 
                        }
                        break;
                    default:
                        Debug.Log("I really messed up if we execute this");
                        break;
                }
        }
        
        return false;
    }

    private bool DiagonalPathBlocked(List<Vector3Int> vectors, List<GameObject> teamID, Vector3Int hitPosition)
    {
        List<GameObject> opposingTeam = null; 
        opposingTeam = teamID == Team1 ? Team2 : Team1;
        List<GameObject> friendly = null; 
        friendly = teamID == Team1 ? Team1 : Team2;
        //we need to create some logic to create a temp list to remove any piece from the list that is at the hit position
        List<GameObject> listWithoutHitposition = GetListWithoutHitPositionPiece(opposingTeam,hitPosition);
        //we now take out grid locations that lead to the hit and see if any pieces is occupying them
        //I should create a list that contains all the pieces later
        //Debug.Log("heehdsfhjasjkhf");
        for (var index = 0; index < vectors.Count; index++)
        {
            var v = vectors[index];
            for (int j = 0; j < friendly.Count; j++)
            {
                //this should be just team 1 since the hitposition is being counted
                Vector3Int gridPos = GetGridPosition(friendly[j]);
                gridPos.z = -1;
                //Debug.Log("GridPosition" +index + " : " + gridPos);
                if (v == gridPos && friendly[j].activeInHierarchy)
                {
                    Debug.Log("This is the blocking piece: " + friendly[j]);
                    return true;
                }
            }
            for (int j = 0; j < listWithoutHitposition.Count; j++)
            {   
                Vector3Int gridPos = GetGridPosition(listWithoutHitposition[j]);
                gridPos.z = -1;
                //if we reach the hit position on the check it no longer matters if an opposing piece is blocking it since we can capture it
                if (v == gridPos && listWithoutHitposition[j].activeInHierarchy)
                {
                    Debug.Log("This is the blocking piece (list wihtoutHitPosition): " + listWithoutHitposition[j]);
                    return true;
                }
            }
        }

        return false;
    }

    private List<GameObject> GetListWithoutHitPositionPiece(List<GameObject> opposingTeam, Vector3Int hitPosition)
    {
        List<GameObject> newList = opposingTeam;
        for (int i = 0; i < newList.Count; i++)
        {
            if (GetGridPosition(newList[i]) == hitPosition)
            {
                newList.RemoveAt(i);
            }
        }
        return newList;
    }

    private void Castle(GameObject currentPiece, GameObject referencePiece)
    {
        //the section of the code will check if we meet the correct conditions to castle
        var hitPieceIdentity = _referenceGameObject.GetComponent<PieceIdentity>();
        PieceIdentity piece = currentPiece.GetComponent<PieceIdentity>();
        var identity = piece.pieceType;
        //key will contain current piece type and the piece tag
        var key = (identity, currentPiece.tag,_referenceGameObject.tag);
        switch (key)
        {
            case (ChessPieceType.Player1Rook, "Player1", "Player1"):
            case (ChessPieceType.Player2Rook, "Player2", "Player2"):
                var rookComponent = currentPiece.GetComponent<RookMove>();
                var rookMove = rookComponent.hasMoved;
                var kingComponent = referencePiece.GetComponent<KingMove>();
                var destinationPosition = GetGridPosition(referencePiece);
                var kingMove = kingComponent.hasMoved;
                if (rookMove == false && kingMove == false)
                {
                    if (PathIsBlocked(currentPiece, destinationPosition) == false)
                    {
                        Debug.Log("We can castle");
                        rookMove = true;
                        kingMove = true;
                        canCastle = true;
                        hasCastled = true; 
                    }
                    else
                    {
                        Debug.Log("We cannot castle");
                    }
                }
                else
                {
                    Debug.Log("Pieces have already moved");
                }                                                               
                break;

            case (ChessPieceType.Player1King, "Player1", "Player1"):
            case (ChessPieceType.Player2King, "Player2", "Player2"):
                //current piece should be the king and hit should be the rook
                kingComponent = currentPiece.GetComponent<KingMove>();
                kingMove = kingComponent.hasMoved;
                rookComponent = referencePiece.GetComponent<RookMove>();
                destinationPosition = GetGridPosition(referencePiece);
                rookMove = rookComponent.hasMoved;
                if (rookMove == false && kingMove == false)
                {
                    if (PathIsBlocked(currentPiece, destinationPosition) == false)
                    {
                        Debug.Log("We can castle");
                        rookMove = true;
                        kingMove = true;
                        canCastle = true;
                        hasCastled = true;
                    }
                    else
                    {
                        Debug.Log("We cannot castle");
                    }
                }
                else
                {
                    Debug.Log("Pieces have already moved");
                }                                                               
                break;                     
        }
        //the code below will do the actual castling 
        if (canCastle != true) return;
        var firstPieceLocation = GetGridPosition(currentPiece);
        var secondPieceLocation = GetGridPosition(referencePiece);
        /*Debug.Log("Rook position: " + firstPieceLocation);
        Debug.Log("King position: " + secondPieceLocation);*/
        currentPiece.transform.position = _grid.GetCellCenterWorld(secondPieceLocation);
        referencePiece.transform.position = _grid.GetCellCenterWorld(firstPieceLocation);
    }

    private bool InCheck(GameObject teamKing, List<GameObject> enemyPieces)
    {
        var currentKingGridPosition = GetGridPosition(teamKing);
        var inCheck = teamKing.GetComponent<IsInCheck>();
        
        for (var index = 0; index < enemyPieces.Count; index++)
        {
            var teamGameObject = enemyPieces[index];
            var teamGameObjectGridPosition = GetGridPosition(teamGameObject);
            var pieceID = teamGameObject.GetComponent<PieceIdentity>();
            //Debug.Log("We are entering");
            if (IsValidPosition(teamGameObject,teamGameObjectGridPosition,currentKingGridPosition, false))
            {
                //Debug.Log("This is what has us in check: "+ teamGameObject);
                inCheck.isInCheck = true; 
                return true;
            }
        }

        inCheck.isInCheck = false;
        return false;
    }
    //the following function is to prevents us from moving the king to a check position
    private bool WillKingMovePlaceUsInCheck(GameObject CurrentPiece, Vector3Int hitPosition)
    {
        //we need to check if the path is blocked 
        PieceIdentity pieceIdentity = CurrentPiece.GetComponent<PieceIdentity>();
        List<GameObject> teamId;
        teamId = CurrentPiece.CompareTag("Player1") ? Team2 : Team1;
        List<GameObject> opposingPieces = new List<GameObject>(teamId);
        //the following code will check to see if any of the opposing pieces can reach the hit position 
        for (int i = 0; i < opposingPieces.Count; i++)
        {
            var currPiece = opposingPieces[i];
            var currentPosition = GetGridPosition(currPiece);
            //this will check if any of the enemy pieces can reach the new king position 
            if (IsValidPosition(currPiece, currentPosition, hitPosition, false))
            {
               // Debug.Log($"Index {i}: currPiece = {currPiece}");
               Debug.Log("This move will place us in check!!!!!!!!!!");
               return true;
            }
        }

        return false; 
    }
    private List<Vector3Int> GetAllValidPositionsforKing(List<Vector3Int> allValidPositions, Vector3Int currentPositionofKing)
    {
        List<Vector3Int> templist = new List<Vector3Int>();
        Vector3Int[] directions = new Vector3Int[]
        {
            //add this to the vector 3 to get all directions
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, -1, 0)
        };

        foreach (var dir in directions)
        {
            if (IsLocationOccupied(selectedPawn, currentPositionofKing + dir) == false)
            {
                templist.Add(currentPositionofKing + dir);
            }
        }

        
        foreach (var currentGridPosition in templist)
        {
            //the following if statement checks to see if we are in bounds for both x and y 
            if ((currentGridPosition.x >= -6 && currentGridPosition.x <= 1) &&
                (currentGridPosition.y >= 0 && currentGridPosition.y <= 7))
            {
                allValidPositions.Add(currentGridPosition);
            }
        }
        return allValidPositions; 
    }

    private bool IsLocationOccupied(GameObject selectedPiece, Vector3Int destination)
    {
        List<GameObject> teamId = null;
        teamId = (selectedPiece.CompareTag("Player1")) ? Team1 : Team2;
        foreach (GameObject friendly in teamId)
        {
           var gridLocation = GetGridPosition(friendly);
           if (gridLocation == destination)
           {
               //we are occupied 
               return true; 
           }
        }

        return false; 
    }

    private GameObject GetOccupyingPiece(GameObject selectedPiece, Vector3Int destination)
    {
        List<GameObject> teamId = null;
        teamId = (selectedPiece.CompareTag("Player1")) ? Team2 : Team1;
        foreach (GameObject enemy in teamId)
        {
            if (enemy.activeInHierarchy)
            {
                var gridLocation = GetGridPosition(enemy);
                if (gridLocation == destination)
                {
                    //we are occupied 
                    return enemy;
                }
            }
        }
        return null; 
    }


    //check if there is a move that can get us out of check, so we aren't forced to only move the king
    private bool WillPieceMoveGetUsOutOfCheck(GameObject hitPieceGameObject,List<GameObject> enemyPieces, Vector3Int hitPosition, Vector3Int kingPosition)
    {
        if (IsValidPosition(hitPieceGameObject, GetGridPosition(hitPieceGameObject), hitPosition, false))
        {
            //get enemy piece if it's at the hit position
            var pieceEliminated = GetOccupyingPiece(hitPieceGameObject,hitPosition);
            //Debug.Log("King Position: " + kingPosition);
            var opposingTeamPieces = enemyPieces;
            //Debug.Log("Hit Position" + hitPosition);
            if (pieceEliminated != null)
            {
                Debug.Log("Removed Deactivated Piece");
                Debug.Log("De-active Piece: " + pieceEliminated);
                pieceEliminated.SetActive(false);
            }
            
            //loop through enemy team to see if any pieces can reach the king
            for (int i = 0; i < opposingTeamPieces.Count; i++)
            {
                //check if any enemy piece can reach the king
                if (IsValidPosition(opposingTeamPieces[i], GetGridPosition(opposingTeamPieces[i]), kingPosition, false))
                {
                    //Debug.Log("Wtf are you: " + opposingTeamPieces[i]);
                    if (pieceEliminated != null)
                    {
                        //set active so we eliminate it in the later turn
                        pieceEliminated.SetActive(true);
                    }
                    //Debug.Log("This is invalid because our King will be placed in check!");
                    return true;
                }
            }
            if (pieceEliminated != null)
            {
                pieceEliminated.SetActive(true);
            }
        }
        //Debug.Log("This move does not place us in check ");
        return false; 
    }

    //this code is to prevent your own move from placing your king in check
    private bool WillMovePlaceOurKingInCheck(GameObject hitPieceGameObject,List<GameObject> enemyPieces, Vector3Int hitPosition, Vector3Int kingPosition)
    {
        // Objective 1: We need to remove the current piece at is location 
        // Objective 2: We need to have the opposing team loop check if getting to the king is valid now (call inCheck)

        var teamKing = Instance.state == GameStates.PlayerTurn1 ? kings : kings2;
        GameObject kingGameObject = teamKing[0];
        //Debug.Log("Deactived piece: " + hitPieceGameObject);
        hitPieceGameObject.SetActive(false);
        //Debug.Log("Is it true: " + (InCheck(kingGameObject, enemyPieces) == true));
        if (hitPieceGameObject.activeInHierarchy == false)
        {
            //Debug.Log("Still de-actived");
        }
        if (InCheck(kingGameObject, enemyPieces) == true)
        {
            Debug.Log("This move will place our king in check so its invalid");
            hitPieceGameObject.SetActive(true);
            return true; 
        }
        hitPieceGameObject.SetActive(true);
        return false; 
    }

    public void HandlePromotion(GameObject currentPiece)
    {
        StartCoroutine(HandlePromotionCoroutine(currentPiece));
    }

    private IEnumerator HandlePromotionCoroutine(GameObject currentPiece)
    {
        var ID = currentPiece.GetComponent<PieceIdentity>();
        var currentLocation = GetGridPosition(currentPiece);

        GridLocations.Team team = ID.pieceType == ChessPieceType.Player1Pawn
            ? GridLocations.Team.Team2
            : GridLocations.Team.Team1;

        HashSet<Vector3Int> promotionTiles = new HashSet<Vector3Int>(teamStartingPositions.GetTeamPositions(team));
        if (!promotionTiles.Contains(currentLocation))
        {
            //Debug.Log("No promotion!");
            yield break;
        }

        Debug.Log("Promotion Approved :)");
        currentPiece.SetActive(false);

        UI_Promotion.PromotedPiece selectedPiece = UI_Promotion.PromotedPiece.None;
        bool isDone = false;

        PromotionScreen.Setup((piece) => {
            selectedPiece = piece;
            isDone = true;
            Debug.Log("Completed");
        });
        // Wait until player picks a piece
        yield return new WaitUntil(() => isDone == true);
        Debug.Log("Current Location: " + currentLocation);
        switch (selectedPiece)
        {
            case UI_Promotion.PromotedPiece.Queen:
                SpawnQueen(currentLocation);
                break;
            case UI_Promotion.PromotedPiece.Rook:
                SpawnRook(currentLocation);
                break;
            case UI_Promotion.PromotedPiece.Bishop:
                SpawnBishop(currentLocation);
                break;
            case UI_Promotion.PromotedPiece.Knight:
                SpawnKnight(currentLocation);
                break;
        }

        PromotionScreen.Clean();
    }

    // don't need now
    private bool hasOppositeSideBeReached(GameObject currentPiece)
    {
        var ID = currentPiece.GetComponent<PieceIdentity>();
        GridLocations.Team team = ID.pieceType == ChessPieceType.Player1Pawn
            ? GridLocations.Team.Team2
            : GridLocations.Team.Team1;
        HashSet<Vector3Int> promotionTiles = new HashSet<Vector3Int>(teamStartingPositions.GetTeamPositions(team));
        if (promotionTiles.Contains(GetGridPosition(currentPiece)) == true)
        {
            return true; 
        }

        return false; 
    }

    private void EnableTargeter(List<GameObject> teamID)
    {
        for (int i = 0; i < teamID.Count; i++)
        {
            teamID[i].GetComponent<Targeter>().enabled = true;
        }
    }
    private void DisableTargeter(List<GameObject> teamID)
    {
        for (int i = 0; i < teamID.Count; i++)
        {
            teamID[i].GetComponent<Targeter>().enabled = false;
        }
    }
    
    private void AdjustPieceHeight(GameObject item)
    {
        var piece = item.GetComponent<PieceIdentity>();
        switch (piece.pieceType)
        {
            case ChessPieceType.Player1Rook:
            case ChessPieceType.Player2Rook:
            case ChessPieceType.Player1Bishop:
            case ChessPieceType.Player2Bishop:
                item.transform.position = new Vector3(item.transform.position.x, .2f, item.transform.position.z);
                break;
            case ChessPieceType.Player1Knight:
            case ChessPieceType.Player2Knight:
            case ChessPieceType.Player2Queen:
            case ChessPieceType.Player1Queen:
                item.transform.position = new Vector3(item.transform.position.x, .22f, item.transform.position.z);
                break;
            case ChessPieceType.Player1King:
            case ChessPieceType.Player2King:
                item.transform.position = new Vector3(item.transform.position.x, .3f, item.transform.position.z);
                break;
            default:
                item.transform.position = new Vector3(item.transform.position.x, 0, item.transform.position.z);
                break;
        }
    }

    public void SpawnQueen(Vector3Int newPosition)
    {
        Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, 3, -1));
        Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
        worldPosition2.y = 0.202f;
        transform.position = worldPosition2;
        Vector3 worldPosition = _grid.GetCellCenterWorld(newPosition);
        worldPosition.y = .202f;
        GameObject queenInstance = Instantiate(_queen, worldPosition, Quaternion.identity);
        queenInstance.GetComponent<Renderer>().material = team1;
        BoxCollider boxCollider = queenInstance.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        Targeter targeter = queenInstance.AddComponent<Targeter>();
        targeter.renderer = queenInstance.GetComponent<Renderer>();
        queenInstance.transform.localScale = new Vector3(15f, 15f, 15f);
        boxCollider.size = new Vector3(.04f, .04f, .04f);
        PieceIdentity id = queenInstance.AddComponent<PieceIdentity>();
        Rigidbody rigidbody = queenInstance.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        queenInstance.AddComponent<Overlap>();
        Debug.Log("Instance: " + Instance.state);
        //this has to be used since the gamestate will change before this is called
        if (Instance.state == GameStates.PlayerTurn2)
        {
            id.pieceType = ChessPieceType.Player1Queen;
            queenInstance.tag = "Player1";
            
            queenInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            queens.Add(queenInstance);
            Team1.Add(queenInstance);
        }
        else
        {
            id.pieceType = ChessPieceType.Player2Queen;
            queenInstance.tag = "Player2";
            queenInstance.GetComponent<Renderer>().material = team2;
            queenInstance.transform.rotation = Quaternion.Euler(-90,90,0);
            queens.Add(queenInstance);
            Team2.Add(queenInstance);
        }
        queenInstance.layer = LayerMask.NameToLayer("Chessboard");
        allPieces.Add(queenInstance);
    }

    public void SpawnRook(Vector3Int newPosition)
    {
        //Vector3 worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, i, -1));
        //Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
        //worldPosition2.y = 0.128f;
        Vector3 worldPosition = _grid.GetCellCenterWorld(newPosition);
        worldPosition.y = .128f;
        //transform.position = worldPosition2; // Set the position of the rook to the world position
        GameObject rookInstance = Instantiate(_rook, worldPosition, Quaternion.identity);
        rookInstance.GetComponent<Renderer>().material = team1;
        BoxCollider boxCollider = rookInstance.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        Targeter targeter = rookInstance.AddComponent<Targeter>();
        targeter.renderer = rookInstance.GetComponent<Renderer>();
        Rigidbody rigidbody = rookInstance.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rookInstance.AddComponent<Overlap>();
        rookInstance.transform.localScale = new Vector3(15f, 15f, 15f);
        boxCollider.size = new Vector3(.04f, .04f, .04f);
        PieceIdentity id = rookInstance.AddComponent<PieceIdentity>();
        RookMove pawnComponent = rookInstance.AddComponent<RookMove>();
        if (Instance.state == GameStates.PlayerTurn2)
        {
            rookInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            id.pieceType = ChessPieceType.Player1Rook;
            rookInstance.tag = "Player1";
            rooks.Add(rookInstance);
            Team1.Add(rookInstance);
        }
        else
        {
            rookInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            id.pieceType = ChessPieceType.Player2Rook;
            rookInstance.GetComponent<Renderer>().material = team2;
            rooks.Add(rookInstance);
            Team2.Add(rookInstance);
            rookInstance.tag = "Player2";
        }
        
        rookInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
        allPieces.Add(rookInstance);
    }

    public void SpawnBishop(Vector3Int newPosition)
    {
        //Vector3Int gridPosition = _grid.WorldToCell(worldPosition2);
        //worldPosition2.y = .191f;
        //transform.position = worldPosition2; // Set the position of the rook to the world position
        Vector3 worldPosition = _grid.GetCellCenterWorld(newPosition);
        worldPosition.y = .191f;
        GameObject bishopInstance = Instantiate(_bishop, worldPosition, Quaternion.identity);
        bishopInstance.GetComponent<Renderer>().material = team1;
        BoxCollider boxCollider = bishopInstance.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        Targeter targeter = bishopInstance.AddComponent<Targeter>();
        targeter.renderer = bishopInstance.GetComponent<Renderer>();
        bishopInstance.transform.localScale = new Vector3(15f, 15f, 15f);
        boxCollider.size = new Vector3(.04f, .04f, .04f);
        Rigidbody rigidbody = bishopInstance.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        bishopInstance.AddComponent<Overlap>();
        PieceIdentity id = bishopInstance.AddComponent<PieceIdentity>();
        if (Instance.state == GameStates.PlayerTurn2)
        {
            id.pieceType = ChessPieceType.Player1Bishop;
            bishopInstance.tag = "Player1";
            bishopInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            bishops.Add(bishopInstance);
            Team1.Add(bishopInstance);
        }

        else
        {
            id.pieceType = ChessPieceType.Player2Bishop;
            bishopInstance.tag = "Player2";
            bishopInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);
            bishopInstance.GetComponent<Renderer>().material = team2;
            bishops.Add(bishopInstance);
            Team2.Add(bishopInstance);
        }

        bishopInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
        allPieces.Add(bishopInstance);
    }

    public void SpawnKnight(Vector3Int newPosition)
    {
        Vector3 worldPosition = _grid.GetCellCenterWorld(newPosition);
        worldPosition.y = .212f;
        GameObject knightInstance = Instantiate(_knight, worldPosition, Quaternion.identity);
        knightInstance.GetComponent<Renderer>().material = team1;
        BoxCollider boxCollider = knightInstance.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        Targeter targeter = knightInstance.AddComponent<Targeter>();
        targeter.renderer = knightInstance.GetComponent<Renderer>();
        knightInstance.transform.localScale = new Vector3(15f, 15f, 15f);
        boxCollider.size = new Vector3(.04f, .04f, .04f);
        PieceIdentity id = knightInstance.AddComponent<PieceIdentity>();
        knightInstance.AddComponent<Overlap>();
        Rigidbody rigidbody = knightInstance.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        if (Instance.state == GameStates.PlayerTurn2)
        {
            id.pieceType = ChessPieceType.Player1Knight;
            knightInstance.tag = "Player1";
            knightInstance.transform.rotation = Quaternion.Euler(-90, 90, 0);
            knights.Add(knightInstance);
            Team1.Add(knightInstance);
        }
        else
        {
            id.pieceType = ChessPieceType.Player2Knight;
            knightInstance.tag = "Player2";
            knightInstance.transform.rotation = Quaternion.Euler(-90, -90, 0);
            knights.Add(knightInstance);
            Team2.Add(knightInstance);
        }
        knightInstance.layer = LayerMask.NameToLayer("Chessboard"); //for the raycast
        allPieces.Add(knightInstance);
    }

    public void GameOver(int teamID)
    {
        GameOverScreen.Setup(teamID);
    }
    

}