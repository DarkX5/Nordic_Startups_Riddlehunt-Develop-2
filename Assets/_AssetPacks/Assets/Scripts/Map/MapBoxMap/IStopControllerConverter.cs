using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStopControllerConverter<ITargetType>
{
    public ITargetType ConvertSelf(IStopController controller);
}