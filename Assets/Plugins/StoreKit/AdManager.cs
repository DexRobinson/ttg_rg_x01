using UnityEngine;
using System.Collections;

public class AdManager : MonoBehaviour {
	public static AdManager instance;
	private int bannerIsEnabled; // this is used to check if ads can be run using this...

	#if UNITY_IPHONE
<<<<<<< HEAD
	private UnityEngine.iOS.ADBannerView banner = null;
=======
	private ADBannerView banner = null;
    private ADInterstitialAd fullScreenAd = null;
>>>>>>> origin/master
    #endif

	void Awake()
	{
		instance = this;
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			bannerIsEnabled = PlayerPrefs.GetInt ("bannerIsEnabled", 0);
            #if UNITY_IPHONE
            if(bannerIsEnabled == 0)
            {
                fullScreenAd = new ADInterstitialAd();
                ADInterstitialAd.onInterstitialWasLoaded += PlayFullScreenAd;
			    //banner = new ADBannerView (ADBannerView.Type., ADBannerView.Layout.Top);
			    //ADBannerView.onBannerWasClicked += OnBannerClicked;
			    //ADBannerView.onBannerWasLoaded += OnBannerLoaded;
            }
			else
			{
				HideBanner();
			}
            #endif
        } 
		else 
		{
			bannerIsEnabled = 1;
		}
	}

    #if UNITY_IPHONE
	void OnBannerClicked()
	{
		Debug.Log("Clicked!\n");
	}
	
	void OnBannerLoaded()
	{
		if (bannerIsEnabled == 0) 
		{
			ShowBanner();
		}
	}
	// hide the ad banner
	public void HideBanner()
	{
        if (bannerIsEnabled == 0) 
		    banner.visible = false;
	}
	// display the ad banner
	public void ShowBanner()
	{
        if (bannerIsEnabled == 0) 
		    banner.visible = true;
	}
	// this is for when you unlock ads
	public void UnlockAds()
	{
		PlayerPrefs.SetInt ("bannerIsEnabled", 1);
	}

    public void PlayFullScreenAd()
    {
        StartCoroutine(_PlayFullScreenAd());
    }
    private IEnumerator _PlayFullScreenAd()
    {
        while (!fullScreenAd.loaded)
            yield return new WaitForEndOfFrame();

        if (fullScreenAd.loaded)
            fullScreenAd.Show();
    }

#endif

    public bool IsAdsRunning()
	{
		if (bannerIsEnabled == 1) {
			return false;
		}

		return true;
	}

}
