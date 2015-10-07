using UnityEngine;
using System.Collections;

public interface IConsumable : IGameObject
{
    bool IsAvailableForConsumption(object sender);
    void Consumed();
}
