using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Prime31;
using UnityEngine.Advertisements;

public class GUIManager : MonoBehaviour 
{
    public static GUIManager instance;
	public AdManager adManager;

    public float versionNumber = 0.1f;

    private Rect topRowRect = new Rect();
    private Rect middleWindow = new Rect();

    public Vector2 scrollPosition = new Vector2();
    public int currentMiddleIndex = 0;
    public int recipeSelected = 0;
    private List<Recipe> allSortedList = new List<Recipe>();

    public GUISkin pantrySkin;
    public GUISkin recipeSkin;
    public GUISkin nonIngSkin;
    public GUISkin gotIngSkin;
    public GUISkin settingsSkin;
    public GUISkin recipePickerSkin;
    public GUISkin addIngSkin;
    public GUISkin bottomRowSkin;
    public GUISkin likedRecipeSkin;
    public GUISkin recipeCreateSkin;
    public GUISkin recipePickerTopSkin;
    public GUISkin storeSkin;
    public GUISkin tutorialSkin;

    public string[] skinNames;
    //public GUISkin[] pantrySkinList;
    //public GUISkin[] recipeSkinList;
    //public GUISkin[] nonIngSkinList;
    //public GUISkin[] gotIngSkinList;
    //public GUISkin[] settingsSkinList;
    //public GUISkin[] recipePickerSkinList;
    //public GUISkin[] addIngSkinList;

    public GUIStyle style;

    public int currentGUISkinIndex = 0;

    private string searchText = "enter search here";
    private bool initGUIStart;
    private bool nameSortAssend;
    private bool percentSortAssend;
    private float updateTimer = 5;
    private string feedbackText = "";
    private string ingridentText = "";
    private bool isWaiting;

    private Texture selectedRecipeImage;
    private Vector2 oldRecipePositionIndex;

	public float targetScrollPositionY;
	public float swipeVelocity;
	public float maxScrollPositionY;

    public bool isEditing;

    private bool isFirstTime = true;
    private int tutorialIndex;

    private float updateTimerForSwipe;
    private Vector2 previousSwipePosition;

    private bool recipeQuickViewOpen;
    public List<string> myUploadedRecipesTitles = new List<string>();

    public float buttonWidthScaleAmount;
    float settingsButtonWidth = 0;

    public Texture[] bottomRowIcons;
    public Texture[] settingIcons;
    public Texture checkMark;
    public Texture xMark;
    public Texture xCheck;

    public Texture icon;

    private string titleOfPage = "Ingridents";

    public int touchCount = 0;
    public int touchRate = 25;

    public Font normalFont;
    public Font retinaFont;

#if UNITY_IPHONE
	// In app purchases product list
	private List<StoreKitProduct> _products;
    string productId = "19764";
#endif

#if UNITY_ANDROID
    string productId = "19763";
#endif

#if UNITY_EDITOR
    //string productId = "19763";
#endif

    private Rect middleWindowNonSort = new Rect();

