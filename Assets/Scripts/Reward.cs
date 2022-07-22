[System.Serializable]
public class Reward 
{
    public string name;
    public GoodsType type;
    public int num;

    public Reward(string _name, GoodsType _type, int _num)
    {
        name = _name;
        type = _type;
        num = _num;
    }
}