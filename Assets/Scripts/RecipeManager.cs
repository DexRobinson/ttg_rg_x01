using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RecipeManager  {
    // keep a list of all the recipes that are in the database
    public static List<Recipe> allRecipes = new List<Recipe>();
    // keep a list of all the ingridents in the database
    public static List<Ing> allIngridents = new List<Ing>();

    private static int _id = 0;
    private static int _ingId = 0;

    /// <summary>
    /// Insert a recipe into allRecipes list
    /// </summary>
    /// <param name="title">Name of the Recipe</param>
    /// <param name="url">Link to the recipes website</param>
    /// <param name="ing">All the ing</param>
    /// <param name="author">Name of the person who made the recipe</param>
    /// <param name="desc">A quick desc of the recipe</param>
    /// <param name="ingAmount">How much Ing to add</param>
    /// <param name="preptime">How long does it take to prep the recipe</param>
    /// <param name="cooktime">How long does it take to cook the recipe</param>
    /// <param name="steps">Enter what to do</param>
    public static void InsertRecipe( string title, string url, List<Ing> ing, string author, string desc,
        List<string> ingAmount, int preptime, int cooktime, List<string> steps, string imageUrl, int _idSQL )
    {
        allRecipes.Add(new Recipe());
        allRecipes[_id].id = _id;
        allRecipes[_id].title = title;
        allRecipes[_id].url = url;
        allRecipes[_id].author = author;
        allRecipes[_id].description = desc;
        allRecipes[_id].cookTime = cooktime;
        allRecipes[_id].prepTime = preptime;
        
        for (int i = 0; i < ing.Count; i++)
        {
            allRecipes[_id].ings.Add(ing[i]);
        }
        for (int x = 0; x < ingAmount.Count; x++)
        {
            allRecipes[_id].ingredientsAmount.Add(ingAmount[x]);
        }
        for (int y = 0; y < steps.Count; y++)
        {
            allRecipes[_id].steps.Add(steps[y]);
        }

        allRecipes[_id].imageUrl = imageUrl;
        allRecipes[_id].idSQL = _idSQL;

        _id++;
    }

    /// <summary>
    /// Returns how much of the recipes Ing you have
    /// </summary>
    /// <param name="recipeIndex">Enter recipe index</param>
    /// <returns></returns>
    public static float ReturnPercentOfRecipe(int recipeIndex)
    {
        float have = 0;
        float total = allRecipes[recipeIndex].ings.Count;

        if (PantryManager.myIngridents.Count > 0)
        {
            for (int i = 0; i < PantryManager.myIngridents.Count; i++)
            {
                for (int x = 0; x < allRecipes[recipeIndex].ings.Count; x++ )
                {
                    if (allRecipes[recipeIndex].ings[x].id == PantryManager.myIngridents[i].id)
                    {
                        have++;
                        break;
                    }
                }
            }
        }

        if (have == 0)
        {
            allRecipes[recipeIndex].SetPercent(0);
            return 0;
        }
        else
        {
            allRecipes[recipeIndex].SetPercent((have / total) * 100);
            return (have / total) * 100;
        }
    }

    public static void InsertAuthor(string name)
    {
        
        allRecipes[_id].author = name;
    }
    public static void InsertDescription(string name)
    {
        allRecipes[_id].description = name;
    }
    public static void InsertWebsite(string name)
    {
        allRecipes[_id].url = name;
    }
    public static void InsertTitle(string name)
    {
        //allRecipes.Add(new Recipe());

        //allRecipes[_id].id = _id;

        allRecipes[_id].title = name;
    }
    public static void InsertId(string name)
    {
        allRecipes.Add(new Recipe());

        allRecipes[_id].id = _id;
    }
    public static void InsertPrepTime(string name)
    {
        allRecipes[_id].prepTime = int.Parse(name);
    }
    public static void InsertCookTime(string name)
    {
        allRecipes[_id].cookTime = int.Parse(name);
    }
    public static void InsertIngridents(string name)
    {
        allRecipes[_id].ings = LoadDBIngridents(name);
    }
    public static void InsertSteps(string name)
    {
        allRecipes[_id].steps = PantryManager.LoadStrings(name);
    }
    public static void InsertIngAmount(string name)
    {
        allRecipes[_id].ingredientsAmount = PantryManager.LoadStrings(name);
    }
    public static void InsertImageUrl( string name )
    {
        allRecipes[_id].imageUrl = name;
    }
    public static void InsertIdSQL( string name )
    {
        //Debug.Log(name);
        allRecipes.Add(new Recipe());

        allRecipes[_id].id = _id;
        
        if(name != "")
            allRecipes[_id].idSQL = int.Parse(name);
    }

    public static void IncreaseId()
    {
        _id++;
    }

    public static void InsertIngridentId(string id)
    {
        allIngridents.Add(new Ing());
        //Debug.Log("Add: " + id);
        allIngridents[_ingId].id = int.Parse(id);
    }
    public static void InsertIngridentName(string name)
    {
        char[] a = name.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        for (int i = 1; i < a.Length; i++)
        {
            if (a[i - 1] == ' ')
            {
                if (char.IsLower(a[i]))
                {
                    a[i] = char.ToUpper(a[i]);
                }
            }
            else
            {
                a[i] = char.ToLower(a[i]);
            }
        }

        allIngridents[_ingId].name = new string(a);
    }

    public static void IncreaseIngridentId()
    {
        _ingId++;
    }

    public static List<Ing> LoadDBIngridents( string entry )
    {
        string[] seperatedEntries = entry.Split('|');
        List<Ing> arrToList = new List<Ing>();

        foreach (string s in seperatedEntries)
        {
            int tempNumber;
            //int i = 0;

            if (int.TryParse(s, out tempNumber))
            {
                //Debug.Log(RecipeManager.allIngridents.Count + ", " + tempNumber);

                for (int i = 0; i < allIngridents.Count; i++)
                {
                    if (tempNumber == allIngridents[i].id)
                    {
                        arrToList.Add(new Ing(tempNumber, RecipeManager.allIngridents[i].name));
                        break;
                    }
                }

                //arrToList.Add(new Ing(tempNumber, RecipeManager.allIngridents[i].name));
            }
        }

        return arrToList;
    }
}

[System.Serializable]
public class Recipe
{
    public int id = 0;
    public List<string> ingredientsAmount = new List<string>();
    public List<Ing> ings = new List<Ing>();
    public string title;
    public string author;
    public string description;
    public int prepTime;
    public int cookTime;
    public List<string> steps = new List<string>();
    public float percent;
    public string url;
    public string imageUrl;
    public int idSQL;

    public void SetPercent(float setPercent)
    {
        percent = setPercent;
    }
}

[System.Serializable]
public class Ing
{
    public int id;
    public string name;

    public Ing()
    {
    }

    public Ing( int _id, string _name )
    {
        id = _id;
        name = _name;
    }
}