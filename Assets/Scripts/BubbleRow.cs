[System.Serializable]
public class BubbleRow
{
    public int[] row;

    public int sizeOfRow()
    {
        return row.Length;
    }

    public int getElement(int x) {
        return row[x];
    }
}
