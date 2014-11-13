using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class RecipeMaker : MonoBehaviour {
    public static RecipeMaker instance;

    //private const string root = "http://localhost/RecipeGenie/Unity/core/";
    private const string root = "http://www.taptoongames.com/WebPlayer/Unity/core/";


    private string uploadRecipeURL = root + "upload_recipe.php";
    private string count = root + "return_recipe_count.php";
    private string names = root + "return_author_name.php";
    private string ingridents = root + "return_ing_name.php";
    private string editRecipeURL = root + "edit_recipe.php";
 
    private string author = "";
    private string website = "";
    private string prep = "";
    private string cook = "";
    private string desc = "";
    private string title = "";
    private string imageUrl = "";
    private List<Ing> ingridetns = new List<Ing>();
    private List<string> steps = new List<string>();
    private List<string> ingAmount = new List<string>();

    private bool dropDownOpen = false;
    private Vector2 windowScrollPosition = new Vector2();

    private Rect middleWindow = new Rect();
    private int recipeCount = 0;
    private int recipeLoaded = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Count();
        //middleWindow = new Rect(5, 5, Screen.width - 40, Screen.height - (0 + 80));
        middleWindow = GUIManager.instance.ReturnMiddleNonSortRect();
        middleWindow = new Rect(middleWindow.x - 5, middleWindow.y - 5, middleWindow.width + 15, middleWindow.height );

    }
    
    public void CreateRecipe()
    {
        GUIManager.instance.scrollPosition = GUILayout.BeginScrollView(GUIManager.instance.scrollPosition);
        
        //GUI.Label(new Rect(10, 35, 200, buttonH), "Author: ");
        //author = GUI.TextField(new Rect(215, 35, 200, buttonH), author, 50);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Title: ",  GUILayout.Width(120));
        title = GUILayout.TextField(title, 50);
        title = Regex.Replace(title, @"[^a-zA-Z0-9  .\-:;,]", "");
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Author: ",  GUILayout.Width(120));
        author = GUILayout.TextField(author, 30);
        author = Regex.Replace(author, @"[^a-zA-Z0-9  .\-:;]", "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Website: ", GUILayout.Width(120));
        website = GUILayout.TextField(website, 1024);
		website = Regex.Replace(website, @"[^a-zA-Z0-9 / .\-:;]", "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Image URL: ", GUILayout.Width(120));
        imageUrl = GUILayout.TextField(imageUrl, 1024);
		imageUrl = Regex.Replace(imageUrl, @"[^a-zA-Z0-9 / .\-:;]", "");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Prep Time:", GUILayout.Width(120));
        prep = GUILayout.TextField(prep, 20);
        prep = Regex.Replace(prep, @"[^0-9]", "");
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Cook Time:",  GUILayout.Width(120));
        cook = GUILayout.TextField(cook, 20);
        cook = Regex.Replace(cook, @"[^0-9]", "");
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Description: ", GUILayout.Width(120));
        desc = GUILayout.TextField(desc, 1024);
        desc = Regex.Replace(desc, @"[^a-zA-Z0-9  .\-:;,]", "");
        GUILayout.EndHorizontal();

		GUILayout.Label("Ingredients: ");

        if (ingridetns.Count > 0)
        {
            for (int i = 0; i < ingridetns.Count; i++)
            {
                GUILayout.BeginHorizontal();
                ingAmount[i] = GUILayout.TextField(ingAmount[i], 12, GUILayout.Width(118));
                ingAmount[i] = Regex.Replace(ingAmount[i], @"[^a-zA-Z0-9 /]", "");
                //GUILayout.Label("- " + RecipeManager.ReturnNameOfIngrident(ingridetns[i]));
                GUILayout.Label("- " + ingridetns[i].name);

                //GUI.skin = GUIManager.instance.gotIngSkin;

                if (GUILayout.Button(GUIManager.instance.xCheck, GUILayout.Width(60)))
                {
                    ingAmount.RemoveAt(i);
                    ingridetns.RemoveAt(i);
                }

                //GUI.skin = GUIManager.instance.recipeSkin;
                GUILayout.EndHorizontal();
            }
        }
		if (GUILayout.Button("Add Ingredient"))
        {
            GUIManager.instance.currentMiddleIndex = 0;
            
            if (!dropDownOpen)
            {
                dropDownOpen = true;
            }
        }

        GUILayout.Label("Steps: ");

        

        if (steps.Count > 0)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Step " + (i + 1).ToString() + ": ");
                steps[i] = GUILayout.TextField(steps[i], GUILayout.MaxWidth(middleWindow.width / 2));
                steps[i] = Regex.Replace(steps[i], @"[^a-zA-Z0-9  .\-:;,]", "");
                if (GUILayout.Button(GUIManager.instance.xCheck))
                {
                    steps.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Add Step"))
        {
            steps.Add("");
			GUIManager.instance.scrollPosition.y = 100000;
        }


        //if (dropDownOpen)
        //{
        //    CreateDropDown();
        //}

        GUILayout.Box("");
        if (GUIManager.instance.isEditing)
        {
            if (GUILayout.Button("Save Edit"))
            {
                Save();
            }
        }
        else
        {
            if (GUILayout.Button("Save"))
            {
                Save();
            }
        }
        GUILayout.EndScrollView();
    }

    void Save()
    {
        Upload();
    }

    public void Upload()
    {
        StartCoroutine(_Upload());
    }

    IEnumerator _Upload()
    {
        bool ingNameBlank = false;

        for (int i = 0; i < ingAmount.Count; i++)
        {
            if (ingAmount[i] == "")
            {
                ingNameBlank = true;
            }
        }

        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i] == "")
            {
                ingNameBlank = true;
            }
        }

		if (
			title == "" ||
			author == "" ||
			desc == "" ||
			cook == "" ||
			prep == "" ||
			ingAmount.Count () == 0 ||
			ingridetns.Count () == 0 || 
			steps.Count () == 0 ||
            ingNameBlank
		) 
		{
				
		} 
		else 
		{
			WWWForm wwwF = new WWWForm ();

            if(GUIManager.instance.isEditing)
                wwwF.AddField("id", RecipeManager.allRecipes[GUIManager.instance.recipeSelected].idSQL);

			wwwF.AddField ("title", title);
			wwwF.AddField ("author", author);
			wwwF.AddField ("website", website);
			wwwF.AddField ("desc", desc);
			wwwF.AddField ("cook", cook);
			wwwF.AddField ("prep", prep);
			wwwF.AddField ("ing", PantryManager.SaveInventory (ingridetns));
			wwwF.AddField ("ing_amount", PantryManager.SaveStrings (ingAmount));
			wwwF.AddField ("steps", PantryManager.SaveStrings (steps));
			wwwF.AddField ("username", LoginGUI.instance.Username ());
			wwwF.AddField ("imageUrl", imageUrl);

            string wwwURL = "";

            if (GUIManager.instance.isEditing)
            {
                wwwURL = editRecipeURL;
            }
            else
            {
                wwwURL = uploadRecipeURL;
            }

			WWW www = new WWW(wwwURL, wwwF);

			while (!www.isDone) {
					yield return new WaitForEndOfFrame ();
			}

			if (www.error != null)
            {
				Debug.Log ("Error: " + www.error);
			} 
            else 
            {
				Debug.Log(www.text);
				if (www.text == "Success") 
                {
                    this.title = "";
                    this.author = "";
                    this.website = "";
                    this.desc = "";
                    this.cook = "";
                    this.prep = "";
                    this.imageUrl = "";
                    this.ingAmount.Clear();
                    this.ingridetns.Clear();
                    this.steps.Clear();
                    GUIManager.instance.isEditing = false;
                        
				}
			}
		}
    }

    public void Count()
    {
        StartCoroutine(_Count());
    }
    IEnumerator _Count()
    {
        WWW www = new WWW(count);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            recipeCount = int.Parse(www.text);
            //Debug.Log(www.text);
        }
    }

    public int RecipeCount()
    {
        return recipeCount;
    }
    public int RecipeCurrentCount()
    {
        return recipeLoaded;
    }

    public void GetAuthor()
    {
        StartCoroutine(_GetAuthor());
    }

    private IEnumerator _GetAuthor()
    {
        WWW wwwIng = new WWW(ingridents);

        while (!wwwIng.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (wwwIng.error != null)
        {
            Debug.Log("Error: " + wwwIng.error);
        }
        else
        {
            LoadIngridents(wwwIng.text);
        }

		//GestureHandler.InitMaxDistance();

        WWW www = new WWW(names);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            LoadRecipes(www.text);
        }

        yield return new WaitForEndOfFrame();
        LoginGUI.instance.AllRecipesLoad = true;
        //Debug.Log("Loaded All Recipes/Ingridents");
    }

    
    public void LoadRecipes(string entry)
    {
        string newEntry = entry;
        string[] seperatedEntries = newEntry.Split('*');
        //List<string> arrToList = new List<string>();

        //foreach (string s in seperatedEntries)
        //{
        //    Debug.Log(s);
        //}
        for (int i = 0; i < seperatedEntries.Length - 1; i++)
        {
            string s = seperatedEntries[i];
            //Debug.Log(i.ToString() + ": " + s);

            switch (i % 11)
            {
                case 0:
                    //Debug.Log("ID: " + s);
                    RecipeManager.InsertIdSQL(s);
                    break;
                case 1:
                    //Debug.Log("Author: " + s);
                    RecipeManager.InsertTitle(s);
                    break;
                case 2:
                    //Debug.Log("Author: " + s);
                    RecipeManager.InsertAuthor(s);
                    break;
                case 3:
                    //Debug.Log("Website: " + s);
                    RecipeManager.InsertWebsite(s);
                    break;
                case 4:
                    //Debug.Log("Prep: " + s);
                    RecipeManager.InsertPrepTime(s);
                    break;
                case 5:
                    //Debug.Log("Cook: " + s);
                    RecipeManager.InsertCookTime(s);
                    break;
                case 6:
                    //Debug.Log("Desc: " + s);
                    RecipeManager.InsertDescription(s);
                    break;
                case 7:
                    //Debug.Log("ing: " + s);
                    RecipeManager.InsertIngridents(s);
                    break;
                case 8:
                    //Debug.Log("ing amt: " + s);
                    RecipeManager.InsertIngAmount(s);
                    break;
                case 9:
                    RecipeManager.InsertSteps(s);
                    break;  
                case 10:
                    RecipeManager.InsertImageUrl(s);
                    //Debug.Log("Steps: " + s);

                    recipeLoaded++;
                    RecipeManager.IncreaseId();
                    break;
            }
        }
    }

    public void LoadIngridents( string entry )
    {
        string newEntry = entry;
        string[] seperatedEntries = newEntry.Split('*');
        //Debug.Log(entry);

        for (int i = 0; i < seperatedEntries.Length; i++)
        {
            if (i != seperatedEntries.Length - 1)
            {
                string s = seperatedEntries[i];
                //Debug.Log(i.ToString() + ": " + s);

                switch (i % 3)
                {
                    case 0:
                        //Debug.Log("Ingrident ID: " + s);
                        RecipeManager.InsertIngridentId(s);
                        break;
                    case 1:
                        //Debug.Log("Ingrident Name: " + s);
                        RecipeManager.InsertIngridentName(s);

                        RecipeManager.IncreaseIngridentId();
                        break;
                    case 2:
                        break;
                }
            }
        }
    }

    public void AddToCreateRecipe(int id)
    {
        //Debug.Log("Added: " + id);

        ingridetns.Add(RecipeManager.allIngridents[id]);
        ingAmount.Add("amt");
		GUIManager.instance.scrollPosition.y = 100000;
        dropDownOpen = false;
        GUIManager.instance.currentMiddleIndex = 8;
    }

    public bool DropDown()
    {
        return dropDownOpen;
    }

    public void Clear()
    {
        this.title = "";
        this.author = "";
        this.website = "";
        this.desc = "";
        this.cook = "";
        this.prep = "";
        this.imageUrl = "";
        this.ingAmount.Clear();
        this.ingridetns.Clear();
        this.steps.Clear();
        GUIManager.instance.isEditing = false;
    }

    public void InsertRecipeForEdit(string _author, string _website, string _prep, string _cook, string _desc, string _title, string _imageURL, List<Ing> _ings, List<string> _steps, List<string> _amt)
    {
        author = _author;
        website = _website;
        prep = _prep;
        cook = _cook;
        desc = _desc;
        title = _title;
        imageUrl = _imageURL;
        this.ingridetns = _ings;
        this.steps = _steps;
        this.ingAmount = _amt;
    }
}
