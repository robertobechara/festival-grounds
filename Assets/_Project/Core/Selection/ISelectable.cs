namespace FestivalGrounds.Core {
    public interface ISelectable
    {
        string DisplayName { get; }                 // e.g., "Sim #42"
        object GetDetailViewModel();                // any data object your UI understands
        void OnSelected();
        void OnDeselected();
    }

}