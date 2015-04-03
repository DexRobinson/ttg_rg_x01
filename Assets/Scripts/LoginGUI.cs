using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;


public class LoginGUI : MonoBehaviour {
    public static LoginGUI instance;
    public GUISkin loginSkin;
    public GUISkin signupSkin;

    public Rect loadingWindow;

    //private const string root = "http://localhost/RecipeGenie/Unity/Login/";
    private const string root = "http://www.taptoongames.com/WebPlayer/Unity/Login/";


    private string url = root + "unity_login.php";
    private string loginText = "Sign up to sync your pantry from anywhere!";
    private string username = "Username";
    private string password = "";
    private bool isLoggedIn;
    private string nameURL = root + "unity_return_name.php";
    private string emailURL = root + "unity_return_email.php";
    private string pantryURL = root + "unity_return_pantry.php";
    private string pantryUpdateURL = root + "unity_change_pantry.php";
    private string registerURL = root + "unity_register.php";
    private string feedbackURL = root + "unity_feedback.php";
    private string forgotEmailURL = root + "unity_forgot_email.php";
    private string uploadIngrident = root + "unity_return_ingrident.php";
	private string likedRecipesURL = root + "unity_liked_recipes.php";
    private string neededURL = root + "unity_return_needed.php";
    private string increaseViewCount = root + "unity_increase_view.php";
    private string returnUserUploads = root + "unity_return_uploads.php";

    private bool setfocus;
    private string firstName = "";
    private string lastName = "";
    private string emailText = "";
    private string pantryList = "";
    private string neededList = "";
	private string likedRecipes = "";
    private bool regestring;

    private string errorRegisiter = "";
    private string rePassword = "";
    private Rect middleWindow = new Rect();
    private bool allRecipesLoaded;
    private bool rememberMe;
    private bool forgotPassword;

    private int recipesLoaded;


    public Texture lockTexture;
    public Texture userTexture;
    public Texture check;
    public Texture backTexture;
    public Texture emailTexture;
    public Texture loginCheck;
    public Texture iconTexture;


    public Rect usernameRect;
    public Rect usernameTextureRect;

    public Rect passwordRect;
    public Rect passwordTextureRect;

    public Rect loginButtonRect;
    public Rect guestLoginRect;
    public Rect signupRect;

    public Rect remeberMeLabelRect;
    public Rect remeberMeBoxRect;

    public Rect reportingRect;
    public Rect iconRect;
    public Rect bounsIconRect;

    void Awake()
    {
        instance = this;

        middleWindow = new Rect(5, 105, Screen.width - 10, Screen.height - (0 + 90));
        loadingWindow = new Rect(0.02f * Screen.width, 0.3f * Screen.height, 0.96f * Screen.width, 0.5f * Screen.height);

        //if (PlayerPrefs.GetInt("rememberMe") == 1)
        //{
            rememberMe = true;
            username = PlayerPrefs.GetString("username");
            password = PlayerPrefs.GetString("password");
        //}
    }

