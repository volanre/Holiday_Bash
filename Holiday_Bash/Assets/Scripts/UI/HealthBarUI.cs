using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private float health, maxHealth;
    public float width, height;

    [SerializeField] private RectTransform healthbar;

    public void setMaxHealth(float maxHP)
    {
        maxHealth = maxHP;
    }
    public void setCurrentHealth(float curHP)
    {
        health = curHP;
        float newWidth = (health / maxHealth) * width;
        healthbar.sizeDelta = new Vector2(newWidth, height);
    }
}
