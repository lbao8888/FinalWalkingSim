using UnityEngine;

public class CarInteractable : Interactable
{
    public TestCar car;

    public override void Interact(CCPlayer player)
    {
        if (car != null && !car.isDriving)
        {
            car.EnterCar(player);
        }
    }
}