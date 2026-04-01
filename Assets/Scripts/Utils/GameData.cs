using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public enum Employee
    {
        Boovs,
        Jor,
        Oswald,
        Puirra,
        Zanfael
    }
    
    public enum Shape
    {
        Square1,
        Line,
        ZShape,
        Cross3
    }
    
    public enum ItemType
    {
        Default,
        Armor,
        Accessory,
        Helmet,
        Boots,
        Consumable,
        MeleeWeapon,
        RangeWeapon,
        Skill
    }

    
    public enum Direction
    {
        Down,
        Left,
        Up,
        Right
    }
    
    public class ShapeData
    {
        public List<Vector2Int> GridPositions0;
        public List<Vector2Int> GridPositions90;
        public List<Vector2Int> GridPositions180;
        public List<Vector2Int> GridPositions270;
    }
    
    public static class Shapes
    {
        private static Dictionary<Shape, ShapeData> _shapeMap;
    
        private static void Initialize()
        {
            if (_shapeMap != null) return;
    
            _shapeMap = new Dictionary<Shape, ShapeData>
            {
                {Shape.Square1, Square1},
                {Shape.Line, Line},
                {Shape.ZShape, ZShape},
                {Shape.Cross3, Cross3}
            };
        }
    
        public static ShapeData GetShapeData(Shape shape)
        {
            Initialize();
            if (_shapeMap.TryGetValue(shape, out var shapeData)) return shapeData;
            Debug.LogError($"[ShapePresets] No shape data defined for shape: {shape}");
            return null;
        }
    
        private static readonly ShapeData Square1 = new()
        {
            GridPositions0 = new List<Vector2Int> {new(0, 0)},
            GridPositions90 = new List<Vector2Int> {new(0, 0)},
            GridPositions180 = new List<Vector2Int> {new(0, 0)},
            GridPositions270 = new List<Vector2Int> {new(0, 0)}
        };
    
        private static readonly ShapeData Line = new()
        {
            GridPositions0 = new List<Vector2Int> {new(0, 0), new(1, 0), new(2, 0), new(3, 0)},
            GridPositions90 = new List<Vector2Int> {new(0, 1), new(0, 2), new(0, 3), new(0, 0)},
            GridPositions180 = new List<Vector2Int> {new(0, 0), new(1, 0), new(2, 0), new(3, 0)},
            GridPositions270 = new List<Vector2Int> {new(0, 1), new(0, 2), new(0, 3), new(0, 0)},
        };
    
        private static readonly ShapeData ZShape = new()
        {
            GridPositions0 = new List<Vector2Int> {new(0, 1), new(1, 1), new(1, 0), new(2, 0)},
            GridPositions90 = new List<Vector2Int> {new(0, 0), new(0, 1), new(1, 1), new(1, 2)},
            GridPositions180 = new List<Vector2Int> {new(0, 1), new(1, 1), new(1, 0), new(2, 0)},
            GridPositions270 = new List<Vector2Int> {new(0, 0), new(0, 1), new(1, 1), new(1, 2)},
        };
    
        private static readonly ShapeData Cross3 = new()
        {
            GridPositions0 = new List<Vector2Int> {new(0, 1), new(1, 1), new(1, 0), new(2, 1), new(1, 2)},
            GridPositions90 = new List<Vector2Int> {new(0, 1), new(1, 1), new(1, 0), new(2, 1), new(1, 2)},
            GridPositions180 = new List<Vector2Int> {new(0, 1), new(1, 1), new(1, 0), new(2, 1), new(1, 2)},
            GridPositions270 = new List<Vector2Int> {new(0, 1), new(1, 1), new(1, 0), new(2, 1), new(1, 2)},
        };
    }
    
    
    public static class DirectionUtilities
    {
        public static int ToDegrees(Direction direction) => direction switch
        {
            Direction.Left => 90,
            Direction.Up => 180,
            Direction.Right => 270,
            _ => 0
        };
    
        public static Direction Next(Direction direction) => direction switch
        {
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.Up => Direction.Right,
            _ => Direction.Down
        };
    }

    public static class Colors
    {
        public static Color32 DefaultCellBackgroundColor = new Color32(255, 255, 255, 255);
        public static Color32 AvailableCellBackgroundColor = new(46, 204, 113, 100);
        public static Color32 UnavailableCellBackgroundColor = new(231, 76, 60, 100);
    }

    public static class SceneNames
    {
        public const string CraftScene = "CraftScene";
        public const string ClientScene = "CounterScene";
        public const string LivingroomScene = "LivingroomScene";
        public const string QuestCardScene = "QuestCardScene";
    }

    public class CharacterPresence
    {
        public Employee CharacterName { get; }
        public bool IsVisible { get; set; }
        public bool IsIndicatorVisible { get; set; }

        public CharacterPresence(Employee characterName, bool isVisible, bool isIndicatorVisible)
        {
            CharacterName = characterName;
            IsVisible = isVisible;
            IsIndicatorVisible = isIndicatorVisible;
        }
    }
}