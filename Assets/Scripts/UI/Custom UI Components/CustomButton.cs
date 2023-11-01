using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CustomUI
{
    public class CustomButton : CustomUIComponent
    {

        [SerializeField] string textValue;
        [SerializeField] CustomButtonDefinition definition;
        [SerializeField] Button button;
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] UIStyle style;
        [SerializeField] UnityEvent onClick;
        
        public override void Setup()
        {

        }

        public override void Configure()
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = definition.theme.GetBackgroundColor(style);
            button.colors = colorBlock;

            text.color = definition.theme.GetTextColor(style);
            text.text = textValue;
        }

        public void OnClick()
        {
            onClick.Invoke();
        }

    }
}