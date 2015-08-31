using UnityEngine;
using System.Collections;

public interface IDamaging : IConsumable
{
    float Damage { get; }
}
