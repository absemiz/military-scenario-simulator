using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatePredicators
{
    delegate bool TransformPredicator(Transform transform);
    delegate bool RigidbodyPredicator(Rigidbody rigidbody);
}

