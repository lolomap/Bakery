using GameData;
namespace Grid
{
    public class Grid<TItem> where TItem : ItemSO
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Cell<TItem>[,] _content;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            _content = new Cell<TItem>[width,height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _content[x, y] = new(x, y);
                }
            }
        }

        public TItem Get(int x, int y) => _content[x, y].Item;
        public void Set(int x, int y, TItem newItem) => _content[x, y].Set(newItem);
        public void Set(int x, int y, TItem newItem, out TItem prevItem)
        {
            prevItem = _content[x, y].Item;
            _content[x, y].Set(newItem);
        }

        public bool CanPlace(int x, int y, TItem item)
        {
            if (x < 0 || x >= Width) return false;
            if (y < 0 || y >= Height) return false;

            return _content[x, y].IsEmpty;
        }
    }
}
