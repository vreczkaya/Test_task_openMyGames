using System;
using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);


            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            Dictionary<char, int> letterCount = new Dictionary<char, int>();

            int maxRepeatCount = 0;
            foreach (string word in words)
            {
                Dictionary<char, int> wordLetterCount = new Dictionary<char, int>();
                foreach (char letter in word)
                {
                    if (wordLetterCount.ContainsKey(letter))
                    {
                        wordLetterCount[letter]++;
                    }
                    else
                    {
                        wordLetterCount[letter] = 1;
                    }

                    if (!letterCount.ContainsKey(letter))
                    {
                        letterCount[letter] = 0;
                    }

                    letterCount[letter] = Math.Max(letterCount[letter], wordLetterCount[letter]);
                    maxRepeatCount = Math.Max(maxRepeatCount, letterCount[letter]);
                }
            }

            List<char> result = new List<char>();
            foreach (var kvp in letterCount)
            {
                char letter = kvp.Key;

                for (int i = 0; i < kvp.Value; i++)
                {
                    result.Add(letter);
                }
            }

            return result;
        }

    }
}