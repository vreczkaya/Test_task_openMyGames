using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class Level
    {
        public List<char> Letters { get; private set; }
        public List<int> Positions { get; private set; }

        public int GridSize {  get; private set; }

        private int _cellsAmount;

        public Level()
        {
            Letters = new List<char>();
            Positions = new List<int>();
        }

        public void AddPositions(string[] positions)
        {
            foreach (string position in positions)
            {
                Positions.Add(int.Parse(position));
            }
        }

        public void AddLetters(string word)
        {
            foreach (char letter in word.ToCharArray()) 
            { 
                Letters.Add(letter);
            };
        }

        public void PrepareLevel()
        {
            Positions.Distinct();
            SetGridSize();
            CheckIndexes();
            BubbleSort();

            if (_cellsAmount > Positions.Count)
            {
                FillEmptyCells();
            }
        }

        private void CheckIndexes()
        {
            for (int i = 0; i < Positions.Count; i++)
            {
                if (Positions[i] < 0 || Positions[i] > _cellsAmount - 1)
                {
                    Positions.Remove(Positions[i]);
                }
            }
            if (Letters.Count > Positions.Count)
            {
                FitLackOfIndexes();
            }
        }

        private void SetGridSize()
        {
            double testGridSize = Math.Sqrt(Letters.Count);

            if (testGridSize - (int)testGridSize != 0) 
            {
                GridSize = (int)testGridSize + 1;
            }
            else
            {
                GridSize = (int)testGridSize;
            }
            _cellsAmount = GridSize * GridSize;
        }

        private void FitLackOfIndexes()
        {
            List<int> newPositions = new List<int>(Positions);
            newPositions.Sort();

            for (int i = 1; i < newPositions.Count; i++)
            {
                if (newPositions[0] != 0)
                {
                    Positions.Add(0);
                    return;
                }
                if (newPositions[i] - newPositions[i - 1] != 1)
                {
                    Positions.Add(newPositions[i] - newPositions[i - 1]);
                }
            }
        }

        private void BubbleSort()
        {
            for (int i = 0; i < Letters.Count - 1; i++)
            {
                for (int j = 0; j < Letters.Count - i - 1; j++)
                {
                    if (Positions[j] > Positions[j + 1])
                    {
                        Swap<int>(Positions, j, j + 1);
                        Swap<char>(Letters, j, j + 1);
                    }
                }
            }
        }

        private void Swap<T>(List<T> list, int i, int j) 
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private void FillEmptyCells()
        {
            for (int i = Positions.Count - 1; i < GridSize * GridSize; i++)
            {
                Positions.Add(i + 1);
                Letters.Add(Letters[i - 3]); //ближайша€ несоседн€€ буква
            }
        }
    }

    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private string _pathToLevel = "Fillwords\\pack_0";
        private string _pathToDictionary = "Fillwords\\words_list";

        private List<string> _dictionary;
        private List<string> _levels;

        public ProviderFillwordLevel()
        {
            _dictionary = Resources.Load<TextAsset>(_pathToDictionary).ToString().Split('\n').ToList();

            _levels = Resources.Load<TextAsset>(_pathToLevel).text.ToString().Split('\n').ToList();
        }

        public GridFillWords LoadModel(int index) 
        {
            Level level = ParseLevel(GetLevelDescription(index));

            GridFillWords gridFillWords = new GridFillWords(new Vector2Int(level.GridSize, level.GridSize));

            int lettersIndex = 0;

            for (int i = 0; i < level.GridSize; i++)
            {
                for (int j = 0; j < level.GridSize; j++)
                {
                    gridFillWords.Set(i, j, new CharGridModel(level.Letters[lettersIndex]));
                    lettersIndex++;
                }
            }

            return gridFillWords;
        }

        private string GetLevelDescription(int index)
        {
            string currentLevelDescription = string.Empty;
            
            bool levelIsAvailable = !(_levels == null) && !(_levels.Count == 0) &&
            index != 0 && index <= _levels.Count;

            _levels?.Distinct();

            while (string.IsNullOrEmpty(currentLevelDescription) && levelIsAvailable)
            {
                currentLevelDescription = _levels[index - 1];
                index++;
            }

            if (!levelIsAvailable || string.IsNullOrEmpty(currentLevelDescription))
            {
                throw new Exception();
            }
            
            return currentLevelDescription;
        }

        public Level ParseLevel(string levelDescription)
        {
            string[] parts = levelDescription.Split(' ');
            if (parts.Length == 0) return null;

            Level level = new Level();
            
            foreach (string part in parts)
            {
                if (part.Contains(";"))
                {
                    level.AddPositions(part.Split(';'));
                }
                else
                {
                    level.AddLetters(GetWordFromDictionary(int.Parse(part)));
                }
            }

            level.PrepareLevel();

            return level;
        }

        private string GetWordFromDictionary(int index)
        {
            if (index >= 0 && index < _dictionary.Count)
            {
                return _dictionary[index].Remove(_dictionary[index].Length - 1);
            }
            
            return null;
        }
    }
}