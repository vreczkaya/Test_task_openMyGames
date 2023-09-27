using System.IO;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using Mono.Cecil;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        private string _pathToLevels = "WordSearch\\Levels\\";
        private string _fileExtension = ".json";

        public LevelInfo LoadLevelData(int levelIndex)
        {
            return JsonConvert.DeserializeObject<LevelInfo>(Resources.Load<TextAsset>(_pathToLevels + levelIndex.ToString()).text);
        }
    }
}