    public void IncreaseTouchCount()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        //string productId = "19763";
        touchCount++;
#endif
        if (touchCount > touchRate)
        {
            Debug.Log("Ready to play ad");
            if (Advertisement.isReady())
            {
                touchCount = 0;
                Advertisement.Show();
            }
        }
    }
	public Rect ReturnMiddleRect()
	{
		return middleWindow;
	}
    public Rect ReturnMiddleNonSortRect()
    {
        return middleWindowNonSort;
    }

    void Awake()
    {
        instance = this;

        if (Advertisement.isSupported)
        {
            Advertisement.allowPrecache = true;
            Advertisement.Initialize(productId);
        }
    }

    void Start()
    {
        Debug.Log(Screen.dpi);
        //PlayerPrefs.DeleteAll();
        int firstTime = PlayerPrefs.GetInt("isFirstTime");
        if (firstTime == 1)
            isFirstTime = false;
        //isFirstTime = true;

        currentGUISkinIndex = 0;// PlayerPrefs.GetInt("currentSkin");
        ChangeGUIIndex(currentGUISkinIndex);

        RecipeMaker.instance.GetAuthor();
		if (adManager.IsAdsRunning()) 
		{
            //middleWindow = new Rect(5, 95 + 30, Screen.width - 40, Screen.height - (0 + 80 + 85) - 30);
            //middleWindowNonSort = new Rect(-2, 95 + 25, Screen.width, Screen.height - (0 + 80 + 75) - 30);
            //topRowRect = new Rect(Screen.width - 34, 85 + 30, 33, Screen.height - 85 - 30);

            middleWindowNonSort = new Rect(-2, 25, Screen.width + 5, Screen.height - (0 + 84) - 10);
            middleWindow = new Rect(-2, 25, Screen.width - 30, Screen.height - (0 + 76) - 18);

            topRowRect = new Rect(Screen.width - 34, 0 + 48, 33, Screen.height - 122);
		} 
		else 
		{
            middleWindowNonSort = new Rect(-2, 25, Screen.width + 5 , Screen.height - (0 + 84) - 10);
            middleWindow = new Rect(-2, 25, Screen.width - 30, Screen.height - (0 + 76) - 18);

            topRowRect = new Rect(Screen.width - 34, 0 + 48, 33, Screen.height - 122);
		}

        settingsButtonWidth = Screen.width * buttonWidthScaleAmount;
        //middleWindow = new Rect(5, 65, Screen.width - 40, Screen.height - (0 + 80 + 55));
        #if UNITY_IPHONE
		// array of product ID's from iTunesConnect. MUST match exactly what you have there!
		var productIdentifiers = new string[] { "RemoveAds" };
		StoreKitBinding.requestProductData( productIdentifiers );

		// you cannot make any purchases until you have retrieved the products from the server with the requestProductData method
		// we will store the products locally so that we will know what is purchaseable and when we can purchase the products
		StoreKitManager.productListReceivedEvent += allProducts =>
		{
			Debug.Log( "received total products: " + allProducts.Count );
			_products = allProducts;
		};
        #endif
    }

    void OnGUI()
    {
        if (LoginGUI.instance.IsLoggedIn())
        {
            if(currentMiddleIndex == 0)
                DrawTopRect();
            
            if(isFirstTime)
                DrawTutorial();

            SwitchMiddleArea(currentMiddleIndex);


            InitGUIFocusOnStart();

            GUI.skin = bottomRowSkin;
            GUI.Label(new Rect(0, 0, Screen.width, 50), titleOfPage);

            
            DrawBottomButtons();
        }
    }

    void Update()
    {
#if UNITY_WEBPLAYER || UNITY_EDITOR
        //if (adManager.IsAdsRunning())
        //{
        //    middleWindow = new Rect(5, 95, Screen.width - 40, Screen.height - (0 + 80 + 85));
        //    middleWindowNonSort = new Rect(5, 95, Screen.width, Screen.height - (0 + 80 + 85));
        //    topRowRect = new Rect(Screen.width - 34, 85, 33, Screen.height - 85);
        //}
        //else
        //{
        //    middleWindowNonSort = new Rect(5, 85, Screen.width, Screen.height - (0 + 80));
        //    middleWindow = new Rect(5, 1, Screen.width - 40, Screen.height - (0 + 76));
        //    topRowRect = new Rect(Screen.width - 34, 0, 33, Screen.height);
        //}
#endif
        if (Time.time > updateTimer && LoginGUI.instance.IsLoggedIn())
        {
            updateTimer = Time.time + 10;
            PantryManager.Save();
        }

        if (Time.time > updateTimerForSwipe)
        {
            previousSwipePosition = scrollPosition;
            updateTimerForSwipe = Time.time + 0.5f;
        }

		scrollPosition = Vector2.Lerp (scrollPosition, new Vector2 (0, targetScrollPositionY), swipeVelocity * Time.deltaTime);
    }

    // checks to make sure that the user has pressed the button and they arent trying to scroll instead
    bool ReturnButtonPress()
    {
        bool isPressed = false;

        if (previousSwipePosition == scrollPosition)
        {
            if (adManager.IsAdsRunning())
                IncreaseTouchCount();

            isPressed = true;
        }

        return isPressed;
    }

    // switch the current theme
    void ChangeGUIIndex( int newIndex )
    {
        //pantrySkin = Resources.Load("PantrySkin_" + newIndex) as GUISkin;
        //recipeSkin = Resources.Load("RecipeSelectionSkin_" + newIndex) as GUISkin;
        //nonIngSkin = Resources.Load("NonIngSkin_" + newIndex) as GUISkin;
        //gotIngSkin = Resources.Load("GotIngSkin_" + newIndex) as GUISkin;
        //settingsSkin = Resources.Load("SettingSkin_" + newIndex) as GUISkin;
        //recipePickerSkin = Resources.Load("RecipePickerSkin_" + newIndex) as GUISkin;
        //addIngSkin = Resources.Load("AddIngSkin_" + newIndex) as GUISkin;

        PlayerPrefs.SetInt("currentSkin", newIndex);

        if (Screen.height > 1024)
        {
            Debug.Log("Retina Display");

            List<GUISkin> allSkins = new List<GUISkin>();
            allSkins.Add(pantrySkin);
            allSkins.Add(recipeSkin);
            allSkins.Add(nonIngSkin);
            allSkins.Add(gotIngSkin);
            allSkins.Add(settingsSkin);
            allSkins.Add(recipePickerSkin);
            allSkins.Add(addIngSkin);
            allSkins.Add(bottomRowSkin);
            allSkins.Add(recipeCreateSkin);
            allSkins.Add(likedRecipeSkin);
            allSkins.Add(recipePickerTopSkin);
            allSkins.Add(storeSkin);
            allSkins.Add(recipeSortingSkin);

            allSkins.Add(tutorialSkin);
            allSkins.Add(LoginGUI.instance.loginSkin);
            allSkins.Add(LoginGUI.instance.signupSkin);

            for (int i = 0; i < allSkins.Count; i++)
            {
                allSkins[i].box.font = retinaFont;
                allSkins[i].button.font = retinaFont;
                allSkins[i].toggle.font = retinaFont;
                allSkins[i].label.font = retinaFont;
                allSkins[i].textField.font = retinaFont;
                allSkins[i].textArea.font = retinaFont;
            }

            leftAlign.font = retinaFont;
            rightAlign.font = retinaFont;
            centerAlign.font = retinaFont;

            leftAlign.fontSize = 36;
            rightAlign.fontSize = 36;
            centerAlign.fontSize = 36;
        }
        else
        {
            Debug.Log("Normal Display");
            List<GUISkin> allSkins = new List<GUISkin>();
            allSkins.Add(pantrySkin);
            allSkins.Add(recipeSkin);
            allSkins.Add(nonIngSkin);
            allSkins.Add(gotIngSkin);
            allSkins.Add(settingsSkin);
            allSkins.Add(recipePickerSkin);
            allSkins.Add(recipeSortingSkin);
            allSkins.Add(addIngSkin);
            allSkins.Add(bottomRowSkin);
            allSkins.Add(recipeCreateSkin);
            allSkins.Add(likedRecipeSkin);
            allSkins.Add(recipePickerTopSkin);
            allSkins.Add(storeSkin);
            allSkins.Add(tutorialSkin);
            allSkins.Add(LoginGUI.instance.loginSkin);
            allSkins.Add(LoginGUI.instance.signupSkin);

            for (int i = 0; i < allSkins.Count; i++)
            {
                allSkins[i].box.font = normalFont;
                allSkins[i].button.font = normalFont;
                allSkins[i].toggle.font = normalFont;
                allSkins[i].label.font = normalFont;
                allSkins[i].textField.font = normalFont;
                allSkins[i].textArea.font = normalFont;
            }

            leftAlign.font = normalFont;
            rightAlign.font = normalFont;
            centerAlign.font = normalFont;

            centerAlign.fontSize = 20;
            leftAlign.fontSize = 20;
            rightAlign.fontSize = 20;
        }

        //pantrySkin = pantrySkinList[newIndex];
        //recipeSkin = recipeSkinList[newIndex];
        //nonIngSkin = nonIngSkinList[newIndex];
        //gotIngSkin = gotIngSkinList[newIndex];
        //settingsSkin = settingsSkinList[newIndex];
        //recipePickerSkin = recipePickerSkinList[newIndex];
        //addIngSkin = addIngSkinList[newIndex];
    }

    // in app pages
	void BuyInAppPurchase(int productId)
	{
        #if UNITY_IPHONE
		// enforce the fact that we can't purchase products until we retrieve the product data
		if( _products != null && _products.Count > 0 )
		{
			var product = _products[productId];
			
			Debug.Log( "preparing to purchase product: " + product.productIdentifier );
			StoreKitBinding.purchaseProduct( product.productIdentifier, 1 );
		}
        #endif
	}
	void OpenStore()
	{
        #if UNITY_IPHONE
		StoreKitBinding.displayStoreWithProductId( "RG_0001" );
        #endif
	}

    // tutorial pages
    void DrawTutorial()
    {
        GUI.skin = tutorialSkin;
        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y + 15, middleWindowNonSort.width + 10, middleWindowNonSort.height - 15);
        //if (tutorialIndex != 3 && tutorialIndex != 5 && tutorialIndex != 8)
        //{
        //    Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y - 3, middleWindowNonSort.width + 10, middleWindowNonSort.height - 3);
        //    customRect = GUI.Window(0, customRect, TutorialWindow, "");
        //}
        if (tutorialIndex != 3 && tutorialIndex != 5 && tutorialIndex != 8)
            customRect = GUI.Window(1000, customRect, TutorialWindow, "");
    }
    void TutorialWindow(int windowId)
    {
        titleOfPage = "Tutorial";
        switch(tutorialIndex)
        {
            case 0:
                GUILayout.Label("Hello and welcome to Recipe Genie!");
                GUILayout.Label("Recipe Genie will help you create a recipe tonight with what you have in your pantry right now.");
                GUILayout.Label("Let's go over how to use and setup Recipe Genie.");
                GUILayout.Label("Press the \"Ok\" button to continue.");

                if (GUILayout.Button("Ok"))
                {
                    tutorialIndex++;
                }

            break;
            case 1:
                GUILayout.Label("The first thing you will need to do is go to the Ingredients tab. It is located on the bottom of the screen.");
            break;
            case 2:
                GUILayout.Label("This screen will allow you to add ingredients that you have in your house.");
                GUILayout.Label("Click on the open boxes to add ingredients to your pantry.");
                GUILayout.Label("You can remove items by clicking the same box you used to add the ingredient.");
                GUILayout.Label("Once you are done adding items, go to the Recipe tab on the bottom of the screen.");
                GUILayout.Label("Press \"Ok\" to start adding items to your pantry.");
                if (GUILayout.Button("Ok"))
                {
                    tutorialIndex++;
                }
            break;
            case 4:
                GUILayout.Label("This Screen will allow you to view all the recipes you can make with what you have in your pantry.");
                GUILayout.Label("You can sort the recipes by percent.");
                GUILayout.Label("Press \"Ok\" to view the recipes, then click on a recipe that you want to view.");
                if (GUILayout.Button("Ok"))
                {
                    tutorialIndex++;
                }
            break;
            case 6:
                GUILayout.Label("You have selected a recipe to view.");
                GUILayout.Label("Here you have a few options for the recipes. You can see all the ingridents needed to make the recipe " +
                    ", the steps on how to make it and a link to the website of the original recipe. If you like a recipe, click the \"Like\" button on " +
                    " the top of the page. You can view all your liked recipes in the \"Settings\" page.");
                GUILayout.Label("If you want to make a recipe you can click the \"Cook\" button and it will add all the ingredients you " +
                    "are missing to a grocery list which you can also view in the \"Settings\" page.");
                GUILayout.Label("Press \"Ok\" to continue");
                if (GUILayout.Button("Ok"))
                {
                    tutorialIndex++;
                }
            break;
            case 7:
                GUILayout.Label("Press \"Ok\" to continue, then click on the \"Settings\" tab once you are done");
                if (GUILayout.Button("Ok"))
                {
                    tutorialIndex++;
                }
            break;
            case 9:
            GUILayout.Label("The settings page has a lot of options.");
            GUILayout.Label("Here you can view your grocery list, liked recipes and you can even create your own recipes and ingredients.");
            GUILayout.Label("If you would like to remove the ad's you can pay a small price to remove them. You will also unlock different themes as well.");
            GUILayout.Label("We hope you enjoy Recipe Genie!");

            if (GUILayout.Button("Now let's get Cookin!"))
            {
                PlayerPrefs.SetInt("isFirstTime", 1);
                tutorialIndex++;
                isFirstTime = false;
            }
            break;
        }
    }


    // all pantry pages
    void DrawPantry()
    {
        GUI.skin = pantrySkin;
        middleWindow = GUI.Window(0, middleWindow, DrawPantryWindow, "");

    }
    void DrawPantryWindow( int windowid )
    {
        scrollPosition = GUI.BeginScrollView(new Rect(0, 25, middleWindow.width - 5, middleWindow.height - 30), scrollPosition, new Rect(0, 0, 0, RecipeManager.allIngridents.Count * 79));

        for (int i = 0; i < RecipeManager.allIngridents.Count; i++)
        {
            if (RecipeManager.allIngridents[i].name.Length > 1)
            {
                if (ReturnIfHaveIngridentName(RecipeManager.allIngridents[i]))
                {
                    GUI.skin = gotIngSkin;
                    GUI.Label(new Rect(5, 79 * i, middleWindow.width - 117, 75), RecipeManager.allIngridents[i].name);

                    if (GUI.Button(new Rect(middleWindow.width - 109, 79 * i, 75, 75), checkMark))
                    {
                        if (!RecipeMaker.instance.DropDown())
                        {
                            initRecipeList = false;
                            sortRecipes = false;
                            PantryManager.RemoveFromMyPantry(RecipeManager.allIngridents[i]);
                        }
                        else
                        {
                            initRecipeList = false;
                            sortRecipes = false;
                            RecipeMaker.instance.AddToCreateRecipe(i);
                        }
                    }
                }
                else
                {
                    GUI.skin = nonIngSkin;
                    GUI.Label(new Rect(5, 79 * i, middleWindow.width - 117, 75), RecipeManager.allIngridents[i].name);

                    if (GUI.Button(new Rect(middleWindow.width - 109, 79 * i, 75, 75), xMark))
                    {
                        if (!RecipeMaker.instance.DropDown())
                        {
                            initRecipeList = false;
                            sortRecipes = false;
                            PantryManager.AddToMyPantry(RecipeManager.allIngridents[i]);
                        }
                        else
                        {
                            initRecipeList = false;
                            sortRecipes = false;
                            RecipeMaker.instance.AddToCreateRecipe(i);
                        }
                    }
                }

                GUI.skin = pantrySkin;
            }
            else
            {
                GUI.Box(new Rect(5, 79 * i, middleWindow.width - 38, 75), RecipeManager.allIngridents[i].name);
            }
        }

        GUI.EndScrollView();
    }

    // my pantry pages
    void DrawMyPantry()
    {
        GUI.skin = pantrySkin;
        middleWindowNonSort = GUI.Window(0, middleWindowNonSort, DrawMyPantryWindow, "");

    }
    void DrawMyPantryWindow( int windowid )
    {
        scrollPosition = GUI.BeginScrollView(new Rect(0, 25, middleWindowNonSort.width - 5, middleWindowNonSort.height - 30), scrollPosition, new Rect(0, 0, 0, PantryManager.myIngridents.Count * 79));

        for (int i = 0; i < PantryManager.myIngridents.Count; i++)
        {
            GUI.skin = gotIngSkin;
            GUI.Label(new Rect(5, 79 * i, middleWindowNonSort.width - 90, 75), PantryManager.myIngridents[i].name);
            if (GUI.Button(new Rect(middleWindowNonSort.width - 109, 79 * i, 75, 75), xCheck))
            {
                initRecipeList = false;
                sortRecipes = false;
                PantryManager.RemoveFromMyPantry(i);
            }
        }

        GUI.EndScrollView();
    }

    // my uploaded recipes
    void DrawMyUploadedRecipe()
    {
        GUI.skin = recipePickerSkin;

        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y - 3, middleWindowNonSort.width + 10, middleWindowNonSort.height - 3);
        customRect = GUI.Window(0, customRect, DrawMyUploadedRecipeWindow, "");
    }
    void DrawMyUploadedRecipeWindow( int windowid )
    {
        //scrollPosition = GUI.BeginScrollView(new Rect(0, 70, middleWindow.width - 5, middleWindow.height - 75), scrollPosition, new Rect(0, 0, 0, PantryManager.myUploadedRecipes.Count * 45));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < myUploadedRecipesTitles.Count; i++)
        {
            //if (GUI.Button(new Rect(5, 45 * i, middleWindow.width - 40, 44), myUploadedRecipesTitles[i]))
            if (GUILayout.Button(myUploadedRecipesTitles[i]))
            {
                if (ReturnButtonPress())
                {
                    //oldRecipePositionIndex = scrollPosition;
                    scrollPosition = Vector3.zero;
                    GetRecipeImage(RecipeManager.allRecipes[ReturnIdFromSQLid(i)].imageUrl);
                    recipeSelected = ReturnIdFromSQLid(i);
                    currentMiddleIndex = 5;
                }
            }
        }

        GUILayout.EndScrollView();
    }

    // get my uploaded recipes
    public void InsertMyUploadedRecipes()
    {
        StartCoroutine(_InsertMyUploadedRecipes());
    }
    IEnumerator _InsertMyUploadedRecipes()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < PantryManager.myUploadedRecipes.Count; i++)
        {
            int index = 0;

            while (PantryManager.myUploadedRecipes[i] != RecipeManager.allRecipes[index].idSQL)
            {
                index++;
            }
            

            myUploadedRecipesTitles.Add(RecipeManager.allRecipes[index].title); 
        }

		LoginGUI.instance.LoggedIn(true);
        //Debug.Log(myUploadedRecipesTitles.Count());
    }

    // get sql id
    public int ReturnIdFromSQLid(int id)
    {
        int index = 0;
        Debug.Log("Need: " + PantryManager.myUploadedRecipes[id]);

        while (PantryManager.myUploadedRecipes[id] != RecipeManager.allRecipes[index].idSQL)
        {
            index++;
        }

        Debug.Log(RecipeManager.allRecipes[index].id);

        return RecipeManager.allRecipes[index].id;
    }

    // liked recipe pages
    void DrawLikedRecipe()
    {
        GUI.skin = likedRecipeSkin;

        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y + 5, middleWindowNonSort.width + 15, middleWindowNonSort.height);
        customRect = GUI.Window(0, customRect, DrawLikedRecipeWindow, "");
    }
    void DrawLikedRecipeWindow( int windowid )
    {
        //scrollPosition = GUI.BeginScrollView(new Rect(0, 70, middleWindow.width - 5, middleWindow.height - 75), scrollPosition, new Rect(0, 0, 0, PantryManager.myLikedRecipes.Count * 45));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < PantryManager.myLikedRecipes.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(RecipeManager.ReturnPercentOfRecipe(RecipeManager.allRecipes[PantryManager.myLikedRecipes[i]].id).ToString("f0") + "%", GUILayout.Width(60));

            //if (GUI.Button(new Rect(5, 45 * i, middleWindow.width - 40, 44), RecipeManager.allRecipes[PantryManager.myLikedRecipes[i]].title + "  (" + RecipeManager.ReturnPercentOfRecipe(RecipeManager.allRecipes[PantryManager.myLikedRecipes[i]].id).ToString("f0") + "%)"))
            if (GUILayout.Button(RecipeManager.allRecipes[PantryManager.myLikedRecipes[i]].title))
            {
                if (ReturnButtonPress())
                {
                    oldRecipePositionIndex = scrollPosition;
                    scrollPosition = Vector3.zero;
                    GetRecipeImage(RecipeManager.allRecipes[PantryManager.myLikedRecipes[i]].imageUrl);
                    recipeSelected = RecipeManager.allRecipes[PantryManager.myLikedRecipes[i]].id;
                    currentMiddleIndex = 5;
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    public GUISkin recipeSortingSkin;
    private int currentRecipeMin = 0;
    private int currentRecipeMax = 10;
    public GUIStyle leftAlign;
    public GUIStyle rightAlign;
    public GUIStyle centerAlign;


    // recipe pages
    void DrawRecipe()
    {
        GUI.skin = recipePickerSkin;
        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y, middleWindowNonSort.width + 10, middleWindowNonSort.height - 6);

        customRect = GUI.Window(0, customRect, DrawRecipeWindow, "");
    }
    void DrawRecipeWindow(int windowid)
    {
        //GUI.Label(new Rect(10, 22, Screen.width * 0.2f, 44), "Sort by: ");
        //if (GUI.Button(new Rect(5 + Screen.width * 0.22f, 22, Screen.width * 0.3f, 44), "Name"))
        //{
        //    SortByName();
        //}
        //if (GUI.Button(new Rect(15 + Screen.width * 0.52f, 22, Screen.width * 0.3f, 44), "Percent"))
        //{
        //    SortByPercent();
        //}
        GUI.skin = recipeSortingSkin;
        GUILayout.BeginHorizontal();
        //GUILayout.Label("Sort by: ", GUILayout.Height(40));
        ////if (GUILayout.Button("Name", GUILayout.Height(40)))
        ////{
        ////    SortByName();
        ////}
        //if (GUILayout.Button("Percent", GUILayout.Height(40)))
        //{
        //    SortByPercent();
        //}

        if (GUILayout.Button("< Previous 10", leftAlign, GUILayout.Height(70)))
        {
            if (currentRecipeMin > 0)
            {
                currentRecipeMin -= 10;
                currentRecipeMax -= 10;

                if (currentRecipeMin < 0)
                    currentRecipeMin = 0;
                if (currentRecipeMax < 10)
                    currentRecipeMax = 10;
            }
        }
        if (GUILayout.Button("Next 10 >", rightAlign, GUILayout.Height(70)))
        {
            if (currentRecipeMax <= allSortedList.Count)
            {
                currentRecipeMin += 10;
                currentRecipeMax += 10;

                if(currentRecipeMax > allSortedList.Count)
                    currentRecipeMax = allSortedList.Count;
                if (currentRecipeMin > allSortedList.Count - 10)
                    currentRecipeMin = allSortedList.Count - 10;
            }
        }
        GUILayout.EndHorizontal();

        GUI.skin = recipePickerSkin;
        //scrollPosition = GUI.BeginScrollView(new Rect(0, 70, middleWindow.width - 5, middleWindow.height - 75), scrollPosition, new Rect(0, 0, 0, RecipeManager.allRecipes.Count * 45));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = currentRecipeMin; i < currentRecipeMax; i++)
        {
            GUILayout.BeginHorizontal();
			//if(RecipeManager.ReturnPercentOfRecipe(allSortedList[i].id) > 0 || isFirstTime)
			//{
	            GUILayout.Label(RecipeManager.ReturnPercentOfRecipe(allSortedList[i].id).ToString("f0") + "%");

	            //if (GUI.Button(new Rect(5, 45 * i, middleWindow.width - 40, 44), allSortedList[i].title))
	            if(GUILayout.Button(allSortedList[i].title))
	            {
	                if (ReturnButtonPress())
	                {
	                    //recipeQuickViewOpen = true;

	                    oldRecipePositionIndex = scrollPosition;
	                    scrollPosition = Vector3.zero;
	                    GetRecipeImage(allSortedList[i].imageUrl);
	                    recipeSelected = allSortedList[i].id;
	                    currentMiddleIndex = 5;

	                    if (isFirstTime)
	                    {
	                        tutorialIndex++;
	                    }
	                }
	            }

                //GUILayout.Label(allSortedList[i].percent.ToString("f0") + "%");
               // GUILayout.Label(RecipeManager.ReturnPercentOfRecipe(allSortedList[i].id).ToString("f0") + "%");
			//}

            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
    }

    public int heightAdjuster = 20;

    // selected recipe page
    void DrawSelectedRecipe()
    {
        GUI.skin = recipePickerTopSkin;
        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y, middleWindowNonSort.width + 15, middleWindowNonSort.height + heightAdjuster);

        customRect = GUI.Window(0, customRect, DrawSelectedRecipeWindow, "");
    }
    void DrawSelectedRecipeWindow(int id)
    {
        titleOfPage = RecipeManager.allRecipes[recipeSelected].title;

		GUILayout.BeginHorizontal();
		//GUI.skin = pantrySkin;
        if (GUILayout.Button("< Back", GUILayout.Height(44)))
        {
            currentMiddleIndex = 2;
            scrollPosition = oldRecipePositionIndex;
        }
        if (LoginGUI.instance.Username() == "Dex" || LoginGUI.instance.Username() == "Chris")
        {
            if (GUILayout.Button("Edit", GUILayout.Height(44)))
            {
                EditRecipe();
                currentMiddleIndex = 8;
            }
        }
		if (GUILayout.Button ("Report", GUILayout.Height(44))) {
			for(int i = 0; i < PantryManager.myLikedRecipes.Count; i++)
				Debug.Log(PantryManager.myLikedRecipes[i]);
		}

        if (GUILayout.Button("Cook", GUILayout.Height(44)))
        {
            for (int i = 0; i < RecipeManager.allRecipes[recipeSelected].ings.Count; i++)
            {
                if (!PantryManager.myIngridents.Contains(RecipeManager.allRecipes[recipeSelected].ings[i]))
                {
                    PantryManager.AddToNeededPantry(RecipeManager.allRecipes[recipeSelected].ings[i]);
                }
            }

            PantryManager.SaveNoCheck();
        }
		//================================================================
		// used for checking if the user has already liked the recipe
		//================================================================
		if (PantryManager.AlreadyLiked(RecipeManager.allRecipes[recipeSelected].id)) 
		{
			if (GUILayout.Button ("Unlike", GUILayout.Height (44))) 
			{
				PantryManager.RemoveFromLikedRecipes(RecipeManager.allRecipes[recipeSelected].id);
                PantryManager.SaveNoCheck();
			}
		}
		else
		{
			if (GUILayout.Button ("Like", GUILayout.Height(44))) 
			{
				PantryManager.AddToLikeRecipes(RecipeManager.allRecipes[recipeSelected].id);
                PantryManager.SaveNoCheck();
			}
		}

		GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(middleWindowNonSort.width - 10), GUILayout.Height(middleWindowNonSort.height - heightAdjuster));
		GUI.skin = recipeSkin;

        GUILayout.Label("Author: " + RecipeManager.allRecipes[recipeSelected].author);
        if (GUILayout.Button("Link to Website     >", centerAlign))
        {
            Application.OpenURL(RecipeManager.allRecipes[recipeSelected].url);
        }

        if (selectedRecipeImage)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(selectedRecipeImage, "", GUILayout.Width(256), GUILayout.Height(256));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.Label("Prep Time: " + RecipeManager.allRecipes[recipeSelected].prepTime.ToString() + " mins.");
        GUILayout.Label( "Cook Time: " + RecipeManager.allRecipes[recipeSelected].cookTime.ToString() + " mins.");

        string desc = RecipeManager.allRecipes[recipeSelected].description;
        if(desc[0] != '\"')
            desc = '\"' + RecipeManager.allRecipes[recipeSelected].description + '\"';

        GUILayout.Label("Description: " + desc + "");

        GUILayout.Label("");

        GUILayout.Label("Ingredients");

        //for (int i = 0; i < RecipeManager.allRecipes[recipeSelected].ingredients.Count; i++)
        for (int i = 0; i < RecipeManager.allRecipes[recipeSelected].ings.Count; i++)
        {
            GUI.contentColor = Color.white;
            
            GUILayout.BeginHorizontal();
            //GUI.skin = recipeSkin;

            if (Screen.height > 1024)
            {
                GUILayout.Label(RecipeManager.allRecipes[recipeSelected].ingredientsAmount[i].ToString(), GUILayout.Width(260));
            }
            else
            {
                GUILayout.Label(RecipeManager.allRecipes[recipeSelected].ingredientsAmount[i].ToString(), GUILayout.Width(130));
            }
            
            //GUILayout.Label(String.Format("{0, -40} | {1}",
                    //RecipeManager.allRecipes[recipeSelected].ingredientsAmount[i].ToString(), RecipeManager.allRecipes[recipeSelected].ings[i].name));

            //GUILayout.Label(RecipeManager.allRecipes[recipeSelected].ingredientsAmount[i].ToString().PadRight(15) + "|" + RecipeManager.allRecipes[recipeSelected].ings[i].name);
                
            
            // need something here later to highlight what ingredients we still need
            //if (ReturnIfHaveIngridentName(RecipeManager.allRecipes[recipeSelected].ingredients[i]))
            //if (ReturnIfHaveIngridentName(RecipeManager.allRecipes[recipeSelected].ingredients[i]))
            if (ReturnIfHaveIngridentName(RecipeManager.allRecipes[recipeSelected].ings[i]))
            {
                //style.normal.textColor = Color.white;
                //GUI.skin = gotIngSkin;

                GUILayout.BeginHorizontal();

                GUILayout.Label(RecipeManager.allRecipes[recipeSelected].ings[i].name);
                GUILayout.Label(checkMark, GUILayout.Width(70));

                GUILayout.EndHorizontal();
            }
            else
            {
                //style.normal.textColor = Color.red;
                //GUI.skin = nonIngSkin;

                GUILayout.BeginHorizontal();
                GUILayout.Label(RecipeManager.allRecipes[recipeSelected].ings[i].name);
                //GUILayout.Box(RecipeManager.allRecipes[recipeSelected].ings[i].name);
                GUILayout.Label(xCheck, GUILayout.Width(70));

                GUILayout.EndHorizontal();
                
            }

            //GUILayout.Label(RecipeManager.ReturnNameOfIngrident(RecipeManager.allRecipes[recipeSelected].ingredients[i]));
            

            GUILayout.EndHorizontal();
        }

        //GUI.skin = recipeSkin;
        GUILayout.Label("");
        GUILayout.Label("Steps");

        for (int i = 0; i < RecipeManager.allRecipes[recipeSelected].steps.Count; i++)
        {
            GUILayout.Label("     -Step " + (1 + i).ToString() + ": " + RecipeManager.allRecipes[recipeSelected].steps[i]);
        }


        GUILayout.EndScrollView();
    }

    bool ReturnIfHaveIngridentName( Ing ing )
    {
        for (int i = 0; i < PantryManager.myIngridents.Count; i++)
        {
            if (PantryManager.myIngridents[i].id == ing.id)
            {
                return true;
            }
        }

        return false;

        //if (PantryManager.myIngridents)
        //{
        //    return true;
        //}

        //return false;
    }

    // settings pages
    void DrawSettings()
    {
        GUI.skin = settingsSkin;
        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y, middleWindowNonSort.width + 15, middleWindowNonSort.height + heightAdjuster);

        customRect = GUI.Window(1, customRect, Settings, "");
    }
    void Settings(int id)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(middleWindowNonSort.width + 15), GUILayout.Height(middleWindowNonSort.height - 25));

        
        //GUILayout.Label("Created with: Unity3d");
        //GUILayout.Label("All Rights Reserved");
        //settingsButtonWidth = Screen.width * buttonWidthScaleAmount;


        GUILayout.BeginHorizontal();
        if (GUILayout.Button(settingIcons[0]))
        {
            if (ReturnButtonPress())
                currentMiddleIndex = 12;
        }
        if (GUILayout.Button(settingIcons[7]))
        {
            if (ReturnButtonPress())
                currentMiddleIndex = 4;
        }
        if (GUILayout.Button(settingIcons[2]))
        {
            if (ReturnButtonPress())
                currentMiddleIndex = 11;
        }
        GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();
        if (GUILayout.Button(settingIcons[3]))
        {
            if (ReturnButtonPress())
                currentMiddleIndex = 14;
        }
        

        if (GUILayout.Button(settingIcons[4]))
        {
            if (ReturnButtonPress())
            {
                RecipeMaker.instance.Clear();
                currentMiddleIndex = 8;
            }
        }
        if (GUILayout.Button(settingIcons[5]))
        {
            if (ReturnButtonPress())
                currentMiddleIndex = 10;
        }
        GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();
        if (GUILayout.Button(settingIcons[6]))
        {
            Application.OpenURL("https://itunes.apple.com/us/app/the-recipe-genie/id854557951?ls=1&mt=8");
        }
        if (GUILayout.Button(settingIcons[8]))
        {
            if (ReturnButtonPress())
                currentMiddleIndex = 6;
        }
        if (GUILayout.Button(settingIcons[9]))
        {
            if (ReturnButtonPress())
                LoginGUI.instance.Logout();
        }
        GUILayout.EndHorizontal();



        GUILayout.Label("Username: " + LoginGUI.instance.Username());
        //GUILayout.Label("Email: " + LoginGUI.instance.Email());

        GUILayout.Label("Version: " + versionNumber.ToString());
        
       
        //if (GUILayout.Button("Sync Online List"))
        //{
        //    currentMiddleIndex = 7;
        //}

		//====================================
		// TODO: add in list to keep track of all liked recipes
		//====================================
        
        //if (GUILayout.Button("Feedback"))
        //{
        //    if (ReturnButtonPress())
        //    currentMiddleIndex = 9;
        //}
        
        //if (GUILayout.Button("Remove all Save Data"))
        //{
        //    if (ReturnButtonPress())
        //    PlayerPrefs.DeleteAll();
        //}

		GUILayout.EndScrollView();
    }

    // grocery list page
    void DrawNeededPantry()
    {
        GUI.skin = pantrySkin;
        middleWindowNonSort = GUI.Window(0, middleWindowNonSort, DrawNeededPantryWindow, "");

    }
    void DrawNeededPantryWindow( int windowid )
    {
        scrollPosition = GUI.BeginScrollView(new Rect(0, 25, middleWindowNonSort.width - 5, middleWindowNonSort.height - 30), scrollPosition, new Rect(0, 0, 0, PantryManager.neededIngridents.Count * 79));

        for (int i = 0; i < PantryManager.neededIngridents.Count; i++)
        {
            GUI.skin = gotIngSkin;
            GUI.Label(new Rect(5, 79 * i, middleWindowNonSort.width - 117, 75), PantryManager.neededIngridents[i].name);
            if (GUI.Button(new Rect(middleWindowNonSort.width - 109, 79 * i, 75, 75), xCheck))
            {
                PantryManager.RemoveFromNeededPantry(i);
            }
        }

        GUI.EndScrollView();
    }

    // themes page
    void DrawSkinsPage()
    {
        GUI.skin = pantrySkin;
        middleWindow = GUI.Window(0, middleWindow, SkinsPage, "");
    }
    void SkinsPage( int id )
    {
        GUILayout.Label("Change the color layout.");

        for (int i = 0; i < skinNames.Count(); i++)
        {
            if (GUILayout.Button(skinNames[i]))
            {
                ChangeGUIIndex(i);
            }
        }

        if (GUILayout.Button("Back"))
        {
            currentMiddleIndex = 3;
        }
    }

    // store page
    void DrawDontate()
    {
		GUI.skin = storeSkin;

        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y, middleWindowNonSort.width + 15, middleWindowNonSort.height);
        customRect = GUI.Window(0, customRect, DonatePage, "");

        //middleWindow = GUI.Window(0, middleWindow, DonatePage, "");
    }
    void DonatePage(int id)
    {
        GUILayout.Label("Help us improve this application!");

		if (GUILayout.Button("Unlock Ultimate Version($0.99 USD)", centerAlign))
        {
			// remove ads for 0.99 = ~2k-4k ads per user
            // 500-4000 mins of ads @ 5 mins/day = 100-800 days(~2 years)
			BuyInAppPurchase(0);
        }
		GUILayout.Label("Ultimate Features");
		GUILayout.TextField("-No Ads");
		GUILayout.TextField("-One time purchase for life");
		GUILayout.TextField("-Helping us to improve the app even more");

		/*
		if (GUILayout.Button("Donate", GUILayout.Height(100)))
		{
			// have a donate button for a buck
			OpenStore();
		}*/

        if (GUILayout.Button("Back", centerAlign))
        {
            currentMiddleIndex = 3;
        }
    }

    // feedback page
    void DrawFeedback()
    {
        GUI.skin = recipeSkin;
        middleWindow = GUI.Window(0, middleWindow, FeedbackPage, "");
    }
    void FeedbackPage(int id)
    {
        GUILayout.Label("Like or Hate the app, let us know!",GUILayout.Height(80));
        feedbackText = GUILayout.TextField(feedbackText);// GUILayout.Width(middleWindow.width - 10));

        if (GUILayout.Button("Submit"))
        {
            if (feedbackText != "")
            {
				if(AdManager.instance.IsAdsRunning())
				{
					LoginGUI.instance.UploadFeedback(feedbackText);
				}
				else
				{
					if(Application.platform == RuntimePlatform.IPhonePlayer)
						LoginGUI.instance.UploadFeedback("Ultimate User: " + feedbackText);
					else
						LoginGUI.instance.UploadFeedback(feedbackText);
				}

                

                feedbackText = "Feedback submitted! Thank you.";
            }
        }
        if (GUILayout.Button("Back"))
        {
            currentMiddleIndex = 3;
        }
    }

    // add ingrident editor
    void DrawAddIngridents()
    {
        GUI.skin = addIngSkin;
        middleWindowNonSort = GUI.Window(0, middleWindowNonSort, AddIngridents, "");
    }
    void AddIngridents( int id )
    {
        GUILayout.Label("Add your own ingredients to use in our app below!");
        ingridentText = GUILayout.TextField(ingridentText, GUILayout.Height(80));// GUILayout.Width(middleWindow.width - 10));

        if (GUILayout.Button("Submit"))
        {
            if (ingridentText != "" && !isWaiting)
            {
                LoginGUI.instance.UploadIngrident(ingridentText);

                StartCoroutine(WaitAndClear());
            }
        }
        if (GUILayout.Button("Back"))
        {
            currentMiddleIndex = 3;
        }
    }

    // edit recipe page
    void EditRecipe()
    {
        //Debug.Log(RecipeManager.allRecipes[recipeSelected].idSQL);
        isEditing = true;
        List<Ing> tempIng = new List<Ing>(RecipeManager.allRecipes[recipeSelected].ings);
        List<string> tempSteps = new List<string>(RecipeManager.allRecipes[recipeSelected].steps);
        List<string> tempIngAmt = new List<string>(RecipeManager.allRecipes[recipeSelected].ingredientsAmount);

        RecipeMaker.instance.InsertRecipeForEdit(   RecipeManager.allRecipes[recipeSelected].author, RecipeManager.allRecipes[recipeSelected].url, 
                                                    RecipeManager.allRecipes[recipeSelected].prepTime.ToString(), RecipeManager.allRecipes[recipeSelected].cookTime.ToString(), 
                                                    RecipeManager.allRecipes[recipeSelected].description, RecipeManager.allRecipes[recipeSelected].title,
                                                    RecipeManager.allRecipes[recipeSelected].imageUrl, tempIng,
                                                    tempSteps, tempIngAmt);
    }

    private IEnumerator WaitAndClear()
    {
        ingridentText = "Ingredient submitted! Thank you.";
        isWaiting = true;
        yield return new WaitForSeconds(1.0f);
        ingridentText = "";
        isWaiting = false;
    }

    // draw sync page
    void DrawSyncLocalPage()
    {
        //middleWindow = GUI.Window(0, middleWindow, SyncLocalPage, "");
        GUI.skin = storeSkin;

        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y, middleWindowNonSort.width + 15, middleWindowNonSort.height);
        customRect = GUI.Window(0, customRect, SyncLocalPage, "");
    }
    void SyncLocalPage(int id)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(settingIcons[10]))
        {
            if (ReturnButtonPress())
                Application.OpenURL("https://www.facebook.com/pages/Recipe-Genie/688735421203814?ref=hl");
        }
        if (GUILayout.Button(settingIcons[11]))
        {
            if (ReturnButtonPress())
                Application.OpenURL("http://instagram.com/recipe_genie");
        }
        if (GUILayout.Button(settingIcons[12]))
        {
            if (ReturnButtonPress())
                Application.OpenURL("https://twitter.com/Recipe_Genie");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("This feature will take your local/guest pantry information and add it to your registered pantry list. If you do not " +
        "have a guest account this will do nothing.");

        if (GUILayout.Button("Sync"))
        {
            PantryManager.AppendLocalListWithOnlineList();
        }
        if (GUILayout.Button("Back"))
        {
            currentMiddleIndex = 3;
        }
    }
    // draw sync page
    void DrawSyncOnlinePage()
    {
        //middleWindow = GUI.Window(0, middleWindow, SyncLocalPage, "");
        GUI.skin = storeSkin;

        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y, middleWindowNonSort.width + 15, middleWindowNonSort.height);
        customRect = GUI.Window(0, customRect, SyncLocalPage, "");
    }
    void SyncOnlinePage( int id )
    {
        GUILayout.Label("This feature will take your online regestered pantry information and add it to your local/guest pantry list. If you do not " +
        "have a registered account this will do nothing.");

        if (GUILayout.Button("Sync"))
        {
            PantryManager.AppendOnlineListWithLocalList();
        }
        if (GUILayout.Button("Back"))
        {
            currentMiddleIndex = 3;
        }
    }

    // draw create recipe page
    void DrawCreateRecipe()
    {
        GUI.skin = recipeCreateSkin;

        Rect customRect = new Rect(middleWindowNonSort.x - 8, middleWindowNonSort.y - 3, middleWindowNonSort.width + 15, middleWindowNonSort.height - 3);
        customRect = GUI.Window(0, customRect, CreateRecipePage, "");
    }
    void CreateRecipePage(int id)
    {
        RecipeMaker.instance.CreateRecipe();
    }

    // return A,B,C,D, etc from side buttons
    string GetColumnName(int index)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        var value = "";

        if (index >= letters.Length)
            value += letters[index / letters.Length - 1];

        value += letters[index % letters.Length];

        return value;
    }
    // move scroll button down to correct location
    void SearchBar(int id)
    {
        int yPosition = 0;
        searchText = GetColumnName(id);

        if (currentMiddleIndex == 0)
        {
            for (int i = 0; i < RecipeManager.allIngridents.Count; i++)
            {
                if (searchText.ToUpper()[0] == RecipeManager.allIngridents[i].name[0])
                {
                    yPosition = i;
                    break;
                }
            }
        }
        else if (currentMiddleIndex == 2)
        {
        }
        

        scrollPosition.y = 0;
        //targetScrollPositionY = 0;

        scrollPosition.y = yPosition * 79;
        //targetScrollPositionY = yPosition * 79;
        GUI.FocusWindow(0);
    }
    // sort by name
    void SortByName()
    {
        if (nameSortAssend)
        {
            allSortedList = RecipeManager.allRecipes.OrderByDescending(c => c.title).ThenBy(c => c.percent).ToList();
        }
        else
        {
            allSortedList = RecipeManager.allRecipes.OrderBy(c => c.title).ThenBy(c => c.percent).ToList();
        }

        nameSortAssend = !nameSortAssend;
    }
    // sort by percent of ingridents
    void SortByPercent()
    {
        for (int i = 0; i < allSortedList.Count; i++)
        {
            RecipeManager.ReturnPercentOfRecipe(allSortedList[i].id);
        }

        if (percentSortAssend)
        {
            allSortedList = RecipeManager.allRecipes.OrderBy(c => c.percent).ThenBy(c => c.title).ToList();
        }
        else
        {
            allSortedList = RecipeManager.allRecipes.OrderByDescending(c => c.percent).ThenBy(c => c.title).ToList();
        }

        //percentSortAssend = !percentSortAssend;
    }

    // load all information into user (pantry, liked, grocey list)
    public void LoadAfterLogin()
    {
        currentMiddleIndex = 0;
        PantryManager.Load();
        allSortedList = RecipeManager.allRecipes.OrderByDescending(c => c.percent).ThenBy(c => c.title).ToList();

        InsertMyUploadedRecipes();
    }

    // draw what the current page is
    void SwitchMiddleArea( int index )
    {
        switch (index)
        {
            case 0:
                titleOfPage = "Ingredients";
                DrawPantry();
                break;
            case 1:
                titleOfPage = "My Ingredients";
                DrawMyPantry();
                break;
            case 2:
                titleOfPage = "All Recipes";
                DrawRecipe();
                break;
            case 3:
                titleOfPage = "Settings";
                DrawSettings();
                break;
            case 4:
                titleOfPage = "Grocery List";
                DrawNeededPantry();
                break;
            case 5:
                DrawSelectedRecipe();
                break;
            case 6:
                titleOfPage = "Social";
                DrawSyncLocalPage();
                break;
            case 7:
                titleOfPage = "Social";
                DrawSyncOnlinePage();
                break;
            case 8:
                titleOfPage = "Create Recipe";
                DrawCreateRecipe();
                break;
            case 9:
                titleOfPage = "Feedback";
                DrawFeedback();
                break;
            case 10:
                titleOfPage = "Add Ingredient";
                DrawAddIngridents();
                break;
            case 11:
                titleOfPage = "Liked Recipes";
                DrawLikedRecipe();
                break;
            case 12:
                DrawDontate();
                break;
            case 13:
                DrawSkinsPage();
                break;
            case 14:
                titleOfPage = "My Uploaded Recipes";
                DrawMyUploadedRecipe();
                break;
        }
    }

    // **not used** was used as a search bar maybe future plans
    void DrawTopRect()
    {
        GUI.skin = pantrySkin;
		//float windowHeight = (Screen.height - (Screen.height - middleWindow.height));
        for (int i = 0; i < 26; i++)
        {
			if (GUI.Button(new Rect(topRowRect.x, topRowRect.y + (i * topRowRect.height / 26), topRowRect.width, topRowRect.height / 26), GetColumnName(i)))
            {
                SearchBar(i);
            }
        }
    }

    private bool initRecipeList = false;
    private bool sortRecipes;

    // draw the bottom row of buttons
    void DrawBottomButtons()
    {
        GUI.skin = bottomRowSkin;
        //GUI.Box(bottomRowRect, "");
        GUI.Box(new Rect(0, Screen.height - 75, Screen.width + 2, 80), "");

        if (isFirstTime)
        {
            switch (tutorialIndex)
            {
                case 1:
                    //if (GUI.Button(new Rect(4 + 0 * (Screen.width / 4 - 10), Screen.height - 65, Screen.width / 4 - 10, 60), bottomRowIcons[0]))
                    //if (GUI.Button(new Rect(4 + 0 * (Screen.width / 8 - 10), Screen.height - 71, 70, 70), bottomRowIcons[0]))
                    //if (GUI.Button(new Rect(((Screen.width / 4)) + (0 * (Screen.width / 4) / 2), Screen.height - 71, 70, 70), bottomRowIcons[0]))
                    if (GUI.Button(new Rect((Screen.width / bottomRowIcons.Length * .25f) + (0 * Screen.width / bottomRowIcons.Length), Screen.height - 73, 78, 78), bottomRowIcons[0]))
                    {
                        tutorialIndex++;

                        isEditing = false;
                        targetScrollPositionY = 0;
                        scrollPosition.y = 0;
                        //previousMiddleIndex = currentMiddleIndex;
                        currentMiddleIndex = 0;

                        GUI.FocusWindow(0);
                    }
                    break;
                case 3:
                    //if (GUI.Button(new Rect(4 + 2 * (Screen.width / 4 - 10), Screen.height - 65, Screen.width / 4 - 10, 60), bottomRowIcons[2]))
                    //if (GUI.Button(new Rect(4 + 2 * (Screen.width / 8 - 10), Screen.height - 71, 70, 70), bottomRowIcons[2]))
                    //if (GUI.Button(new Rect(((Screen.width / 4)) + (2 * (Screen.width / 4) / 2), Screen.height - 71, 70, 70), bottomRowIcons[2]))
                    if (GUI.Button(new Rect((Screen.width / bottomRowIcons.Length * .25f) + (2 * Screen.width / bottomRowIcons.Length), Screen.height - 73, 78, 78), bottomRowIcons[2]))
                    {
                        tutorialIndex++;

                        isEditing = false;
                        targetScrollPositionY = 0;
                        scrollPosition.y = 0;
                        //previousMiddleIndex = currentMiddleIndex;
                        currentMiddleIndex = 2;

                        GUI.FocusWindow(0);
                    }
                    break;
                //case 8:
                //    //if (GUI.Button(new Rect(4 + 3 * (Screen.width / 4 - 10), Screen.height - 65, Screen.width / 4 - 10, 60), bottomRowIcons[3]))
                //    if (GUI.Button(new Rect(Screen.width - Screen.width / 8, Screen.height - 71, 70, 70), bottomRowIcons[3]))
                //    {
                //        Debug.Log("F");
                //    }
                //    break;
                case 8:
                    //GUI.Box(new Rect(0, Screen.height - 70, Screen.width, 80), "");

                    //if (GUI.Button(new Rect(4 + 3 * (Screen.width / 4 - 10), Screen.height - 65, Screen.width / 4 - 10, 60), bottomRowIcons[3]))
                    //if (GUI.Button(new Rect(Screen.width - Screen.width / 8, Screen.height - 71, 70, 70), bottomRowIcons[3]))
                    //if (GUI.Button(new Rect(((Screen.width / 4)) + (3 * (Screen.width / 4) / 2), Screen.height - 71, 70, 70), bottomRowIcons[3]))
                    if (GUI.Button(new Rect((Screen.width / bottomRowIcons.Length * .25f) + (3 * Screen.width / bottomRowIcons.Length), Screen.height - 73, 78, 78), bottomRowIcons[3]))
                    {
                        tutorialIndex++;

                        isEditing = false;
                        targetScrollPositionY = 0;
                        scrollPosition.y = 0;
                        //previousMiddleIndex = currentMiddleIndex;
                        currentMiddleIndex = 3;

                        GUI.FocusWindow(0);
                    }
                    break;
            }
        }
        else
        {
            for (int i = 0; i < bottomRowIcons.Length; i++)
            {
                //if (GUI.Button(new Rect(4 + i * (Screen.width / 4 - 10), Screen.height - 72, Screen.width / 4 - 10, 72), ReturnBotRowNames(i)))
                //if (GUI.Button(new Rect(4 + i * (Screen.width / 8 - 10), Screen.height - 71, 70, 70), bottomRowIcons[i]))
                //if (GUI.Button(new Rect((Screen.width / bottomRowIcons.Length * .5f) + (i * Screen.width / bottomRowIcons.Length), Screen.height - 73, 78, 78), bottomRowIcons[i]))
                if (GUI.Button(new Rect((Screen.width / bottomRowIcons.Length * .25f) + (i * Screen.width / bottomRowIcons.Length), Screen.height - 73, 78, 78), bottomRowIcons[i]))
                {
                    if (i == 2 && !initRecipeList)
                    {
                        initRecipeList = true;
                        for (int ii = 0; ii < allSortedList.Count; ii++)
                        {
                            allSortedList[ii].SetPercent(RecipeManager.ReturnPercentOfRecipe(allSortedList[ii].id));
                        }
                    }


                    if (!sortRecipes)
                    {
                        sortRecipes = true;
                        SortByPercent();
                    }

                    isEditing = false;
                    targetScrollPositionY = 0;
                    scrollPosition.y = 0;
                    //previousMiddleIndex = currentMiddleIndex;
                    currentMiddleIndex = i;

                    GUI.FocusWindow(0);
                }

                //GUI.Box(new Rect(4 + i * (Screen.width / 8 - 10), Screen.height - lower, 70, 70), "Ingredients");
            }

            //if (GUI.Button(new Rect(Screen.width - Screen.width / 8, Screen.height - 71, 70, 70), bottomRowIcons[3]))
            //{
            //    isEditing = false;
            //    targetScrollPositionY = 0;
            //    scrollPosition.y = 0;
            //    //previousMiddleIndex = currentMiddleIndex;
            //    currentMiddleIndex = 3;

            //    GUI.FocusWindow(0);
            //}
        }
    }
    // focus window on login rect
    void InitGUIFocusOnStart()
    {
        if (!initGUIStart)
        {
            initGUIStart = true;
            GUI.FocusWindow(0);
        }
    }

    // get the recipe image from the url
    public void GetRecipeImage( string url )
    {
        StartCoroutine(_GetRecipeImage(url));
    }
    private IEnumerator _GetRecipeImage(string url)
    {
        if (url != "")
        {
            WWW www = new WWW(url);
            yield return www;
            Debug.Log(url);
            if (www.error == null)
            {
                if (www.texture != null)
                    selectedRecipeImage = www.texture;
            }
            else
                selectedRecipeImage = null;
        }
        else
            yield return new WaitForEndOfFrame();
    }

    // return the name of the bottom row
    string ReturnBotRowNames(int id)
    {
        string name = "";
        switch (id)
        {
            case 0:
                name = "Ingredients";
                break;
            case 1:
                name = "My Stash";
                break;
            case 2:
                name = "Recipes";
                break;
            case 3:
                name = "Settings";
                break;
        }

        return name;
    }

    // current scroll position
    public Vector2 ScrollPosition
    {
        get { return scrollPosition; }
        set { scrollPosition = value; }
    }
}
