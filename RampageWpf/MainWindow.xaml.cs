using Microsoft.AspNetCore.SignalR.Client;
using RampageWpf.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace RampageWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> gameHistory;
        private Room currentRoom;
        private Player currentPlayer;
        public event PropertyChangedEventHandler? PropertyChanged;
        private int round = 0;
        private bool canChange = true;
        private List<RoomAction> actions;
        private RoomAction selectedAction;
        private static readonly HubConnection client = new HubConnectionBuilder().WithUrl($"{Config.ServerName}/game").WithAutomaticReconnect().Build();

        private void Signal([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Player CurrentPlayer
        {
            get => currentPlayer;
            set
            {
                currentPlayer = value;
                Signal();
            }
        }

        public bool CanChange
        {
            get => canChange;
            set
            {
                canChange = value;
                Signal();
            }
        }
        public Room CurrentRoom
        {
            get => currentRoom;
            set
            {
                currentRoom = value;
                Signal();
            }
        }
        public List<RoomAction> Actions
        {
            get => actions;
            set
            {
                actions = value;
                Signal();
            }
        }
        public RoomAction SelectedAction
        {
            get => selectedAction;
            set
            {
                selectedAction = value;
                Signal();
            }
        }

        public ObservableCollection<string> GameHistory
        {
            get => gameHistory;
            set
            {
                gameHistory = value;
                Signal();
            }
        }

        public MainWindow()
        {
            Closing += OnClosing;
            InitializeComponent();
            Initialize();
            DataContext = this;
        }

        public async void OnClosing(object sender, CancelEventArgs e)
        {
            await client.StopAsync();
        }


        private async Task Initialize()
        {
            await client.StartAsync();
            CurrentPlayer = new();
            CurrentRoom = new();
            CanChange = true;
            GameHistory = [];
            client.On<Room>("StartRound", async (room) =>
            {
                Dispatcher.Invoke(() =>
                {
                    GameHistory.Add($"Раунд {round++}");
                    CurrentRoom = room;

                    room.PlayerState.TryGetValue(currentPlayer.Name, out var player);
                    if (player is null)
                    {
                        CurrentPlayer.HP = 0;
                        return;
                    }
                    CurrentPlayer = player;
                    //
                    GetInfoRoom(CurrentRoom);
                });
            });

            //Что произошло за ход
            client.On<List<string>>("PastRoundInfo", (actions) =>
            {
                Dispatcher.InvokeAsync(() => 
                {
                    foreach (string action in actions)
                    {
                        Console.WriteLine(action);
                    }
                });
            });

            //Конец игры
            client.On<Player>("Winner", player =>
            {
                Dispatcher.InvokeAsync(async () => 
                {
                    GameHistory.Add($"Конец игры. Победитель - {player.Name} c {player.HP} HP");
                    await Initialize();
                });
                
            });

            Actions = [new RoomAction { ActionType = 1}, new() { ActionType = 2}];
        }

        

        private int GetInfoRoom(Room room)
        {
            for (int i = 0; i < room.PlayerState.Count; i++)
            {
                var player = room.PlayerState.ElementAt(i).Value;
                GameHistory.Add($"{i + 1} {player.Name} - {player.HP} HP");
            }
            return room.PlayerState.Count;
        }

        private async void JoinGame_Click(object sender, RoutedEventArgs e)
        {
            //GameHistory.Add("Введите имя");
            bool result;
            result = await client.InvokeAsync<bool>("Registration", CurrentPlayer.Name);
            if (!result)
            {
                GameHistory.Add("Не удалось подключиться, попробуйте снова\n");
            }
            else
            {
                CanChange = false;
                GameHistory.Add($"Вы вошли под именем {CurrentPlayer.Name}");
            }
        }

        private async void ChooseTarget_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction.GroupId = CurrentRoom.Number;
            SelectedAction.Actor = CurrentPlayer.Name;
            await client.SendAsync("PickAction", SelectedAction);
            SelectedAction = new() { Actor = CurrentPlayer.Name, GroupId = CurrentRoom.Number };
        }
    }
}