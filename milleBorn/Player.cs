namespace milleBorn
{
    public class Player
    {
        public string Name { get; }
        public bool IsHuman { get; }
        public List<Card> Hand { get; }
        public int TotalDistance { get; private set; }
        public List<Card> SafetyCards { get; }
        public List<Card> DistanceCards { get; }

        // Battle pile - current status
        public Card? CurrentBattle { get; private set; }
        public Card? SpeedLimitCard { get; private set; }

        // Game state flags
        public bool CanMove => CurrentBattle == null || CurrentBattle.Type == CardType.Go;
        public bool HasSpeedLimit => SpeedLimitCard != null;
        public bool HasRightOfWay => SafetyCards.Any(c => c.Type == CardType.RightOfWay);
        public bool HasDrivingAce => SafetyCards.Any(c => c.Type == CardType.DrivingAce);
        public bool HasExtraTank => SafetyCards.Any(c => c.Type == CardType.ExtraTank);
        public bool HasPunctureProof => SafetyCards.Any(c => c.Type == CardType.PunctureProof);

        public Player(string name, bool isHuman = false)
        {
            Name = name;
            IsHuman = isHuman;
            Hand = new List<Card>();
            SafetyCards = new List<Card>();
            DistanceCards = new List<Card>();
            TotalDistance = 0;
        }

        public void AddCardToHand(Card card)
        {
            Hand.Add(card);
        }

        public void RemoveCardFromHand(Card card)
        {
            Hand.Remove(card);
        }

        public bool CanPlayDistance(int distance)
        {
            if (!CanMove) return false;

            int maxDistance = HasSpeedLimit ? 50 : int.MaxValue;
            if (distance > maxDistance) return false;

            return TotalDistance + distance <= 1000;
        }

        public void PlayDistance(Card card)
        {
            if (card.Category != CardCategory.Distance)
                throw new InvalidOperationException("Not a distance card");

            TotalDistance += card.Distance;
            DistanceCards.Add(card);
        }

        public void PlaySafety(Card card)
        {
            if (card.Category != CardCategory.Safety)
                throw new InvalidOperationException("Not a safety card");

            SafetyCards.Add(card);

            // Safety cards also clear matching hazards
            switch (card.Type)
            {
                case CardType.RightOfWay:
                    CurrentBattle = CardFactory.CreateCard(CardType.Go);
                    SpeedLimitCard = null;
                    break;
                case CardType.DrivingAce:
                    if (CurrentBattle?.Type == CardType.Accident)
                        CurrentBattle = CardFactory.CreateCard(CardType.Go);
                    break;
                case CardType.ExtraTank:
                    if (CurrentBattle?.Type == CardType.OutOfGas)
                        CurrentBattle = CardFactory.CreateCard(CardType.Go);
                    break;
                case CardType.PunctureProof:
                    if (CurrentBattle?.Type == CardType.FlatTire)
                        CurrentBattle = CardFactory.CreateCard(CardType.Go);
                    break;
            }
        }

        public void ApplyHazard(Card hazard)
        {
            if (hazard.Category != CardCategory.Hazard)
                throw new InvalidOperationException("Not a hazard card");

            // Check if protected by safety card
            switch (hazard.Type)
            {
                case CardType.Accident when HasDrivingAce:
                case CardType.OutOfGas when HasExtraTank:
                case CardType.FlatTire when HasPunctureProof:
                case CardType.Stop when HasRightOfWay:
                    return; // Protected
                case CardType.SpeedLimit when HasRightOfWay:
                    return; // Protected
            }

            if (hazard.Type == CardType.SpeedLimit)
            {
                SpeedLimitCard = hazard;
            }
            else
            {
                CurrentBattle = hazard;
            }
        }

        public bool CanPlayRemedy(Card remedy)
        {
            if (remedy.Category != CardCategory.Remedy)
                return false;

            return remedy.Type switch
            {
                CardType.Go => CurrentBattle?.Type == CardType.Stop,
                CardType.Repairs => CurrentBattle?.Type == CardType.Accident,
                CardType.Gasoline => CurrentBattle?.Type == CardType.OutOfGas,
                CardType.SpareTire => CurrentBattle?.Type == CardType.FlatTire,
                CardType.EndOfLimit => HasSpeedLimit,
                _ => false
            };
        }

        public void PlayRemedy(Card remedy)
        {
            if (!CanPlayRemedy(remedy))
                throw new InvalidOperationException("Cannot play this remedy");

            if (remedy.Type == CardType.EndOfLimit)
            {
                SpeedLimitCard = null;
            }
            else
            {
                CurrentBattle = CardFactory.CreateCard(CardType.Go);
            }
        }

        public bool IsProtectedFrom(CardType hazardType)
        {
            return hazardType switch
            {
                CardType.Accident => HasDrivingAce,
                CardType.OutOfGas => HasExtraTank,
                CardType.FlatTire => HasPunctureProof,
                CardType.Stop => HasRightOfWay,
                CardType.SpeedLimit => HasRightOfWay,
                _ => false
            };
        }

        public string GetStatus()
        {
            string status = $"{Name}: {TotalDistance}km";
            if (!CanMove && CurrentBattle != null)
            {
                status += $" [{CurrentBattle.Name}]";
            }
            else if (CanMove)
            {
                status += " [ˆÚ“®‰Â”\]";
            }
            if (HasSpeedLimit)
            {
                status += " [‘¬“x§ŒÀ]";
            }
            return status;
        }
    }
}
