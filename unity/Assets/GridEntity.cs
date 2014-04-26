using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class GridEntity : MonoBehaviour
{
    public int row;
    public int col;

    public Int2 pos { get { return new Int2(row,col); } }

    [HideInInspector]
    public MapSpawner host;

    public bool CanMove( Int2 delta )
    {
        return CanMove(delta.x, delta.y);
    }
    public bool CanMove( int dr, int dc )
    {
        return host.grid.CheckBounds( row+dr, col+dc )
                && host.grid[row+dr, col+dc] == null;
    }

    public void Move( int dr, int dc )
    {
        host.grid[row, col] = null;
        row += dr;
        col += dc;
        host.grid[row, col] = this;
    }

    public bool TryMove( Int2 delta )
    {
        return TryMove( delta.row, delta.col );
    }

    public bool TryMove( int dr, int dc )
    {
        if( CanMove(dr,dc) )
        {
            Move(dr,dc);
            return true;
        }
        else
        {
            return false;
        }
    }

    void LateUpdate()
    {
        transform.localPosition = new Vector3(col, -row, 0); 
    }

    public GridEntity Peek( int dr, int dc )
    {
        if( host.grid.CheckBounds(row+dr, col+dc) )
        {
            return host.grid[row+dr, col+dc];
        }
        else
            return null;
    }
}
