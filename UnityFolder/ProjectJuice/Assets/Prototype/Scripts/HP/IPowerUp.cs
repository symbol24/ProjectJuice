using UnityEngine;
using System.Collections;

public interface IPowerUp : IConsumable
{
    float HPRecov { get; }
}