    void DrawLogin()
    {
        GUI.skin = loginSkin;
        middleWindow = GUI.Window(0, middleWindow, DrawLoginWindow, "");
    }
    void DrawLoginWindow(int id)
    {
        //GUILayout.Label("Login");
        if (regestring)
        {
            if (forgotPassword)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Username: ", GUILayout.Height(80));
                username = GUILayout.TextField(username, GUILayout.Height(80));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Send Password"))
                {
                    SendEmail();

                    forgotPassword = false;
                    regestring = false;
                    loginText = "Email sent!";
                }
            }
            else
            {
                GUI.skin = signupSkin;

                //errors
                //username
                //password
                //firstname
                //lastname
                //email

                GUILayout.Box(errorRegisiter);


                GUI.SetNextControlName("username");
                GUILayout.BeginHorizontal();
                GUILayout.Label(userTexture, GUILayout.Width(120));
                username = GUILayout.TextField(username);
                username = Regex.Replace(username, @"[^a-zA-Z0-9]", "");
                GUILayout.EndHorizontal();

                GUI.SetNextControlName("pass");
                GUILayout.BeginHorizontal();
                GUILayout.Label(lockTexture, GUILayout.Width(120));
                password = GUILayout.TextField(password);
                password = Regex.Replace(password, @"[^a-zA-Z0-9]", "");
                GUILayout.EndHorizontal();

                GUI.SetNextControlName("repass");
                GUILayout.BeginHorizontal();
                GUILayout.Label(lockTexture, GUILayout.Width(120));
                rePassword = GUILayout.TextField(rePassword);
                rePassword = Regex.Replace(rePassword, @"[^a-zA-Z0-9]", "");
                GUILayout.EndHorizontal();

                GUI.SetNextControlName("email");
                GUILayout.BeginHorizontal();
                GUILayout.Label(emailTexture, GUILayout.Width(120));
                emailText = GUILayout.TextField(emailText);
                emailText = Regex.Replace(emailText, @"[^a-zA-Z0-9@.]", "");
                GUILayout.EndHorizontal();

                if (GUI.GetNameOfFocusedControl() == "username" && username == "username")
                    username = "";
                if (GUI.GetNameOfFocusedControl() == "pass" && password == "password")
                    password = "";
                if (GUI.GetNameOfFocusedControl() == "repass" && rePassword == "password")
                    rePassword = "";
                if (GUI.GetNameOfFocusedControl() == "email" && emailText == "email")
                    emailText = "";

                //GUILayout.BeginHorizontal();
                //GUILayout.Label("First Name: ", GUILayout.Width(120));
                //firstName = GUILayout.TextField(firstName);
                //firstName = Regex.Replace(firstName, @"[^a-zA-Z]", "");
                //GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //GUILayout.Label("Last Name: ", GUILayout.Width(120));
                //lastName = GUILayout.TextField(lastName);
                //lastName = Regex.Replace(lastName, @"[^a-zA-Z]", "");
                //GUILayout.EndHorizontal();


                if (GUILayout.Button(loginCheck))
                {
                    if (password == rePassword)
                    {
                        Register();
                    }
                    else
                    {
                        errorRegisiter = "Your passwords don't match.";
                    }
                }
                if (GUILayout.Button(backTexture))
                {
                    regestring = false;
                }
            }
        }

