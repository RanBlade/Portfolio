﻿using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace UtilityScripts
{
    //enum for game states
    public enum MAINSTATES
    {
        menu,
        playing,
        quit,
        pause,
        loading
    }
    public enum PLAYINGSTATES
    {
        playerTurn,
        win,
        loss,
        newGame,
        HighScore,
        mainMenu
    }
    public enum NEWGAMETYPE
    {
        newGame,
        winGame,
        LoseGame
    }
    public enum PLAYAGAINFLAG
    {
        yes,
        no,
        notSelected
    }

    [System.Serializable]
    public class WordLibEntry
    {
        public string word;
        public string definition;
        //place a image here or a list of images to be use to build the image in the word lib.
        public Texture image;
        public int wordSize;

        public WordLibEntry()
        {

        }
        public int GetWordSize()
        {
            return word.Length;
        }

    }
    [System.Serializable]
    public class LeaderBoardEntry
    {
        public string name;
        public int score;

        public LeaderBoardEntry()
        {
            name = null;
            score = 0;
        }
    }

}
