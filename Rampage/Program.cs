using Microsoft.AspNetCore.SignalR.Client;
using Rampage.Model;
using System.Threading.Tasks;

namespace Rampage
{
    internal class Program
    {

        private static readonly HubConnection client = new HubConnectionBuilder().WithUrl("").WithAutomaticReconnect().Build();
        private static readonly Player currentPlayer = new();
        private static Room currentRoom = new();
        private static string currentRoomName;
        static async Task Main(string[] args)
        {
            bool game = true;
            while (!await JoinGame())
            {
            }

            //При начале раунда/хода
            client.On<Room>("StartRound", async (room) =>
            {
                currentRoom = room;
                int action = await ChooseAction();
                await client.SendAsync("PickAction", new RoomAction { RoomId = currentRoomName, ActionType = action});
            });

            //Конец игры
            client.On("EndGame", () =>
            {
                game = false;
            });



            while (game)
            {
                Console.ReadLine();
            }
            //Console.WriteLine("Hello, World!");
        }

        private static async Task<bool> JoinGame()
        {
            Console.WriteLine("Введите имя");
            string result;
            currentPlayer.Name = GetUserInput();
            result = await client.InvokeAsync<string>("Registration", currentPlayer.Name);
            if (result == "fail")
            {
                Console.WriteLine("Не удалось подключиться, попробуйте снова\n");
                return false;
            }
            currentRoomName = result;
            //currentRoom.Number = result;
            return true; ;
        }
        private static async Task<int> ChooseAction()
        {


            return 0;
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
