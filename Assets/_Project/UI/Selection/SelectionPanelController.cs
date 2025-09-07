using FestivalGrounds.Core;
using FestivalGrounds.Sims;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FestivalGrounds.UI {
    public class SelectionPanelController : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject panelRoot;        // the whole panel
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Sim Needs")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider bladderSlider;
        [SerializeField] private Slider sleepSlider;
        [SerializeField] private Slider funSlider;

        private void Awake()
        {
            if (panelRoot == null)
                Debug.LogWarning("[SelectionPanel] panelRoot not assigned — assign the 'Content' child.");
            else
                panelRoot.SetActive(false); // hide just the content
        }

        private void OnEnable()
        {
            if (SelectionService.Instance != null)
            {
                SelectionService.Instance.OnSelectionChanged += HandleSelectionChanged;
                Debug.Log("[SelectionPanel] Subscribed to SelectionService");
            }
            else
            {
                Debug.LogError("[SelectionPanel] No SelectionService.Instance found");
            }
        }

        private void OnDisable()
        {
            if (SelectionService.Instance != null)
                SelectionService.Instance.OnSelectionChanged -= HandleSelectionChanged;
        }

        private void HandleSelectionChanged(ISelectable selected)
        {
            Debug.Log($"[SelectionPanel] HandleSelectionChanged: {(selected!=null?selected.DisplayName:"null")}");
            if (selected == null) { panelRoot.SetActive(false); return; }

            // Basic header
            titleText.text = selected.DisplayName;

            // Try to pull a SimViewModel
            var vmObj = selected.GetDetailViewModel();
            Debug.Log($"[SelectionPanel] Got selection: {selected.DisplayName} ({selected.GetType().Name}), VM: {(vmObj != null ? vmObj.GetType().Name : "null")}");

            if (vmObj is SimViewModel sim)
            {
                // ensure sliders assigned
                if (hungerSlider == null || bladderSlider == null || sleepSlider == null || funSlider == null)
                {
                    Debug.LogError("[SelectionPanel] One or more Slider references are not assigned in the inspector.");
                    panelRoot.SetActive(false);
                    return;
                }

                hungerSlider.value = Mathf.Clamp01(sim.Hunger);
                bladderSlider.value = Mathf.Clamp01(sim.Bladder);
                sleepSlider.value   = Mathf.Clamp01(sim.Sleep);
                funSlider.value     = Mathf.Clamp01(sim.Fun);

                panelRoot.SetActive(true);
                return;
            }

            // Not a Sim? (In this version we only show Sims)
            Debug.Log("[SelectionPanel] Selected item is not a SimViewModel; hiding panel.");
            panelRoot.SetActive(false);
        }

        
    }
}