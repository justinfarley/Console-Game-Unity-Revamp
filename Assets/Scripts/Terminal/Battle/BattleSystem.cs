using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class BattleSystem
{
    public static Enemy CurrentEnemy { get; private set; } = null;
    public static async Task StartBattle(Enemy enemy)
    {
        CurrentEnemy = enemy;
        if (Player.Weapon == null)
        {
            Debug.Log("You don't have a weapon!!!");
            return;
        }

        ConsoleController.ChangeConsoleState(ConsoleController.ConsoleState.Battle);
        ACG.AddToPath($"<color=red>{CurrentEnemy.Name}_Fight</color>/");
        await ACG.Display($"You encountered a wild {CurrentEnemy.DisplayName}!");

        await Awaitable.WaitForSecondsAsync(1f);

        ACG.SpawnBattleUI(ConsoleController.Controller.transform).Display();

        while (enemy.Health > 0 && Player.Instance.Health > 0) //TODO: fix later, temporary while loop
        {
            await TakeTurn(); //TODO: also eventually have some minigame for how good the attack is instead of randomly picking the num in the range
        }
        ConsoleController.ChangeConsoleState(ConsoleController.ConsoleState.Default);
        ACG.ResetPath();
        CurrentEnemy = null;
    }
    private static async Task TakeTurn()
    {
        if (ACG.PlayerGoesFirst(CurrentEnemy))
        {
            //what would you like to do
            string output = await ACG.DisplayWithPrompt(ACG.PickRandom(ACG.BattlePrompts) + $"\n{Command.GetListOfCommands()}", true);

            Command cmd = new Command(output);

            await ConsoleController.RunCommand(cmd, false);

            await ACG.Display($"{CurrentEnemy.DisplayName} is thinking...");
            await Awaitable.WaitForSecondsAsync(1f);
            await ACG.Display($"{CurrentEnemy.DisplayName} is thinking...");
            await Awaitable.WaitForSecondsAsync(1f);

            //enemy attack
            Enemy.AttackType attackType = CurrentEnemy.ChooseAttack();

            switch (attackType)
            {
                case Enemy.AttackType.Weapon:
                    var attackResult = CurrentEnemy.Weapon.DealDamage(Player.Instance);

                    ACG.SpawnBattleUI(ConsoleController.Controller.transform).Display();

                    await Awaitable.WaitForSecondsAsync(0.25f);

                    await ACG.Display($"The {CurrentEnemy.DisplayName} attacked <color=yellow>YOU</color>!\n" +
                                                     (attackResult.WasCrit
                                                        ? $"<color=yellow>It's a Critical Hit!</color> You were hit for <color=yellow>{attackResult.Damage}</color> Damage!\n"
                                                        : $"You were hit for <color=red>{attackResult.Damage}</color> Damage!\n"
                                                     )
                                                     + $"<color=yellow>{Player.UserName}</color>'s Health: <color=green>{attackResult.OtherHealthBefore}</color> -> <color=red>{attackResult.OtherHealthAfter}</color>");
                    break;
                case Enemy.AttackType.HeldItem:
                    if(CurrentEnemy.HeldItem is Potion)
                    {
                        PotionUseData data = CurrentEnemy.HeldItem.Use(Player.Instance) as PotionUseData;

                        await Awaitable.WaitForSecondsAsync(0.25f);

                        await ACG.Display($"The {CurrentEnemy.DisplayName} was holding a(n) {CurrentEnemy.HeldItem.DisplayName}!\n" +
                                                         $"The {CurrentEnemy.HeldItem.DisplayName} was used up: {CurrentEnemy.DisplayName}'s Health: <color=red>{data.HealthBeforeUse}</color> -> <color=red>{data.HealthAfterUse}</color>{((data.HealthAfterUse >= CurrentEnemy.MaxHealth) ? "(MAX)" : string.Empty)}");
                    }
                    break;
            }
        }
        else
        {
            //enemy random move
            await ACG.DisplayWithPrompt(ACG.PickRandom(ACG.BattlePrompts) + $"\n{Command.GetListOfCommands()}");

            //what would you like to do
        }
    }
    public class BattleData
    {
        public TMP_Text playerName;
        public TMP_Text enemyName;
        
        public Image enemyHealthBar;
        public Image playerHealthBar;
        
        public Image enemySprite;
        public Image playerSprite;

        public Enemy Enemy;

        public BattleData(TMP_Text playerName, TMP_Text enemyName, Image enemyHealthBar,  Image playerHealthBar, Image enemySprite, Image playerSprite, Enemy enemy)
        {
            this.playerName = playerName;
            this.enemyName = enemyName;

            this.enemySprite = enemySprite;
            this.playerSprite = playerSprite;

            this.enemyHealthBar = enemyHealthBar;
            this.playerHealthBar = playerHealthBar;
            Enemy = enemy;
        }
    }
}
