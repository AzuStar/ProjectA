namespace ProjectA.Game.Singletons;

public partial class PlayerSingleton
{
    private int _coinsCollected;

    public int CoinsCollected
    {
        get => _coinsCollected;
        set
        {
            if (_coinsCollected == value)
                return;

            _coinsCollected = value;
            UpdateText();
        }
    }

    public int BaxPattedTimes;
}
