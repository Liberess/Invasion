[System.Serializable]
public struct HumalPiece
{
    public string name;
    public int amount;

    public HumalPiece(string _name, int _amount)
    {
        name = _name;
        amount = _amount;
    }
}

[System.Serializable]
public class HumalPieceDictionary : SerializableDictionary<string, int> { }