namespace milleBorn;

public class GameState {
    public List<Player> Players { get; }
    public Deck Deck { get; }
    public List<Card> DiscardPile { get; }
    public int CurrentPlayerIndex { get; private set; }
    public bool GameEnded { get; private set; }
    public Player? Winner { get; private set; }

    private const int HandSize = 6;
    private const int WinningDistance = 1000;

    public Player CurrentPlayer => Players[CurrentPlayerIndex];

    public GameState(List<Player> players) {
        if (players.Count is < 2 or > 4) {
            throw new ArgumentException("Game requires 2-4 players");
        }

        Players = players;
        Deck = new Deck();
        DiscardPile = [];
        CurrentPlayerIndex = 0;
        GameEnded = false;
    }

    public void DealInitialHands() {
        foreach (var player in Players) {
            var cards = Deck.DrawMultiple(HandSize);
            foreach (var card in cards) {
                player.AddCardToHand(card);
            }
        }
    }

    public void NextPlayer() => CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;

    public bool PlayCard(Player player, Card card, Player? targetPlayer = null) {
        if (!player.Hand.Contains(card)) {
            return false;
        }

        var cardPlayed = false;

        switch (card.Category) {
        case CardCategory.Distance:
            if (player.CanPlayDistance(card.Distance)) {
                player.PlayDistance(card);
                cardPlayed = true;
            }

            break;

        case CardCategory.Safety:
            player.PlaySafety(card);
            cardPlayed = true;
            break;

        case CardCategory.Remedy:
            if (player.CanPlayRemedy(card)) {
                player.PlayRemedy(card);
                cardPlayed = true;
            }

            break;

        case CardCategory.Hazard:
            if (targetPlayer != null && targetPlayer != player) {
                targetPlayer.ApplyHazard(card);
                cardPlayed = true;
            }

            break;
        }

        if (cardPlayed) {
            player.RemoveCardFromHand(card);
            DiscardPile.Add(card);

            // Check for winner
            if (player.TotalDistance >= WinningDistance) {
                GameEnded = true;
                Winner = player;
            }

            return true;
        }

        return false;
    }

    public bool DiscardCard(Player player, Card card) {
        if (!player.Hand.Contains(card)) {
            return false;
        }

        player.RemoveCardFromHand(card);
        DiscardPile.Add(card);
        return true;
    }

    public void DrawCard(Player player) {
        if (Deck.Count > 0) {
            var card = Deck.Draw();
            if (card != null) {
                player.AddCardToHand(card);
            }
        }
    }

    public List<Player> GetValidTargets(Player attacker, Card hazardCard) => hazardCard.Category != CardCategory.Hazard
            ? []
            : Players
            .Where(p => p != attacker && !p.IsProtectedFrom(hazardCard.Type))
            .ToList();

    public string GetGameStatus() {
        var status = "\n=== ゲーム状況 ===\n";
        status += $"山札: {Deck.Count}枚\n";
        status += $"捨札: {DiscardPile.Count}枚\n\n";

        foreach (var player in Players) {
            status += $"{player.GetStatus()}\n";
            if (player.SafetyCards.Any()) {
                status += $"  安全カード: {string.Join(", ", player.SafetyCards.Select(c => c.Name))}\n";
            }
        }

        return status;
    }

    public bool IsGameOver() {
        if (GameEnded) {
            return true;
        }

        // Game also ends if deck is empty and no one can play
        if (Deck.Count == 0) {
            var anyoneCanPlay = Players.Any(p => p.Hand.Any());
            if (!anyoneCanPlay) {
                GameEnded = true;
                Winner = Players.OrderByDescending(p => p.TotalDistance).First();
                return true;
            }
        }

        return false;
    }
}
