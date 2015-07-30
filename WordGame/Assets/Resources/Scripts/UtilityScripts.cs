using UnityEngine;
using System.Collections;



namespace UtilityScripts {

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
}
