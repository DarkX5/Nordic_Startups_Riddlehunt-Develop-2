using TMPro;
using UnityEngine;
using Zenject;

public class StartupPanelBehavior : MonoBehaviour
{
    [SerializeField] private ProductListBehaviour productListView;
    [Inject] private LoginHandler _loginHandler;

    // Start is called before the first frame update
    void Start()
    {
        _loginHandler.ServiceUpdated += LoggedInAction;
    }

    public void Complete()
    {
        this.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        _loginHandler.ServiceUpdated -= LoggedInAction;
    }

    public void LoggedInAction(LoginEvents loginEvent)
    {
        Debug.Log("login fired");
        switch (loginEvent)
        {
            case LoginEvents.loggedInAsGuest:
            case LoginEvents.loggedInAsUser:
                productListView.Configure(() =>
                {
                    this.gameObject.SetActive(false);
                });
                break;
        }
    }
}
