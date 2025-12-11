using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Node
{
    public int x;
    public int y;

    public bool walkable;
    public GameObject tile;

    public float gCost; 
    public float hCost;
    public Node parent;

    public float fCost => gCost + hCost;

    public Node(int x, int y, bool walkable, GameObject tile)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.tile = tile;

        gCost = float.PositiveInfinity;
        hCost = 0f;
        parent = null;
    }

    public void ResetCosts()
    {
        gCost = float.PositiveInfinity;
        hCost = 0f;
    }
}


public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material walkableMaterial;
    [SerializeField] private Material wallMaterial;

    private Node[,] nodes;
    private Dictionary<GameObject, Node> tileToNode = new();

    private InputAction clickAction;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    private void Awake()
    {
        GenerateGrid();
    }

    private void OnEnable()
    {
        clickAction = new InputAction(name: "Click", type: InputActionType.Button, binding: "<Mouse>/leftButton");

        clickAction.performed += OnClickPerformed; 
        clickAction.Enable();
    }

    private void OnDisable()
    {
        if(clickAction != null)
        {
            clickAction.performed -= OnClickPerformed;
            clickAction.Disable();
        }
    }


    private void GenerateGrid()
    {
        nodes = new Node[width, height]; 

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, 0f, y * cellSize);
                GameObject tileGO = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tileGO.name = $"Tile_{x}_{y}";

                // Create node
                Node node = new Node(x, y, true, tileGO);
                nodes[x, y] = node;
                tileToNode[tileGO] = node;

                SetTileMaterial(node, walkableMaterial);
            }
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {

        Camera cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;
            if(tileToNode.TryGetValue(clicked, out Node node))
            {
                bool newWalkable = !node.walkable;
                SetWalkable(node, newWalkable);
            }
        }  
    }

    public Node GetNode(int x, int y)
    {
        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return nodes[x, y];
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.z / cellSize); 
        return GetNode(x, y);
    }

    public IEnumerable<Node> GetNeighbours(Node node, bool allowDiagonals = false)
    {
        int x = node.x;
        int y = node.y;

        // 4-neighbour
        Node right = GetNode(x + 1, y);
        Node left = GetNode(x - 1, y);
        Node up = GetNode(x, y + 1);
        Node down = GetNode(x, y - 1);

        if (right != null) yield return right;
        if (left != null) yield return left;
        if (up != null) yield return up;
        if (down != null) yield return down;

        if (allowDiagonals)
        {
            yield return GetNode(x + 1, y + 1);
            yield return GetNode(x - 1, y + 1);
            yield return GetNode(x + 1, y - 1);
            yield return GetNode(x - 1, y - 1);
        }
    }

    public void SetWalkable(Node node, bool walkable)
    {
        node.walkable = walkable;
        SetTileMaterial(node, walkable ? walkableMaterial : wallMaterial);
    }

    private void SetTileMaterial(Node node, Material mat)
    {
        var renderer = node.tile.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.material = mat;
        }
    }

    
}

