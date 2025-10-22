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
    
    public void Setup(long amount)
    {
        text.text = $"+{amount}";
        text.color = originalColor;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        Color c = text.color;
        c.a -= fadeSpeed * Time.deltaTime;
        text.color = c;

        if (text.color.a <= 0f)
            Destroy(gameObject);
    }
}