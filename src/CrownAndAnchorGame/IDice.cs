namespace CrownAndAnchorGame
{
    public interface IDice
    {
        DiceValue CurrentValue { get; }
        string CurrentValueRepr { get; }
        DiceValue roll();
    }
}