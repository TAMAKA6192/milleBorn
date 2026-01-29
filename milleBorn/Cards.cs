namespace milleBorn
{
    public enum CardType
    {
        // Distance Cards (ˆÚ“®ƒJ[ƒh)
        Distance25,
        Distance50,
        Distance75,
        Distance100,
        Distance200,

        // Hazard Cards (–WŠQƒJ[ƒh)
        Accident,      // Ž–ŒÌ
        OutOfGas,      // ƒKƒXŒ‡
        FlatTire,      // ƒpƒ“ƒN
        SpeedLimit,    // ‘¬“x§ŒÀ
        Stop,          // ’âŽ~

        // Remedy Cards (C—ƒJ[ƒh)
        Repairs,       // C—
        Gasoline,      // ƒKƒ\ƒŠƒ“
        SpareTire,     // ƒXƒyƒAƒ^ƒCƒ„
        EndOfLimit,    // ‘¬“x§ŒÀ‰ðœ
        Go,            // o”­

        // Safety Cards (ˆÀ‘SƒJ[ƒh)
        DrivingAce,    // ‰^“]‚Ì’Bl (Ž–ŒÌ–hŽ~)
        ExtraTank,     // —\”õƒ^ƒ“ƒN (ƒKƒXŒ‡–hŽ~)
        PunctureProof, // ƒpƒ“ƒN–hŽ~
        RightOfWay     // —DæŒ  (’âŽ~E‘¬“x§ŒÀ–hŽ~)
    }

    public class Card
    {
        public CardType Type { get; }
        public string Name { get; }
        public int Distance { get; }
        public CardCategory Category { get; }

        public Card(CardType type, string name, int distance, CardCategory category)
        {
            Type = type;
            Name = name;
            Distance = distance;
            Category = category;
        }

        public override string ToString() => Name;
    }

    public enum CardCategory
    {
        Distance,
        Hazard,
        Remedy,
        Safety
    }

    public static class CardFactory
    {
        public static Card CreateCard(CardType type)
        {
            return type switch
            {
                CardType.Distance25 => new Card(type, "25km", 25, CardCategory.Distance),
                CardType.Distance50 => new Card(type, "50km", 50, CardCategory.Distance),
                CardType.Distance75 => new Card(type, "75km", 75, CardCategory.Distance),
                CardType.Distance100 => new Card(type, "100km", 100, CardCategory.Distance),
                CardType.Distance200 => new Card(type, "200km", 200, CardCategory.Distance),

                CardType.Accident => new Card(type, "Ž–ŒÌ", 0, CardCategory.Hazard),
                CardType.OutOfGas => new Card(type, "ƒKƒXŒ‡", 0, CardCategory.Hazard),
                CardType.FlatTire => new Card(type, "ƒpƒ“ƒN", 0, CardCategory.Hazard),
                CardType.SpeedLimit => new Card(type, "‘¬“x§ŒÀ", 0, CardCategory.Hazard),
                CardType.Stop => new Card(type, "’âŽ~", 0, CardCategory.Hazard),

                CardType.Repairs => new Card(type, "C—", 0, CardCategory.Remedy),
                CardType.Gasoline => new Card(type, "ƒKƒ\ƒŠƒ“", 0, CardCategory.Remedy),
                CardType.SpareTire => new Card(type, "ƒXƒyƒAƒ^ƒCƒ„", 0, CardCategory.Remedy),
                CardType.EndOfLimit => new Card(type, "‘¬“x§ŒÀ‰ðœ", 0, CardCategory.Remedy),
                CardType.Go => new Card(type, "o”­", 0, CardCategory.Remedy),

                CardType.DrivingAce => new Card(type, "‰^“]‚Ì’Bl", 0, CardCategory.Safety),
                CardType.ExtraTank => new Card(type, "—\”õƒ^ƒ“ƒN", 0, CardCategory.Safety),
                CardType.PunctureProof => new Card(type, "ƒpƒ“ƒN–hŽ~", 0, CardCategory.Safety),
                CardType.RightOfWay => new Card(type, "—DæŒ ", 0, CardCategory.Safety),

                _ => throw new ArgumentException($"Unknown card type: {type}")
            };
        }
    }
}
