namespace milleBorn;

public enum CardType {
    // Distance Cards (移動カード)
    Distance25,
    Distance50,
    Distance75,
    Distance100,
    Distance200,

    // Hazard Cards (妨害カード)
    Accident,      // 事故
    OutOfGas,      // ガス欠
    FlatTire,      // パンク
    SpeedLimit,    // 速度制限
    Stop,          // 停止

    // Remedy Cards (修理カード)
    Repairs,       // 修理
    Gasoline,      // ガソリン
    SpareTire,     // スペアタイヤ
    EndOfLimit,    // 速度制限解除
    Go,            // 出発

    // Safety Cards (安全カード)
    DrivingAce,    // 運転の達人 (事故防止)
    ExtraTank,     // 予備タンク (ガス欠防止)
    PunctureProof, // パンク防止
    RightOfWay     // 優先権 (停止・速度制限防止)
}

public class Card {
    public CardType Type { get; }
    public string Name { get; }
    public int Distance { get; }
    public CardCategory Category { get; }

    public Card(CardType type, string name, int distance, CardCategory category) {
        Type = type;
        Name = name;
        Distance = distance;
        Category = category;
    }

    public override string ToString() => Name;
}

public enum CardCategory {
    Distance,
    Hazard,
    Remedy,
    Safety
}

public static class CardFactory {
    public static Card CreateCard(CardType type) => type switch {
        CardType.Distance25 => new Card(type, "25km", 25, CardCategory.Distance),
        CardType.Distance50 => new Card(type, "50km", 50, CardCategory.Distance),
        CardType.Distance75 => new Card(type, "75km", 75, CardCategory.Distance),
        CardType.Distance100 => new Card(type, "100km", 100, CardCategory.Distance),
        CardType.Distance200 => new Card(type, "200km", 200, CardCategory.Distance),

        CardType.Accident => new Card(type, "事故", 0, CardCategory.Hazard),
        CardType.OutOfGas => new Card(type, "ガス欠", 0, CardCategory.Hazard),
        CardType.FlatTire => new Card(type, "パンク", 0, CardCategory.Hazard),
        CardType.SpeedLimit => new Card(type, "速度制限", 0, CardCategory.Hazard),
        CardType.Stop => new Card(type, "停止", 0, CardCategory.Hazard),

        CardType.Repairs => new Card(type, "修理", 0, CardCategory.Remedy),
        CardType.Gasoline => new Card(type, "ガソリン", 0, CardCategory.Remedy),
        CardType.SpareTire => new Card(type, "スペアタイヤ", 0, CardCategory.Remedy),
        CardType.EndOfLimit => new Card(type, "速度制限解除", 0, CardCategory.Remedy),
        CardType.Go => new Card(type, "出発", 0, CardCategory.Remedy),

        CardType.DrivingAce => new Card(type, "運転の達人", 0, CardCategory.Safety),
        CardType.ExtraTank => new Card(type, "予備タンク", 0, CardCategory.Safety),
        CardType.PunctureProof => new Card(type, "パンク防止", 0, CardCategory.Safety),
        CardType.RightOfWay => new Card(type, "優先権", 0, CardCategory.Safety),

        _ => throw new ArgumentException($"Unknown card type: {type}")
    };
}
