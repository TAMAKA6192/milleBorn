namespace milleBorn
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("======================");
            Console.WriteLine("  ミルボーン (Mille Bornes)");
            Console.WriteLine("======================\n");

            // Create players: 1 human + 3 AI
            var players = new List<Player>
            {
                new Player("あなた", isHuman: true),
                new Player("CPU1", isHuman: false),
                new Player("CPU2", isHuman: false),
                new Player("CPU3", isHuman: false)
            };

            var gameState = new GameState(players);
            var aiPlayer = new AIPlayer();

            // Deal initial hands
            gameState.DealInitialHands();

            Console.WriteLine("ゲーム開始! 最初に1000kmに到達したプレイヤーが勝利です。\n");

            // Main game loop
            while (!gameState.IsGameOver())
            {
                var currentPlayer = gameState.CurrentPlayer;
                
                Console.WriteLine(gameState.GetGameStatus());
                Console.WriteLine($"\n--- {currentPlayer.Name}のターン ---");

                if (currentPlayer.IsHuman)
                {
                    PlayHumanTurn(currentPlayer, gameState);
                }
                else
                {
                    PlayAITurn(currentPlayer, gameState, aiPlayer);
                    System.Threading.Thread.Sleep(1000); // Pause for readability
                }

                // Draw a card at the end of turn
                if (currentPlayer.Hand.Count < 6)
                {
                    gameState.DrawCard(currentPlayer);
                }

                gameState.NextPlayer();
            }

            // Game over
            Console.WriteLine("\n======================");
            Console.WriteLine("  ゲーム終了!");
            Console.WriteLine("======================");
            Console.WriteLine(gameState.GetGameStatus());
            
            if (gameState.Winner != null)
            {
                Console.WriteLine($"\n勝者: {gameState.Winner.Name} ({gameState.Winner.TotalDistance}km)");
            }

            Console.WriteLine("\nEnterキーを押して終了...");
            Console.ReadLine();
        }

        static void PlayHumanTurn(Player player, GameState gameState)
        {
            bool turnComplete = false;

            while (!turnComplete)
            {
                Console.WriteLine($"\n手札 ({player.Hand.Count}枚):");
                for (int i = 0; i < player.Hand.Count; i++)
                {
                    var card = player.Hand[i];
                    string info = GetCardInfo(card, player);
                    Console.WriteLine($"  {i + 1}. {card.Name} {info}");
                }

                Console.Write("\nカードを選択 (1-{0}): ", player.Hand.Count);
                if (int.TryParse(Console.ReadLine(), out int choice) && 
                    choice >= 1 && choice <= player.Hand.Count)
                {
                    var selectedCard = player.Hand[choice - 1];
                    
                    if (selectedCard.Category == CardCategory.Hazard)
                    {
                        var targets = gameState.GetValidTargets(player, selectedCard);
                        if (targets.Any())
                        {
                            Console.WriteLine("\n対象を選択:");
                            for (int i = 0; i < targets.Count; i++)
                            {
                                Console.WriteLine($"  {i + 1}. {targets[i].Name} ({targets[i].TotalDistance}km)");
                            }
                            
                            Console.Write("選択 (1-{0}, 0=キャンセル): ", targets.Count);
                            if (int.TryParse(Console.ReadLine(), out int targetChoice))
                            {
                                if (targetChoice == 0)
                                    continue;
                                    
                                if (targetChoice >= 1 && targetChoice <= targets.Count)
                                {
                                    var target = targets[targetChoice - 1];
                                    if (gameState.PlayCard(player, selectedCard, target))
                                    {
                                        Console.WriteLine($"\n{selectedCard.Name}を{target.Name}に使用しました。");
                                        turnComplete = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n有効な対象がいません。このカードは使用できません。");
                            Console.WriteLine("捨てますか? (y/n): ");
                            if (Console.ReadLine()?.ToLower() == "y")
                            {
                                gameState.DiscardCard(player, selectedCard);
                                Console.WriteLine($"\n{selectedCard.Name}を捨てました。");
                                turnComplete = true;
                            }
                        }
                    }
                    else
                    {
                        if (gameState.PlayCard(player, selectedCard))
                        {
                            Console.WriteLine($"\n{selectedCard.Name}を使用しました。");
                            turnComplete = true;
                        }
                        else
                        {
                            Console.WriteLine("\nそのカードは今使用できません。");
                            Console.WriteLine("捨てますか? (y/n): ");
                            if (Console.ReadLine()?.ToLower() == "y")
                            {
                                gameState.DiscardCard(player, selectedCard);
                                Console.WriteLine($"\n{selectedCard.Name}を捨てました。");
                                turnComplete = true;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("無効な選択です。");
                }
            }
        }

        static void PlayAITurn(Player player, GameState gameState, AIPlayer ai)
        {
            var (card, target) = ai.ChooseAction(player, gameState);
            string action = ai.ExplainAction(card, target);

            Console.WriteLine($"{player.Name}: {action}");

            if (target != null)
            {
                if (!gameState.PlayCard(player, card, target))
                {
                    gameState.DiscardCard(player, card);
                }
            }
            else
            {
                if (!gameState.PlayCard(player, card))
                {
                    gameState.DiscardCard(player, card);
                }
            }
        }

        static string GetCardInfo(Card card, Player player)
        {
            return card.Category switch
            {
                CardCategory.Distance => player.CanPlayDistance(card.Distance) ? "" : "(使用不可)",
                CardCategory.Remedy => player.CanPlayRemedy(card) ? "" : "(使用不可)",
                CardCategory.Hazard => "(妨害カード)",
                CardCategory.Safety => "(安全カード)",
                _ => ""
            };
        }
    }
}
