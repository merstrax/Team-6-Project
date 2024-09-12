using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    void TakeDamage(float amount);

    void TakeDamage(float amount, Vector3 loc, Quaternion rotation);
}
