using UnityEngine;
using System.Collections;
using System.Collections.Generic;  
using System;
using System.Linq;

public class PantryManager {
    // keeps a list of the old pantry to tell the app to save the new list
    public static List<Ing> oldPantryItems = new List<Ing>();
    // keeps a list of all the stuff you have in your pantry list
    public static List<Ing> myIngridents = new List<Ing>();

	public static List<int> myLikedRecipes = new List<int>();

    public static List<Ing> neededIngridents = new List<Ing>();

    public static List<int> myUploadedRecipes = new List<int>();


    /// <summary>
    /// Adds items into your pantry but inputing a new Ing
    /// </summary>
    /// <param name="newItem">Ing holds information on the id and name of the ingrident</param>
    public static void AddToNeededPantry( Ing newItem )
    {
        bool hasAlredy = false;
        Debug.Log(newItem.name + " , " + newItem.id);

        if (neededIngridents.Contains(newItem))
        {
            Debug.Log("Already contains in needed");
            hasAlredy = true;
        }

        for (int i = 0; i < RecipeManager.allRecipes[GUIManager.instance.recipeSelected].ings.Count; i++)
        {
            for (int x = 0; x < PantryManager.myIngridents.Count(); x++)
            {
                if (PantryManager.myIngridents[x].id == newItem.id)//RecipeManager.allRecipes[GUIManager.instance.recipeSelected].ings[i].id)
                {
                    Debug.Log("Already have " + RecipeManager.allRecipes[GUIManager.instance.recipeSelected].ings[i].name);
                    hasAlredy = true;
                    break;
                }
            }
        }

        if (!hasAlredy)
        {
            for (int i = 0; i < neededIngridents.Count; i++)
            {
                if (newItem.id == neededIngridents[i].id)
                {
                    Debug.Log("Needed already have " + newItem.name);
                    hasAlredy = true;
                    break;
                }
            }
        }

        if (!hasAlredy)
        {
            Debug.Log("Adding: " + newItem.name + " , " + newItem.id);
            neededIngridents.Add(newItem);
        }
    }

    /// <summary>
    /// Adds items into your pantry but inputing a new Ing
    /// </summary>
    /// <param name="newItem">Ing holds information on the id and name of the ingrident</param>
    public static void AddToMyPantry( Ing newItem )
    {
        bool hasAlredy = false;
        Debug.Log(newItem.name + " , " + newItem.id);

        if (myIngridents.Count > 0)
        {
            for (int i = 0; i < myIngridents.Count; i++)
            {
                // if there is a match return and don't add anything
                if (myIngridents[i].id == newItem.id)
                {
                    hasAlredy = true;
                    break;
                }
            }
        }

        //for (int i = 0; i < neededIngridents.Count; i++)
        //{
        //    // if there is a match return and don't add anything
        //    if (neededIngridents[i].id == newItem.id)
        //    {
        //        Debug.Log("Contains: " + newItem.name);
        //        neededIngridents.Remove(newItem);
        //        break;
        //    }
        //}

        if(!hasAlredy)
            myIngridents.Add(newItem);
    }

	/// <summary>
	/// Adds items into your liked recipes list
	/// </summary>
	/// <param name="newItem">id of the recipe</param>
	public static void AddToLikeRecipes( int newItem )
	{
		bool hasAlredy = false;
		
		if (myIngridents.Count > 0)
		{
			for (int i = 0; i < myLikedRecipes.Count; i++)
			{
				// if there is a match return and don't add anything
				if (myLikedRecipes[i] == newItem)
				{
					hasAlredy = true;
					break;
				}
			}
		}
		
		if(!hasAlredy)
			myLikedRecipes.Add(newItem);
	}
	public static bool AlreadyLiked(int id)
	{
		if (myLikedRecipes.Contains (id))
			return true;

		return false;
	}

	public static void RemoveFromLikedRecipes(int index)
	{
		myLikedRecipes.Remove(index);
	}

    /// <summary>
    /// Removes an Ing from your needed pantry
    /// </summary>
    /// <param name="index">put in the index the Ing is at to remove</param>
    public static void RemoveFromNeededPantry( int index )
    {
        neededIngridents.RemoveAt(index);
    }

