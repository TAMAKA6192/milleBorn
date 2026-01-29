namespace milleBorn;

public class Deck {
    private readonly List<Card> _cards;
    private readonly Random _random;

    public int Count => _cards.Count;

    public Deck() {
        _random = new Random();
        _cards = [];
        InitializeDeck();
        Shuffle();
    }

    private void InitializeDeck() {
        // Distance cards (移動カード) - 106 cards total
        AddCards(CardType.Distance25, 10);
        AddCards(CardType.Distance50, 10);
        AddCards(CardType.Distance75, 10);
        AddCards(CardType.Distance100, 12);
        AddCards(CardType.Distance200, 4);

        // Hazard cards (妨害カード) - 42 cards total
        AddCards(CardType.Accident, 3);
        AddCards(CardType.OutOfGas, 3);
        AddCards(CardType.FlatTire, 3);
        AddCards(CardType.SpeedLimit, 4);
        AddCards(CardType.Stop, 5);

        // Remedy cards (修理カード) - 38 cards total
        AddCards(CardType.Repairs, 6);
        AddCards(CardType.Gasoline, 6);
        AddCards(CardType.SpareTire, 6);
        AddCards(CardType.EndOfLimit, 6);
        AddCards(CardType.Go, 14);

        // Safety cards (安全カード) - 4 cards total
        AddCards(CardType.DrivingAce, 1);
        AddCards(CardType.ExtraTank, 1);
        AddCards(CardType.PunctureProof, 1);
        AddCards(CardType.RightOfWay, 1);
    }

    private void AddCards(CardType type, int count) {
        for (var i = 0; i < count; i++) {
            _cards.Add(CardFactory.CreateCard(type));
        }
    }

    public void Shuffle() {
        for (var i = _cards.Count - 1; i > 0; i--) {
            var j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public Card? Draw() {
        if (_cards.Count == 0) {
            return null;
        }

        Card card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public List<Card> DrawMultiple(int count) {
        List<Card> drawn = [];
        for (var i = 0; i < count && _cards.Count > 0; i++) {
            Card? card = Draw();
            if (card != null) {
                drawn.Add(card);
            }
        }

        return drawn;
    }
}
