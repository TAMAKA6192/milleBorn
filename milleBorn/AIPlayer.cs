namespace milleBorn;

public class AIPlayer {
    private readonly Random _random;

    public AIPlayer() => _random = new Random();

    public (Card card, Player? target) ChooseAction(Player aiPlayer, GameState gameState) {
        // Priority 1: Play safety cards immediately
        var safetyCard = aiPlayer.Hand.FirstOrDefault(c => c.Category == CardCategory.Safety);
        if (safetyCard != null) {
            return (safetyCard, null);
        }

        // Priority 2: Play remedy if stuck
        if (!aiPlayer.CanMove || aiPlayer.HasSpeedLimit) {
            var remedyCard = aiPlayer.Hand.FirstOrDefault(c =>
                c.Category == CardCategory.Remedy && aiPlayer.CanPlayRemedy(c));
            if (remedyCard != null) {
                return (remedyCard, null);
            }
        }

        // Priority 3: Play distance cards if possible
        if (aiPlayer.CanMove) {
            var distanceCards = aiPlayer.Hand
                .Where(c => c.Category == CardCategory.Distance && aiPlayer.CanPlayDistance(c.Distance))
                .OrderByDescending(c => c.Distance)
                .ToList();

            if (distanceCards.Any()) {
                return (distanceCards.First(), null);
            }
        }

        // Priority 4: Play hazard cards on opponents
        var hazardCards = aiPlayer.Hand.Where(c => c.Category == CardCategory.Hazard).ToList();
        if (hazardCards.Any()) {
            foreach (var hazard in hazardCards) {
                var targets = gameState.GetValidTargets(aiPlayer, hazard);
                if (targets.Any()) {
                    // Target the player with the most distance
                    var target = targets.OrderByDescending(p => p.TotalDistance).First();
                    return (hazard, target);
                }
            }
        }

        // Priority 5: Discard a card if nothing can be played
        // Discard remedy cards first (least useful when you don't need them)
        var cardToDiscard = aiPlayer.Hand.FirstOrDefault(c => c.Category == CardCategory.Remedy)
                         ?? aiPlayer.Hand.FirstOrDefault(c => c.Category == CardCategory.Hazard)
                         ?? aiPlayer.Hand.FirstOrDefault();

        if (cardToDiscard != null) {
            return (cardToDiscard, null);
        }

        // Should never reach here
        return (aiPlayer.Hand.First(), null);
    }

    public string ExplainAction(Card card, Player? target) {
        if (target != null) {
            return $"{card.Name}を{target.Name}に使用";
        } else if (card.Category == CardCategory.Distance) {
            return $"{card.Name}を移動";
        } else if (card.Category == CardCategory.Remedy) {
            return $"{card.Name}を使用";
        } else {
            return card.Category == CardCategory.Safety ? $"{card.Name}を配置" : $"{card.Name}を捨てる";
        }
    }
}