    /// <summary>
    /// Removes an Ing from your pantry
    /// </summary>
    /// <param name="index">put in the index the Ing is at to remove</param>
    public static void RemoveFromMyPantry(int index)
    {
        //myPantryItems.RemoveAt(index);
        myIngridents.RemoveAt(index);
        //myFoodItems.RemoveAt(index);
    }
    public static void RemoveFromMyPantry(Ing index)
    {
        //myPantryItems.Remove(index);
        for (int i = 0; i < myIngridents.Count; i++)
        {
            if (index.id == myIngridents[i].id)
            {
                myIngridents.RemoveAt(i);
            }
        }

        //myIngridents.Remove(index);
        //myFoodItems.Remove(
    }

    /// <summary>
    /// Save your pantry information
    /// </summary>
    public static void Save()
    {
        // check the username and login information first
        if (LoginGUI.instance.Username() == "Guest")
        {
            //Debug.Log("Saving");
            // local save
            PlayerPrefs.SetString("myPantryItems" + LoginGUI.instance.Username(), SaveInventory(myIngridents));
			PlayerPrefs.SetString("myLikedRecipes" + LoginGUI.instance.Username(), SaveInt(myLikedRecipes));
            PlayerPrefs.SetString("neededIng" + LoginGUI.instance.Username(), SaveInventory(neededIngridents));

			Debug.Log(myLikedRecipes.Count);

            SaveOldPantry();
        }
        else
        {
            // check to see if the old info has changed
            bool runSave = false;
            // if the length of the old items isnt the same run the save
            if (oldPantryItems.Count != myIngridents.Count)
            {
                runSave = true;
            }
            else
            {
                // run through and make sure nothing got changed
                for (int i = 0; i < myIngridents.Count; i++)
                {
                    // if something isnt the same, run the save
                    if (myIngridents[i] != oldPantryItems[i])
                    {
                        runSave = true;
                        break;
                    }
                }
            }

            if (runSave)
            {
                // save the information and upload it the database
                LoginGUI.instance.PantryListV = SaveInventory(myIngridents);
                LoginGUI.instance.NeededIng = SaveInventory(neededIngridents);
                LoginGUI.instance.LikeRecipes = SaveInt(myLikedRecipes);
                LoginGUI.instance.UploadPantry();
                SaveOldPantry();
            }
        }
    }
    public static void SaveNoCheck()
    {
        LoginGUI.instance.PantryListV = SaveInventory(myIngridents);
        LoginGUI.instance.LikeRecipes = SaveInt(myLikedRecipes);
        LoginGUI.instance.NeededIng = SaveInventory(neededIngridents);

        LoginGUI.instance.UploadPantry();
        SaveOldPantry();
    }

    /// <summary>
    /// Load all your Ing's into your pantry
    /// </summary>
    public static void Load()
    {
        if (LoginGUI.instance.Username() == "Guest")
        {
            if (PlayerPrefs.GetString("myPantryItems" + LoginGUI.instance.Username()) != null)
			{
                myIngridents = LoadInventory("myPantryItems" + LoginGUI.instance.Username(), true);
			}

			if(PlayerPrefs.GetString("myLikedRecipes" + LoginGUI.instance.Username()) != null)
			{
				myLikedRecipes = LoadInt(PlayerPrefs.GetString("myLikedRecipes" + LoginGUI.instance.Username()));
			}

            if (PlayerPrefs.GetString("neededIng" + LoginGUI.instance.Username()) != null)
            {
                neededIngridents = LoadInventory("neededIng" + LoginGUI.instance.Username(), true);
            }
        }
        else
        {
            string pantryList = LoginGUI.instance.PantryList();
			string likedRecipeString = LoginGUI.instance.LikeRecipes;
            string neededList = LoginGUI.instance.NeededIng;

			// pulls the information into list form if there is data
            if(pantryList != "")
                myIngridents = LoadInventory(pantryList, false);

			// pulls the liked string and then sperates the data out to put into list form
			if(likedRecipeString != "")
				myLikedRecipes = LoadInt(likedRecipeString);

            if (neededList != "")
                neededIngridents = LoadInventory(neededList, false);
        }

        SaveOldPantry();
    }

