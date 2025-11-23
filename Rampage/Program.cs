using Microsoft.AspNetCore.SignalR.Client;
using Rampage.Model;

namespace Rampage
{
    internal class Program
    {

        private static readonly HubConnection client = new HubConnectionBuilder().WithUrl($"{Config.ServerName}/game").WithAutomaticReconnect().Build();
        private static Player currentPlayer = new();
        private static Room currentRoom = new();
        private static int round = 1;
        private static bool game = false;
        private static int state = 1;
        static async Task Main(string[] args)
        {
            game = true;
            await client.StartAsync();
            await Initialize();
            while (!await JoinGame())
            {
            }

            while (game)
            {
                switch (state)
                {
                    case 1:

                        break;


                    
                }
            }
            //while (game)
            //{
            //    Console.ReadLine();
            //}
        }

        private static async Task<bool> JoinGame()
        {
            Console.WriteLine("Введите имя");
            bool result;
            currentPlayer.Name = GetUserInput();
            result = await client.InvokeAsync<bool>("Registration", currentPlayer.Name);
            if (!result)
            {
                Console.WriteLine("Не удалось подключиться, попробуйте снова\n");
                return false;
            }
            //currentRoom.Number = result;
            return true;
        }
        private static async Task Initialize()
        {
            //При начале раунда/хода
            client.On<Room>("StartRound", async (room) =>
            {
                Console.WriteLine($"Раунд {round++}");
                currentRoom = room;
                
                room.PlayerState.TryGetValue(currentPlayer.Name, out var player);
                if (player is null)
                {
                    currentPlayer.HP = 0;
                    return;
                }
                currentPlayer = player;
                state = 1;
                //
                GetInfoRoom(currentRoom);
                RoomAction action = new()
                {
                    Actor = currentPlayer.Name,
                    GroupId = room.Number,
                    ActionType = ChooseAction(),
                    Target = ChooseTarget(room).Name
                };

                await client.SendAsync("PickAction", action);
            });

            //Что произошло за ход
            client.On<List<string>>("PastRoundInfo", (actions) =>
            {
                foreach (string action in actions)
                {
                    Console.WriteLine(action);
                }
            });

            //Конец игры
            client.On<Player>("Winner", player =>
            {
                Console.WriteLine($"Конец игры. Победитель - {player.Name} c {player.HP} HP");
                game = false;
            });
        }
        private static int ChooseAction()
        {
            Console.WriteLine("Выберите действие:\n1 - Атака\n2 - Защита");
            int action;
            while (!int.TryParse(GetUserInput(), out action) || action < 1 || action > 2)
            {
                Console.WriteLine("Введите ноиер действия.");
            }
            return action;
        }
        private static Player ChooseTarget(Room room)
        {
            Console.WriteLine("Выберите цель:");
            int targetsCount = GetInfoRoom(room);
            int idTarget;
            while (!int.TryParse(GetUserInput(), out idTarget) || idTarget < 1 || idTarget > targetsCount || room.PlayerState.ElementAtOrDefault(idTarget).Value is not null  || room.PlayerState.ElementAt(idTarget).Value != currentPlayer)
            {
                Console.WriteLine("Неверная цель, попробуйте снова");
            }
            return room.PlayerState.ElementAt(idTarget).Value;
        }
        private static int GetInfoRoom(Room room)
        {
            for (int i = 0; i < room.PlayerState.Count; i++)
            {
                var player = room.PlayerState.ElementAt(i).Value;
                Console.WriteLine($"{i + 1} {player.Name} - {player.HP} HP");
            }
            return room.PlayerState.Count;
        }
        private static string GetUserInput()
        {
            string input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Неверный ввод");
                input = Console.ReadLine();
            }

            return input;
        }

    }
}