        if (!isLoggedIn && !regestring)
        {
            Event key = Event.current;
            if (key.keyCode == KeyCode.Return)
            {
                if (key.type == EventType.keyUp)
                {
                    Login();
                }
            }

            //GUILayout.BeginHorizontal();
            //GUI.SetNextControlName("username");

            
            GUI.DrawTexture(ReturnRect(iconRect, bounsIconRect), iconTexture);
            
            

            //GUILayout.Label("Username: ", GUILayout.Height(90), GUILayout.Width(120));
            GUI.Label(ReturnRect(usernameTextureRect), userTexture);
            //GUILayout.Label(userTexture);
            username = GUI.TextField(ReturnRect(usernameRect), username);
            //GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //GUILayout.Label(lockTexture, GUILayout.Height(90), GUILayout.Width(120));
            GUI.Label(ReturnRect(passwordTextureRect), lockTexture);
            password = GUI.PasswordField(ReturnRect(passwordRect), password, "*"[0], 32);
            //GUILayout.EndHorizontal();

            if (GUI.Button(ReturnRect(loginButtonRect), loginCheck))
            {
                Login();
            }

            /*GUILayout.BeginHorizontal();
            GUILayout.Box("Remember Me: ");
            if (rememberMe)
            {
                rememberMe = GUILayout.Toggle(rememberMe, GUIManager.instance.checkMark);
            }
            else
            {
                rememberMe = GUILayout.Toggle(rememberMe, GUIManager.instance.xMark);
            }
			
            GUILayout.EndHorizontal();

            */


            if (GUI.Button(ReturnRect(guestLoginRect), "Guest Login"))
            {
                GuestLogin();
            }




            
            /*if (GUILayout.Button("Forgot Password", GUILayout.Height(90)))
            {
                regestring = true;
                forgotPassword = true;
            }*/


            
            if (GUI.Button(ReturnRect(signupRect), "Sign Up"))
            {
				username = "username";
				password = "password";
                rePassword = "password";
                emailText = "email";
                firstName = "first name";
                lastName = "last name";

                regestring = true;
            }

            GUI.Box(ReturnRect(reportingRect), loginText);

            if (!setfocus)
            {
                //LateFocus();

                if (key.type == EventType.keyDown)
                    setfocus = true;
                if (key.type == EventType.mouseDown)
                    setfocus = true;
            }
        }
    }

    /*void DrawLoginWindow( int id )
    {
        //GUILayout.Label("Login");

        if (regestring)
        {
            if (forgotPassword)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Username: ", GUILayout.Height(80));
                username = GUILayout.TextField(username, GUILayout.Height(80));
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Send Password"))
                {
                    SendEmail();

                    forgotPassword = false;
                    regestring = false;
                    loginText = "Email sent!";
                }
            }
            else
            {
                GUI.skin = signupSkin;
                //errors
                //username
                //password
                //firstname
                //lastname
                //email

                GUILayout.Box(errorRegisiter);


                GUI.SetNextControlName("username");
                GUILayout.BeginHorizontal();
                GUILayout.Label(userTexture, GUILayout.Width(120));
                username = GUILayout.TextField(username);
                username = Regex.Replace(username, @"[^a-zA-Z0-9]", "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(lockTexture, GUILayout.Width(120));
                password = GUILayout.TextField(password);
                password = Regex.Replace(password, @"[^a-zA-Z0-9]", "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(lockTexture, GUILayout.Width(120));
                rePassword = GUILayout.TextField(rePassword);
                rePassword = Regex.Replace(rePassword, @"[^a-zA-Z0-9]", "");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(emailTexture, GUILayout.Width(120));
                emailText = GUILayout.TextField(emailText);
                emailText = Regex.Replace(emailText, @"[^a-zA-Z0-9@.]", "");
                GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //GUILayout.Label("First Name: ", GUILayout.Width(120));
                //firstName = GUILayout.TextField(firstName);
                //firstName = Regex.Replace(firstName, @"[^a-zA-Z]", "");
                //GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //GUILayout.Label("Last Name: ", GUILayout.Width(120));
                //lastName = GUILayout.TextField(lastName);
                //lastName = Regex.Replace(lastName, @"[^a-zA-Z]", "");
                //GUILayout.EndHorizontal();


                if (GUILayout.Button(check))
                {
                    if (password == rePassword)
                    {
                        Register();
                    }
                    else
                    {
                        errorRegisiter = "Your passwords don't match.";
                    }
                }
                if (GUILayout.Button(backTexture))
                {
                    regestring = false;
                }
            }
        }

        if (!isLoggedIn && !regestring)
        {
            Event key = Event.current;
            if (key.keyCode == KeyCode.Return)
            {
                if (key.type == EventType.keyUp)
                {
                    Login();
                }
            }

            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("username");

            //GUILayout.Label("Username: ", GUILayout.Height(90), GUILayout.Width(120));
            GUILayout.Label(userTexture);
            username = GUILayout.TextField(username);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //GUILayout.Label(lockTexture, GUILayout.Height(90), GUILayout.Width(120));
            GUILayout.Label(lockTexture);
            password = GUILayout.PasswordField(password, "*"[0], 32);
            GUILayout.EndHorizontal();

            if (GUILayout.Button(check))
            {
                Login();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box("Remember Me: ");
            if (rememberMe)
            {
                rememberMe = GUILayout.Toggle(rememberMe, GUIManager.instance.checkMark);
            }
            else
            {
                rememberMe = GUILayout.Toggle(rememberMe, GUIManager.instance.xMark);
            }

            GUILayout.EndHorizontal();


            if (GUILayout.Button("Guest Login"))
            {
                GuestLogin();
            }

            /if (GUILayout.Button("Forgot Password", GUILayout.Height(90)))
            {
                regestring = true;
                forgotPassword = true;
            }/

            if (GUILayout.Button("Sign Up"))
            {
                username = "username";
                password = "password";
                rePassword = "password";
                emailText = "email";
                firstName = "first name";
                lastName = "last name";

                regestring = true;
            }

            GUILayout.Box(loginText);

            if (!setfocus)
            {
                LateFocus();

                if (key.type == EventType.keyDown)
                    setfocus = true;
                if (key.type == EventType.mouseDown)
                    setfocus = true;
            }
        }
    }*/


    void OnGUI()
    {
        if (allRecipesLoaded)
        {
            if (!isLoggedIn)
                DrawLogin();

            //GUI.Box(new Rect(0, 0, Screen.width + 5, 50), "Login");
        }
        else
        {
            LoadingWindow();
        }
    }

    void LoadingWindow()
    {
        GUI.skin = loginSkin;
        //GUI.contentColor = Color.white;
        loadingWindow = GUI.Window(0,
                                    loadingWindow, 
                                    DrawLoadingWindow, 
                                    "");
    }

    void DrawLoadingWindow(int id)
    {
        //GUI.contentColor = Color.white;
        GUILayout.Box("Loading Recipes, Please Wait");
        GUILayout.Box("Version: " + GUIManager.instance.versionNumber);
        GUILayout.Box("Recipes Loaded: " + RecipeMaker.instance.RecipeCurrentCount() + " / " + RecipeMaker.instance.RecipeCount());
        GUILayout.Box("Ingridents Loaded: " + RecipeManager.allIngridents.Count);
        GUILayout.Box("Internet Connection Required");
    }

    void LoadUserUploads( string entry )
    {
        string newEntry = entry;
        string[] seperatedEntries = newEntry.Split('*');

        //foreach (string s in seperatedEntries)
        //{
        //    Debug.Log(s);
        //}

        for (int i = 0; i < seperatedEntries.Length - 1; i++)
        {
            PantryManager.myUploadedRecipes.Add(int.Parse(seperatedEntries[i]));
            //Debug.Log("Uploaded: " + PantryManager.myUploadedRecipes[i]);
        }

        //Debug.Log("Count: " + PantryManager.myUploadedRecipes.Count);
    }

    public Rect ReturnRect( Rect rect )
    {
        return new Rect(Screen.width / rect.x, 
            Screen.height / rect.y, 
            Screen.width / rect.width, 
            Screen.height / rect.height);
    }
    public Rect ReturnRect(Rect rect, Rect bouns)
    {
        return new Rect(Screen.width / rect.x + bouns.x,
            Screen.height / rect.y + bouns.y,
            Screen.width / rect.width + bouns.width,
            Screen.height / rect.height + bouns.height);
    }

    void ReturnUserUploads()
    {
        StartCoroutine(_ReturnUserUploads());
    }

    IEnumerator _ReturnUserUploads()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", username);

        WWW www = new WWW(returnUserUploads, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log("There is an error: " + www.error);
        }
        else
        {
            //Debug.Log("Uploaded: " + www.text);
			if(PantryManager.myUploadedRecipes.Count < 1)
            	LoadUserUploads(www.text);
        }

		//isLoggedIn = true;
		GUIManager.instance.LoadAfterLogin ();
    }

    void GuestLogin()
    {
        username = "Guest";
        firstName = "";
        lastName = "";
        emailText = "";
        isLoggedIn = true;

        GUIManager.instance.LoadAfterLogin();

		GestureHandler.InitMaxDistance ();

        StartCoroutine(IncreaseViews());
    }
    IEnumerator IncreaseViews()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", username);
        WWW www = new WWW(increaseViewCount, wwwF);
        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log(www.error);
            loginText = ("There is an error: " + www.error);
        }

        //Debug.Log(www.text);
    }

    public void Login()
    {
        StartCoroutine(_Login());

        if (rememberMe)
        {
            PlayerPrefs.SetInt("rememberMe", 1);
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetString("password", password);

        }
        else
        {
            PlayerPrefs.SetInt("rememberMe", 0);
            PlayerPrefs.SetString("username", "");
            PlayerPrefs.SetString("password", "");
        }

		GestureHandler.InitMaxDistance ();
    }
    IEnumerator _Login()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", username);
        wwwF.AddField("password", password);

        WWW www = new WWW(url, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            loginText = ("There is an error: " + www.error);
        }
        else
        {
            loginText = "Loading you pantry...";
            loginText = www.text;
            if (www.text == "Logged")
            {
				GetEmail();
				GetName();


        		GetPantry();

                while (firstName == "")
                    yield return new WaitForEndOfFrame();

                StartCoroutine(IncreaseViews());

            }
        }
    }

    void GetName()
    {
        StartCoroutine(_GetName());
    }
    IEnumerator _GetName()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", Username());
        wwwF.AddField("password", Password());

        WWW www = new WWW(nameURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            firstName = ("There is an error: " + www.error);
        }
        else
        {
            firstName = www.text;
            
        }
    }

    void GetEmail()
    {
        StartCoroutine(_GetEmail());
    }
    IEnumerator _GetEmail()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", Username());
        wwwF.AddField("password", Password());

        WWW www = new WWW(emailURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            emailText = ("There is an error: " + www.error);
        }
        else
        {
            emailText = www.text;
        }
    }

    void GetNeeded()
    {
        StartCoroutine(_GetNeeded());
    }
    IEnumerator _GetNeeded()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", Username());
        wwwF.AddField("password", Password());

        WWW www = new WWW(neededURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            pantryList = ("There is an error: " + www.error);
        }
        else
        {
            if (www.text != "")
            {
				if(PantryManager.neededIngridents.Count < 1)
                	neededList = www.text;
                //Debug.Log(www.text);
            }
        }

		ReturnUserUploads ();
    }

    void GetPantry()
    {
        StartCoroutine(_GetPantry());
    }
    IEnumerator _GetPantry()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", Username());
        wwwF.AddField("password", Password());

        WWW www = new WWW(pantryURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            pantryList = ("There is an error: " + www.error);
        }
        else
        {
            if (www.text != "")
            {
				if(PantryManager.myIngridents.Count < 1)
                	pantryList = www.text;
                //Debug.Log(www.text);
            }
        }

		GetLikedRecipes ();
    }

	void GetLikedRecipes()
	{
		StartCoroutine(_GetLikedRecipes());
	}
	IEnumerator _GetLikedRecipes()
	{
		WWWForm wwwF = new WWWForm();
		wwwF.AddField("username", Username());
		wwwF.AddField("password", Password());

		//
		// TODO: change
		//
		WWW www = new WWW(likedRecipesURL, wwwF);
		
		while (!www.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		
		if (www.error != null)
		{
			pantryList = ("There is an error: " + www.error);
		}
		else
		{
            if (www.text != "")
            {
				if(PantryManager.myLikedRecipes.Count < 1)
                	likedRecipes = www.text;
                //Debug.Log(www.text);
            }
		}

		GetNeeded ();
	}

    public void UploadPantry()
    {
        StartCoroutine(_UploadPantry());
    }
    IEnumerator _UploadPantry()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", Username());
        wwwF.AddField("password", Password());
        wwwF.AddField("pantry", PantryListV);

		//Debug.Log ("Uncomment liked recipes section once php code is updated...");
		wwwF.AddField ("likedrecipes", LikeRecipes);
        wwwF.AddField("needed_ing", NeededIng);

		Debug.Log (LikeRecipes);
        WWW www = new WWW(pantryUpdateURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
        }
        else
            Debug.Log(www.text);
    }

    public void UploadIngrident(string name)
    {
        StartCoroutine(_UploadIngrident(name));
    }
    IEnumerator _UploadIngrident(string name)
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("name", name);

        WWW www = new WWW(uploadIngrident, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log(www.text);
        }
    }

    public void Register()
    {
        StartCoroutine(_Register());
    }
    IEnumerator _Register()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", username);
        wwwF.AddField("password", password);
        wwwF.AddField("firstname", firstName);
        wwwF.AddField("lastname", lastName);
        wwwF.AddField("email", emailText);

        WWW www = new WWW(registerURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            errorRegisiter = www.text;

        }
        else
        {
            Debug.Log(www.text);
            errorRegisiter = www.text;

			Login();
        }
    }

    public void UploadFeedback(string message)
    {
        StartCoroutine(_UploadFeedback(message));
    }
    IEnumerator _UploadFeedback(string message)
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("name", Username());
        wwwF.AddField("message", message);
        wwwF.AddField("email", Email());
        
        WWW www = new WWW(feedbackURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.text);
        }
    }

    public void SendEmail()
    {
        StartCoroutine(_SendEmail());
    }
    IEnumerator _SendEmail()
    {
        WWWForm wwwF = new WWWForm();
        wwwF.AddField("username", username);

        WWW www = new WWW(forgotEmailURL, wwwF);

        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.error != null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.text);
        }
    }

    public bool IsLoggedIn()
    {
        return isLoggedIn;
    }
	public void LoggedIn(bool isLogged)
	{
		isLoggedIn = isLogged;
	}
    void LateFocus()
    {
        GUI.FocusControl("username");
    }
    public void Logout()
    {
        PantryManager.myIngridents.Clear();
        pantryList = "";
        loginText = "";
        username = "";
        firstName = "";
        lastName = "";
        password = "";
        emailText = "";
        isLoggedIn = false;
        regestring = false;
    }

    public string Username()
    {
        return username;
    }
    public string Password()
    {
        return password;
    }
    public string FirstName()
    {
        return firstName;
    }
    public string LastName()
    {
        return lastName;
    }
    public string Email()
    {
        return emailText;
    }
    public string PantryList()
    {
        return pantryList;
    }
    public string PantryListV
    {
        get{return pantryList;}
        set { pantryList = value; }
    }
	public string LikeRecipes
	{
		get{return likedRecipes;}
		set { likedRecipes = value; }
	}
    public bool AllRecipesLoad
    {
        get { return allRecipesLoaded; }
        set { allRecipesLoaded = value; }
    }
    public string NeededIng
    {
        get { return neededList; }
        set { neededList = value; }
    }
}
