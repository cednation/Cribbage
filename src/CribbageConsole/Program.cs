global using Cribbage;
global using Cribbage.Reporting;
using CribbageConsole;

var player1 = new Player("Alan", new PlayerHandFactory());
//var player1 = new Player("Alan", new UserConsoleHandFactory());
var player2 = new Player("Bob", new PlayerHandFactory());

var reporter = new GameReporter(new HandReporter(new ThePlayReporter()));

string gameLogFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".txt");
using var fileLogger = new CribbageGameFileLogger(gameLogFilePath);
fileLogger.SubscribeToReporter(reporter);
fileLogger.Start();

Console.WriteLine("Starting a game between Alan and Bob");
Console.WriteLine($"See the game sequence by accessing the game log @ {fileLogger.FileName}");
var game = new CribbageGame(player1, player2, new DeckFactory());
game.Play(CribbageGame.FirstDealerChoice.CutForDeal, reporter);

fileLogger.Stop();
Console.WriteLine("Game completed!");
Console.WriteLine("Press any key to exit");
Console.ReadKey();