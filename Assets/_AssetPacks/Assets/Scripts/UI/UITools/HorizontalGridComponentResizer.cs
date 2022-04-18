using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.UITools
{
    public interface IHorizontalGridComponentResizer
    {
        public void ResizeGrid(int elementCount, float maxSize);
        public bool CanResize();
    }
    [RequireComponent(typeof(GridLayoutGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalGridComponentResizer : MonoBehaviour, IHorizontalGridComponentResizer
    {
        public class Dependencies
        {
            public RectTransform GridTransform { get; set; }
            public GridLayoutGroup GridLayoutGroup { get; set; }
        }

        public void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                GridTransform = GetComponent<RectTransform>(),
                GridLayoutGroup = GetComponent<GridLayoutGroup>()
            });
        }

        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public void ResizeGrid(int elementCount, float maxSize)
        {
            var gridLayoutGroup = _dependencies.GridLayoutGroup;
            var horizontalSpacing = gridLayoutGroup.spacing.x;
            var totalPadding = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
            var width = _dependencies.GridTransform.rect.width;
            var gridElementSize = (width- CalculateSpacing(elementCount, horizontalSpacing, totalPadding))/elementCount;
            gridElementSize = Mathf.Clamp(gridElementSize, 0, maxSize);
            _dependencies.GridLayoutGroup.cellSize = new Vector2(gridElementSize, gridElementSize);
        }

        private float CalculateSpacing(int elementCount, float horizontalSpacing, float totalPadding)
        {
            return ((elementCount - 1) * horizontalSpacing) + totalPadding;
        }

        public bool CanResize()
        {
            return this.gameObject.activeSelf && (_dependencies.GridTransform.rect.width != 0);
        }
    }
}
