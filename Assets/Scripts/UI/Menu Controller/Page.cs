using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class Page : MonoBehaviour
{
    #region EDITOR EXPOSED FIELDS

    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip closeClip;
    [SerializeField] UnityEvent prePushAction;
    [SerializeField] UnityEvent prePopAction;
    [SerializeField] Animator animator;

    #endregion

    #region PROPERTIES

    public bool CloseOnNewPagePush = true;
    
    #endregion

    public void Open()
    {
        gameObject.SetActive(true);
        prePushAction?.Invoke();
    }

    public void Close()
    {
		prePopAction?.Invoke();
        gameObject.SetActive(false);
    }

}
