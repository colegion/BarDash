using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class LevelGenerator : MonoBehaviour
    {
        private List<Cell> _cells;
        
        public void GenerateLevel(LevelData levelData)
        {
            _cells = new List<Cell>();

            var levelCells = levelData.cells;

            foreach (var cell in levelCells)
            {
                
            }
        }
    }
}

