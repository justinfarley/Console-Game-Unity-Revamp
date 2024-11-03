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

    [SerializeField] private Gradient healthGradient;
    public void Display()
    {
        playerName.text = $"<color=yellow>{Player.UserName}</color>";
        enemyName.text = $"{CurrentEnemy.DisplayName}";

        enemyHealthBar.fillAmount = (float)CurrentEnemy.Health / CurrentEnemy.MaxHealth;
        playerHealthBar.fillAmount = (float)Player.Instance.Health / Player.Instance.MaxHealth;

        enemyHealthBar.color = healthGradient.Evaluate((float)CurrentEnemy.Health / CurrentEnemy.MaxHealth);
        playerHealthBar.color = healthGradient.Evaluate((float)Player.Instance.Health / Player.Instance.MaxHealth);

        //TODO: enemySprite.sprite = ...
        //TODO: playerSprite.sprite = ...
    }
}