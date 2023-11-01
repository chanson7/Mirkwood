using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField] private Page initialPage;
    [SerializeField] private GameObject FirstFocusItem;

    private Canvas canvas;
    private Stack<Page> pageStack = new();

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        if (FirstFocusItem != null)
        {
            EventSystem.current.SetSelectedGameObject(FirstFocusItem);
        }

        if (initialPage != null)
        {
            PushPage(initialPage);
        }
    }

    private void OnCancel()
    {
        if (canvas.enabled && canvas.gameObject.activeInHierarchy)
        {
            if (pageStack.Count != 0)
            {
                PopPage();
            }
        }
    }

    public bool IsPageInStack(Page Page)
    {
        return pageStack.Contains(Page);
    }

    public bool IsPageOnTopOfStack(Page Page)
    {
        return pageStack.Count > 0 && Page == pageStack.Peek();
    }

    public void PushPage(Page Page)
    {
        Page.Open();

        if (pageStack.Count > 0)
        {
            Page currentPage = pageStack.Peek();

            if (currentPage.CloseOnNewPagePush)
            {
                currentPage.Close();
            }
        }

        pageStack.Push(Page);
    }

    public void PopPage()
    {
        if (pageStack.Count > 1)
        {
            Page page = pageStack.Pop();
            page.Close();

            Page newTopPage = pageStack.Peek();
            if (newTopPage.CloseOnNewPagePush)
            {
                newTopPage.Open();
            }
        }
        else
        {
            Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
        }
    }

    public void PopAllPages()
    {
        for (int i = 1; i < pageStack.Count; i++)
        {
            PopPage();
        }
    }
}
