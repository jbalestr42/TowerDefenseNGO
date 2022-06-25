using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalClientProviderHelper : MonoBehaviour
{
    [SerializeField]
    PlayerPrefProvider _clientProvider;

    [SerializeField]
    UnityEngine.UI.Toggle _isOnSiteToggle;

    public void OnClientRoleSelected()
    {
        _clientProvider.SetValue("lbe", _isOnSiteToggle.isOn.ToString());
    }
}
