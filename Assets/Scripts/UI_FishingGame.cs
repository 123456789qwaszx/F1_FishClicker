using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FishingGame : UI_Popup
{
    [Header("Fish UI")]
    public Image Img_Fish;
    public TextMeshProUGUI Txt_FishName;
    public Slider Slider_FishHP;
    public TextMeshProUGUI Txt_FishHP;
    //public TextMeshProUGUI Txt_RewardPreview;

    void OnEnable()
    {
        // EventManager.Instance.AddEvent(EEventType.OnNewFishSpawn, InitFishUI);
        // EventManager.Instance.AddEvent(EEventType.OnAttackFish, UpdateFishHpUI);
        // EventManager.Instance.AddEvent(EEventType.OnFishDefeated, OnFishDefeated);

        // 초기 물고기 스폰
        //FishManager.Instance.SpawnNewFish();
    }

    void OnDisable()
    {
        // EventManager.Instance.RemoveEvent(EEventType.OnNewFishSpawn, InitFishUI);
        // EventManager.Instance.RemoveEvent(EEventType.OnAttackFish, UpdateFishHpUI);
        // EventManager.Instance.RemoveEvent(EEventType.OnFishDefeated, OnFishDefeated);
    }

    public void InitFishUI()
    {
        //FishData fish = FishManager.Instance.GetCurrentFish();
        Img_Fish.sprite = Resources.Load<Sprite>("Trash/Char/Img_Juno000"); //fish.fishSprite;
        // Txt_FishName.text = fish.fishName;
        // Txt_RewardPreview.text = $"{fish.reward} G";
        // Slider_FishHP.maxValue = fish.maxHP;
        // Slider_FishHP.value = fish.maxHP;
        // Txt_FishHP.text = $"{fish.maxHP}/{fish.maxHP}";
    }

    public void OnClickFish()
    {
        //FishManager.Instance.OnClickFish();
    }

    void UpdateFishHpUI()
    {
        // int curHp = FishManager.Instance.currentHP;
        // int maxHp = FishManager.Instance.GetCurrentFish().maxHP;

        // Slider_FishHP.value = curHp;
        // Txt_FishHP.text = $"{curHp}/{maxHp}";
    }

    void OnFishDefeated()
    {
        // 애니메이션이나 효과 넣고 싶으면 여기서
        Debug.Log("UI: 물고기 잡음, 다음 물고기 스폰 예정");
    }
}