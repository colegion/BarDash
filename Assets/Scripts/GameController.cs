using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class GameController : MonoBehaviour
{
   private GameController _instance;
   private LevelGenerator _levelGenerator;
   private Dictionary<ItemType, Cell[,]> _grid;
   
   public GameController Instance
   {
      get
      {
         if (_instance == null)
         {
            _instance = new GameObject("GameController").AddComponent<GameController>();
         }
         
         return _instance;
      }
   }

   private void Start()
   {
      _levelGenerator = new LevelGenerator();
      LoadLevel();
   }

   private void LoadLevel()
   {
      //todo: Read json by player pref saved level index here.
      _grid = _levelGenerator.GenerateLevel(new LevelData());
   }

   private void OnEnable()
   {
      AddListeners();
   }

   private void OnDisable()
   {
      RemoveListeners();
   }

   private void AddListeners()
   {
      
   }

   private void RemoveListeners()
   {
      
   }
}
