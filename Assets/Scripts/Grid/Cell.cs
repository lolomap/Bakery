using GameData;
namespace Grid
{
    public interface ICell
    {
        public bool IsEmpty { get; }
    }
    
    public class Cell<TItem> : ICell where TItem : ItemSO
    {
        public int X { get; }
        public int Y { get; }
        public TItem Item { get; private set; }
        public bool IsEmpty => Item == null;

        public Cell(int x, int y, TItem item = null)
        {
            X = x;
            Y = y;
            Item = item;
        }
        
        public void Set(TItem newItem) => Item = newItem;

        public void Clear() => Item = default(TItem);
    }
}
