using UnityEngine;
using TMPro;

public class SpawnEnemyUI : MonoBehaviour
{
    private CustomSprite customSprite;

    private TextMeshProUGUI text;

    void Awake()
    {
        customSprite = GetComponentInChildren<CustomSprite>();

        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void setEnemy(Enemy enemy, KeyCode keyCode)
    {
        customSprite.LoadNewSprite(enemy.GetFilename());
        text.text = enemy.GetName() + " (" + keyCode + ")";
    }
}
