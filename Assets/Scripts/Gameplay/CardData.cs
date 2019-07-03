namespace Cards.Gameplay
{
    public enum CardType
    {
        Attack,
        Deffence,
        Support
    }

    [System.Serializable]
    public struct CardData
    {
        public string name;
        public int health;
        public int requiredEnenergy;
        public int damage;

        public CardType type;
    }
}
