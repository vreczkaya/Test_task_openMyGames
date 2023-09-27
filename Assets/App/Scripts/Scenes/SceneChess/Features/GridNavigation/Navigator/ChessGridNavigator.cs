using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            Vector2Int[] possiblePositions = SetPossibleCells(unit);

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(from);

            int[,] distance = new int[grid.Size.x, grid.Size.y];
            Vector2Int[,] predecessor = new Vector2Int[grid.Size.x, grid.Size.y];

            for (int i = 0; i < grid.Size.x; i++)
            {
                for (int j = 0; j < grid.Size.x; j++)
                {
                    distance[i, j] = -1;
                }
            }
            distance[from.x, from.y] = 0;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                int x = current.x;
                int y = current.y;

                if (x == to.x && y == to.y)
                {
                    List<Vector2Int> path = new List<Vector2Int>();
                    while (x != from.x || y != from.y)
                    {
                        path.Add(new Vector2Int(x, y));
                        current = predecessor[x, y];
                        x = current.x;
                        y = current.y;
                    }
                    
                    path.Reverse(); 
                    return path;
                }

                for (int i = 0; i < possiblePositions.Length; i++)
                {
                    for (int j = 1; j < grid.Size.x; j++)
                    {
                        Vector2Int newPosition = current + possiblePositions[i] * j;

                        if (IsValid(newPosition, grid.Size.x) && distance[newPosition.x, newPosition.y] == -1 && grid.IsEmptyCell(new Vector2Int(newPosition.x, newPosition.y)))
                        {
                            distance[newPosition.x, newPosition.y] = distance[x, y] + 1;
                            predecessor[newPosition.x, newPosition.y] = new Vector2Int(x, y);
                            queue.Enqueue(newPosition);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return new List<Vector2Int>();
        }

        private Vector2Int[] SetPossibleCells(ChessUnitType unit)
        {
            return unit switch
            {
                ChessUnitType.Pon => new Vector2Int[2]
                {
                    new Vector2Int(0, -1), new Vector2Int(0, 1)
                },
                ChessUnitType.Rook => new Vector2Int[4]
                {
                    new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)
                },
                ChessUnitType.Bishop => new Vector2Int[4]
                {
                    new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
                },
                ChessUnitType.King or ChessUnitType.Queen => new Vector2Int[8]
                {
                    new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(-1, -1),
                    new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1)
                },
                ChessUnitType.Knight => new Vector2Int[8]
                {
                    new Vector2Int(-1, -2), new Vector2Int(1, -2), new Vector2Int(-2, -1), new Vector2Int(2, -1),
                    new Vector2Int(-1, 2), new Vector2Int(1, 2), new Vector2Int(-2, 1), new Vector2Int(2, 1)
                },
                _ => null
            };
        }

        private static bool IsValid(Vector2Int position, int gridSize) => position.x >= 0 && position.y >= 0 && position.x < gridSize && position.y < gridSize;
    }
}
