namespace MinesServer.GameShit.GUI.Horb
{
    public readonly record struct Style(GridStyle Inventory, GridStyle Canvas, bool LargeInput, string FixScrollTag, bool DisableKeyboard, string InventoryButtonAction, float? ScrollHeight, float? Space) { }
}
