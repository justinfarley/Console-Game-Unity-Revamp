using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BattleSystem;

public class BattleUIDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text enemyName;

    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private Image playerHealthBar;

    [SerializeField] private Image enemySprite;
    [SerializeField] private Image playerSprite;

    public void Display(Enemy enemy)
    {
        playerName.text = $"<color=yellow>{PlayerStats.UserName}</color>";
        enemyName.text = $"<color=red>{enemy.Name}</color>";

        enemyHealthBar.fillAmount = enemy.Health / enemy.BaseHealth;
        playerHealthBar.fillAmount = PlayerStats.Health / PlayerStats.MaxHealth;

        //TODO: enemySprite.sprite = ...
        //TODO: playerSprite.sprite = ...
    }
}