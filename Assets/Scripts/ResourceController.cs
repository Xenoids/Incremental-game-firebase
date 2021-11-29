﻿using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    private int _index;
    private int _level
    {
        set{
            // simpan value yang di set ke _level pada Progress data
            UserDataManager.Progress.ResLevels[_index] =value;
            UserDataManager.Save(true);
        }

        get{
            // Mengecek apakah idx sudah terdapat pada progress data
            if(!UserDataManager.HasResources(_index))
            {
                // Jika tidak, tampilkan level 1
                return 1;
            }

            // Jika iya, tampilan berdasarkan Progress data
            return UserDataManager.Progress.ResLevels[_index];

        }
    }

    public bool IsUnlocked { get; private set; }

    private void Start ()
    {
        ResourceButton.onClick.AddListener (() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel ();
            }
            else
            {
                UnlockResource ();
            }
        });
    }

    public void SetConfig (int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";

        SetUnlocked (_config.UnlockCost == 0 || UserDataManager.HasResources(_index));
    }

    public double GetOutput ()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost ()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost ()
    {
        return _config.UnlockCost;
    }

    public void UpgradeLevel ()
    {
        double upgradeCost = GetUpgradeCost ();
        if (UserDataManager.Progress.Gold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold (-upgradeCost);
        _level++;

        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
        AnalyticsManager.LogUpgradeEvent(_index, _level);
    }

    public void UnlockResource ()
    {
        double unlockCost = GetUnlockCost ();
        if (UserDataManager.Progress.Gold < unlockCost)
        {
            return;
        }

        SetUnlocked (true);
        GameManager.Instance.ShowNextResource ();
        AchievementController.Instance.UnlockAchievement (AchievementType.UnlockResource, _config.Name);
        AnalyticsManager.LogUnlockEvent(_index);
    }

    public void SetUnlocked (bool unlocked)
    {
        IsUnlocked = unlocked;
        if(unlocked)
        {
            // Jika res baru di Unlock dan belum ada di Progress Data, maka tambahkan data
            if(!UserDataManager.HasResources(_index))
            {
                UserDataManager.Progress.ResLevels.Add(_level);
                UserDataManager.Save(true);
            }
        }

        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive (!unlocked);
        ResourceUpgradeCost.gameObject.SetActive (unlocked);
    }
}