    /// <summary>
    /// Holds your pantry information to then be check with your new pantry information
    /// </summary>
    public static void SaveOldPantry()
    {
        oldPantryItems.Clear();

        for (int i = 0; i < myIngridents.Count; i++)
        {
            oldPantryItems.Add(myIngridents[i]);
        }
    }

    /// <summary>
    /// Syncs your local pnatry items with your online pantry list
    /// </summary>
    public static void AppendLocalListWithOnlineList()
    {
        List<Ing> localList = new List<Ing>();
        localList = LoadInventory("myPantryItemsGuest", true);

        for (int i = 0; i < localList.Count; i++)
        {
            AddToMyPantry(localList[i]);
        }
    }

    /// <summary>
    /// Syncs your online pantry items with your local pantry items
    /// </summary>
    public static void AppendOnlineListWithLocalList()
    {
        PlayerPrefs.SetString("myPantryItemsGuest", SaveInventory(myIngridents));
    }

    /// <summary>
    /// Saves your inventory
    /// </summary>
    /// <param name="myInventory">Converts your pantry items into string form</param>
    /// <returns></returns>
    public static string SaveInventory( List<Ing> myInventory )
    {
        string boolsToString = "";

        for (int i = 0; i < myInventory.Count; i++)
        {
            boolsToString += "|" + Convert.ToInt32(myInventory[i].id);
        }

        //Debug.Log("Saved: " + boolsToString);

        return boolsToString;

    }

    /// <summary>
    /// Loads your inventory from saved data
    /// </summary>
    /// <param name="entry">Converts the string you saved to into Ing</param>
    /// <param name="isLocal">Check if your loading a local or online pantry</param>
    /// <returns></returns>
    public static List<Ing> LoadInventory(string entry, bool isLocal)
    {

        string newEntry = "";

        if (isLocal)
        {
            newEntry = PlayerPrefs.GetString(entry);
        }
        else
        {
            newEntry = entry;
        }

        string[] seperatedEntries = newEntry.Split('|');
        List<Ing> arrToList = new List<Ing>();

        foreach (string s in seperatedEntries)
        {
            int tempNumber;
            if (int.TryParse(s, out tempNumber))
            {
                for (int i = 0; i < RecipeManager.allIngridents.Count; i++)
                {
                    if (RecipeManager.allIngridents[i].id == tempNumber)
                    {
                        //Debug.Log("Name: " + RecipeManager.allIngridents[i].name + ", ID: " + tempNumber);

                        Ing ig = new Ing(tempNumber, RecipeManager.allIngridents[i].name);
                        arrToList.Add(ig);
                        break;
                    }
                }

                //Debug.Log("Name: " + RecipeManager.allIngridents[tempNumber].name + ", ID: " + tempNumber);

                //Ing ig = new Ing(tempNumber, RecipeManager.unsortedAllIngridents[tempNumber].name);
                //arrToList.Add(ig);
            }
        }

        return arrToList;
    }

	public static string SaveInt(List<int> list)
	{
		string boolsToString = "";
		
		for (int i = 0; i < list.Count; i++)
		{
			boolsToString += "|" + list[i].ToString();
		}
		
		return boolsToString;
	}
	public static List<int> LoadInt(string entry)
	{
		string[] seperatedEntries = entry.Split('|');
		List<int> arrToList = new List<int>();
		//Debug.Log(seperatedEntries);
		foreach (string s in seperatedEntries)
		{
			if (s != "")
				arrToList.Add(int.Parse(s));
		}
		return arrToList;
	}

    public static string SaveStrings(List<string> list)
    {
        string boolsToString = "";

        for (int i = 0; i < list.Count; i++)
        {
            boolsToString += "|" + list[i];
        }

        return boolsToString;
    }
    public static List<string> LoadStrings(string entry)
    {
        string[] seperatedEntries = entry.Split('|');
        List<string> arrToList = new List<string>();
        //Debug.Log(seperatedEntries);
        foreach (string s in seperatedEntries)
        {
            if (s != "")
                arrToList.Add(s);
        }
        return arrToList;
    }
}
