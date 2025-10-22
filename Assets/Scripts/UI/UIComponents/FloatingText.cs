using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 50f;      // 위로 이동 속도
    public float fadeSpeed = 2f;       // 사라지는 속도
    private TextMeshProUGUI text;
    private Color originalColor;
    
    public TextMeshProUGUI goldText;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    void OnEnable()
    {
        // 처음엔 완전 불투명
        text.color = originalColor;
    }

    void Update()
    {
        // 위로 천천히 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        goldText.text = $"+ {GameManager.Instance.GetStatValue(UpgradeType.CurrencyGain) +1}";

        // 투명해지기
        Color c = text.color;
        c.a -= fadeSpeed * Time.deltaTime;
        text.color = c;

        // 완전히 사라지면 파괴
        if (text.color.a <= 0f)
            Destroy(gameObject);
    }
}