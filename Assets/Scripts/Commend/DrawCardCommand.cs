using UnityEngine;

public class DrawCardCommand : ICommand
{
    public void Execute()
    {
        Debug.Log("Draw a card");
    }
    public void Undo()
    {
        Debug.Log("Undo draw a card");
    }
}
