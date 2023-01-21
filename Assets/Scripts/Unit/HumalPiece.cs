[System.Serializable]
public class HumalPiece
{
    public int id;
    public string name;
    public int amount;

    public HumalPiece(int _id, string _name, int _amount)
    {
        id = _id;
        name = _name;
        amount = _amount;
    }
}