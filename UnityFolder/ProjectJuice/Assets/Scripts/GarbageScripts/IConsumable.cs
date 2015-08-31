using UnityEngine;
using System.Collections;

public interface IConsumable : IGameObject
{
    bool IsAvailableForConsumption { get; }
    void Consumed();
}
