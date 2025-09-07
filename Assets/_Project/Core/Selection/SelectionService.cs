using System;
using UnityEngine;

namespace FestivalGrounds.Core {
    [DefaultExecutionOrder(-100)]
    public class SelectionService : MonoBehaviour
    {
        public static SelectionService Instance { get; private set; }

        public event Action<ISelectable> OnSelectionChanged;

        public ISelectable Current { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject); // so UI scene/world scene can always find it
        }

        public void Select(ISelectable selectable)
        {
            if (Current == selectable) return;

            if (Current != null) Current.OnDeselected();
            Current = selectable;
            if (Current != null) Current.OnSelected();

            Debug.Log($"[SelectionService] OnSelectionChanged invoked with: {(Current != null ? Current.DisplayName : "null")}");
            OnSelectionChanged?.Invoke(Current);

        }

        public void ClearSelection()
        {
            if (Current == null) return;
            Current.OnDeselected();
            Current = null;
            OnSelectionChanged?.Invoke(null);
        }

    }
}