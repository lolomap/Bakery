using JetBrains.Annotations;
using UnityEngine;
using Utils;
using Zenject;

namespace Grid
{
    public static class GridItemFactory
    {
        public static GridItemBase Create(Transform parent, Vector2 anchoredPosition, Vector2Int origin,
            Direction direction, GridItemBaseSO itemSO, GridItemBase item = null)
        {
            var sceneContext = GameObject.FindAnyObjectByType<SceneContext>();
            var transform =
                sceneContext.Container.InstantiatePrefab(itemSO.prefab.GetComponent<RectTransform>(), parent)
                    .GetComponent<RectTransform>();
            //var transform = GameObject.Instantiate(itemSO.prefab.GetComponent<RectTransform>(), parent);
            transform.rotation = Quaternion.Euler(0, DirectionUtilities.ToDegrees(direction), 0);
            transform.anchoredPosition = anchoredPosition;

            GridItemBase created = transform.GetComponent<GridItemBase>();
            created.Initialize(origin, direction, itemSO, item);
            return created;
        }
    }
}