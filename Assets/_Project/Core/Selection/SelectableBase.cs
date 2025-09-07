using UnityEngine;

namespace FestivalGrounds.Core {
        
    public abstract class SelectableBase : MonoBehaviour, ISelectable
    {
        [Header("Optional: assign a child/prefab to toggle on selection")]
        [SerializeField] private GameObject highlightObject; // e.g., a ring under the sim

        public abstract string DisplayName { get; }
        public abstract object GetDetailViewModel();

        public virtual void OnSelected()
        {
            if (highlightObject != null) highlightObject.SetActive(true);
        }

        public virtual void OnDeselected()
        {
            if (highlightObject != null) highlightObject.SetActive(false);
        }

        protected virtual void OnDisable()
        {
            if (SelectionService.Instance != null && ReferenceEquals(SelectionService.Instance.Current, this))
                SelectionService.Instance.ClearSelection();
        }

    }

}  