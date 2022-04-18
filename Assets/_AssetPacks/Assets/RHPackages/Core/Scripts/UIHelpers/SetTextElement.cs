using TMPro;
using UnityEngine;

namespace RHPackages.Core.Scripts.UIHelpers
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SetTextElement : MonoBehaviour
    {
        public TextMeshProUGUI TextField { get; private set; }
        public void Configure(string text)
        {
            TextField ??= GetComponent<TextMeshProUGUI>();
            TextField.text = text;
        }
    }
}
