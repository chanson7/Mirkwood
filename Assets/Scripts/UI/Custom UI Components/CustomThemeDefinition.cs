using UnityEngine;

namespace CustomUI
{

    [CreateAssetMenu(menuName = "CustomUI/ThemeDefinition", fileName = "Theme")]
    public class CustomThemeDefinition : ScriptableObject
    {
        [Header("Primary Colors")]
        public Color primaryBackgroundColor;
        public Color primaryTextColor;

        [Header("Secondary Colors")]
        public Color secondaryBackgroundColor;
        public Color secondaryTextColor;

        [Header("Tertiary Colors")]
        public Color tertiaryBackgroundColor;
        public Color tertiaryTextColor;

        [Header("Other Colors")]
        public Color disabledColor;

        public Color GetBackgroundColor(UIStyle style)
        {
            switch(style)
            {
                case UIStyle.Primary:
                    return primaryBackgroundColor;
                case UIStyle.Secondary: 
                    return secondaryBackgroundColor;
                case UIStyle.Tertiary: 
                    return tertiaryBackgroundColor;
                default: 
                    return disabledColor;
            }
        }

        public Color GetTextColor(UIStyle style)
        {
            switch (style)
            {
                case UIStyle.Primary:
                    return primaryBackgroundColor;
                case UIStyle.Secondary:
                    return secondaryBackgroundColor;
                case UIStyle.Tertiary:
                    return tertiaryBackgroundColor;
                default:
                    return disabledColor;
            }
        }
    }

    public enum UIStyle
    {
        Primary, 
        Secondary, 
        Tertiary
    }
}
