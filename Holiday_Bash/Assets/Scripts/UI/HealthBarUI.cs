using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    //So the class is called healthbar but really its used for all the player ui bars
    private float health, maxHealth;
    public float width, height;
    private bool textEnabled = true;

    [SerializeField] private RectTransform healthbar;
    [SerializeField] private GameObject text;

    public void setMaxHealth(float maxHP)
    {
        maxHealth = maxHP;
        if (textEnabled) UpdateText();
    }
    public void setCurrentHealth(float curHP)
    {
        health = curHP;
        float newWidth = (health / maxHealth) * width;
        healthbar.sizeDelta = new Vector2(newWidth, height);
        if (textEnabled) UpdateText();

    }
    public void UpdateText(bool over = false, string value = "")
    {
        string thingy = "" + health.ToString() + "/" + maxHealth.ToString();
        if (over) thingy = value;
        text.GetComponent<TextMeshProUGUI>().SetText(thingy);
    }
    
    public void ToggleText(bool state)
    {
        text.SetActive(state);
        textEnabled = state;
        if (textEnabled) UpdateText();
    }
}
