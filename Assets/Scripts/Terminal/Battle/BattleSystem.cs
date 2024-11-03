using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class BattleSystem
{
    public static async Task StartBattle(Enemy enemy)
    {
        if (PlayerStats.Weapon == null)
        {
            Debug.Log("You don't have a weapon!!!");
            return;
        }

        ConsoleController.ChangeConsoleState(ConsoleController.ConsoleState.Battle);
        ACG.AddToPath($"<color=red>{enemy.Name}_Fight</color>/");
        await ACG.Display($"You encountered a wild <color=red>{enemy.Name}</color>!");

        await Awaitable.WaitForSecondsAsync(1f);

        ACG.SpawnBattleUI(ConsoleController.Controller.transform).GetComponent<BattleUIDisplay>().Display(enemy);

        while (enemy.Health > 0 && PlayerStats.Health > 0) //TODO: fix later, temporary while loop
        {
            await TakeTurn(enemy); //TODO: also eventually have some minigame for how good the attack is instead of randomly picking the num in the range
        }
        ConsoleController.ChangeConsoleState(ConsoleController.ConsoleState.Default);
        ACG.ResetPath();
    }
    private static async Task TakeTurn(Enemy enemy)
    {
        if (ACG.PlayerGoesFirst(enemy))
        {
            //what would you like to do
            string output = await ACG.DisplayWithPrompt(ACG.PickRandom(ACG.BattlePrompts) + $"\n{Command.GetListOfCommands()}");

            Command cmd = new Command(output);

            await ConsoleController.RunCommand(cmd);

            //enemy random move
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
