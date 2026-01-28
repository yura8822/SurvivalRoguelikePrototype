using System.Collections.Generic;
using UnityEngine;

public class SpatialGridService : MonoBehaviour
{
    [Header("Grid settings")] 
    [SerializeField] private float cellSize = 3;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;

    [Header("Debug visualization")] 
    [SerializeField] private bool showGrid = true;
    [SerializeField] private bool showOccupiedCells = true;
    [SerializeField] private Color gridColor = Color.gray;
    [SerializeField] private Color occupiedCellColor = Color.red;
    
    private Dictionary<Vector2Int, GridCell_> _cells;
    private int numCellsX;
    private int numCellsY;
    
    public float CellSize => cellSize;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;  

    public void Initialization()
    {
        numCellsX = (int)(gridWidth / cellSize);
        numCellsY = (int)(gridHeight / cellSize);

        _cells = new Dictionary<Vector2Int, GridCell_>();

        for (int x = 0; x < numCellsX; x++)
        {
            for (int y = 0; y < numCellsY; y++)
            {
                _cells[new Vector2Int(x, y)] = new GridCell_();
            }
        }
    }

    public Vector2Int GetCellCoords(Vector3 worldPos)
    {
        int cellX = Mathf.FloorToInt(worldPos.x / cellSize);
        int cellY = Mathf.FloorToInt(worldPos.y / cellSize);
        
        cellX = Mathf.Clamp(cellX, 0, numCellsX - 1);
        cellY = Mathf.Clamp(cellY, 0, numCellsY - 1);
        
        return new Vector2Int(cellX, cellY);
    }

    public bool IsCellOccupied(Vector2Int cellCoords, IGridElement ignore = null)
    {
        // За пределами сетки считается занятой
        if (!_cells.TryGetValue(cellCoords, out GridCell_ cell))
            return true;
        
        foreach (IGridElement element in cell.elements)
        {
            if (ignore != element)
                return true;
        }

        return false;
    }
    
    public List<IGridElement> GetElementsInCell(Vector2Int cellCoords)
    {
        if (_cells.TryGetValue(cellCoords, out GridCell_ cell))
        {
            return cell.elements;
        }
    
        return new List<IGridElement>();
    }
    
    public List<IGridElement> GetElementsInRadius(Vector3 centerPos, float radius)
    {
        var result = new List<IGridElement>();
        var centerCell = GetCellCoords(centerPos);

        int cellRadius = Mathf.CeilToInt(radius / cellSize);
        float radiusSqr = radius * radius; 

        for (int x = centerCell.x - cellRadius; x <= centerCell.x + cellRadius; x++)
        {
            for (int y = centerCell.y - cellRadius; y <= centerCell.y + cellRadius; y++)
            {
                var coords = new Vector2Int(x, y);

                if (!_cells.TryGetValue(coords, out var cell))
                    continue;

                foreach (IGridElement element in cell.elements)
                {
                    Vector3 delta = element.Transform.position - centerPos;
                    if (delta.sqrMagnitude <= radiusSqr) result.Add(element);
                }
            }
        }

        return result;
    }

    public void AddElement(IGridElement element)
    {
        Vector2Int cellCoords = GetCellCoords(element.Transform.position);

        if (_cells.TryGetValue(cellCoords, out GridCell_ cell))
        {
            cell.elements.Add(element);
            element.CurrentCell = cellCoords;
            
        }
    }

    public void RemoveElement(IGridElement element)
    {
        if (_cells.TryGetValue(element.CurrentCell, out GridCell_ cell))
        {
            cell.elements.Remove(element);
        }
    }
    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGrid) return;
        DrawGridLines();
        DrawCellOccupancy();
    }

    private void DrawGridLines()
    {
        Gizmos.color = gridColor;

        int numX = (int)(gridWidth / cellSize);
        int numY = (int)(gridHeight / cellSize);

        // Вертикальные линии
        for (int x = 0; x <= numX; x++)
        {
            float xPos = x * cellSize;
            Vector3 start = new Vector3(xPos, 0, 0);
            Vector3 end = new Vector3(xPos, gridHeight - gridHeight % cellSize, 0);
            Gizmos.DrawLine(start, end);
        }

        // Горизонтальные линии
        for (int y = 0; y <= numY; y++)
        {
            float yPos = y * cellSize;
            Vector3 start = new Vector3(0, yPos, 0);
            Vector3 end = new Vector3(gridWidth - gridWidth % cellSize, yPos, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    private void DrawCellOccupancy()
    {
        if (_cells == null) return;

        foreach (var kvp in _cells)
        {
            Vector2Int cellCoords = kvp.Key;
            GridCell_ cell = kvp.Value;

            if (cell.elements.Count > 0)
            {
                // Подсвечиваем занятую ячейку
                Gizmos.color = occupiedCellColor;
                
                Vector3 cellCenter = new Vector3(
                    cellCoords.x * cellSize + cellSize * .5f, 
                    cellCoords.y * cellSize + cellSize * .5f, 
                    0);
                
                Gizmos.DrawCube(cellCenter, new Vector3(cellSize, cellSize, .1f));
                
                // Рисуем цифру, отображающую количество элементов в ячейке
                UnityEditor.Handles.Label(
                    cellCenter, 
                    cell.elements.Count.ToString(),
                    new GUIStyle() 
                    { 
                        normal = new GUIStyleState() { textColor = Color.black },
                        fontSize = 12,
                        fontStyle = FontStyle.Bold
                    }
                );
                
            }
            
        }
    }
#endif
}

public class GridCell_
{
    public List<IGridElement> elements = new List<IGridElement>();
}

public interface IGridElement
{
    public Transform Transform { get; }
    public Vector2Int CurrentCell { get; set; }
}
