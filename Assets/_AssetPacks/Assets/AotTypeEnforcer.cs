using Newtonsoft.Json.Utilities;
using riddlehouse_libraries.login;
using UnityEngine;
 
/*
 * This script does not need to be added to a GameObject. It just needs to be compiled. Inheriting from UnityEngine.MonoBehaviour or UnityEngine.ScriptableObject will ensure to always be compiled.
 */
public class AotTypeEnforcer : MonoBehaviour
{
    public void Awake()
    {
        AotHelper.EnsureType<UserInfoResponse>();
    }
}