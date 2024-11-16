using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using System;

public class AdManager : MonoBehaviour
{
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    private string _adUnitId = "ca-app-pub-4348570103813665/1002389856";

    private bool IsInterAdLoaded = false;

    private string InitadUnitId = "ca-app-pub-4348570103813665/9413566645"; // 테스트 전면 광고 단위 ID


    void Start()
    {
        GameRoot.Instance.WaitTimeAndCallback(2f, () => {
            MobileAds.Initialize(initStatus => {
                LoadRewardedAd();
                LoadInterstitialAd();
            });
        });
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(InitadUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                IsInterAdLoaded = true;

                _interstitialAd.OnAdFullScreenContentClosed += HandleInterstitialAdClosed;
            });
    }

    // 전면 광고 표시
    public void ShowInterstitialAd()
    {
        if (IsInterAdLoaded && _interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
            Debug.Log("Interstitial ad is being shown.");
            IsInterAdLoaded = false; // 광고 표시 후 다시 로드 필요
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet.");
        }
    }

    public void HandleInterstitialAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        // 광고가 닫힌 후 다시 로드
        LoadInterstitialAd();
    }
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load with error: " + error);
                // 여기에서 추가적인 디버그 메시지나 오류를 처리할 수 있습니다.
                return;
                }

                Debug.Log("Rewarded ad loaded with response: " + ad.GetResponseInfo());

                _rewardedAd = ad;

            // 광고가 로드되면 이벤트 핸들러 등록
            RegisterEventHandlers(_rewardedAd);

            // 광고가 닫혔을 때 이벤트 핸들러
            _rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
            });
    }
    // 리워드 광고 표시
    public void ShowRewardedAd(System.Action rewardaction)
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                // 보상 처리를 위한 콜백 호출
                rewardaction?.Invoke();

                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    // 사용자가 보상을 받을 때
    public void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log("User earned reward: " + reward.Amount);
        // 여기에서 보상 지급 로직을 구현하세요
    }

    // 광고가 닫혔을 때
    public void HandleRewardedAdClosed()
    {
        Debug.Log("Rewarded ad closed.");
        // 광고가 닫히면 새로운 광고 로드
        LoadRewardedAd();
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
}