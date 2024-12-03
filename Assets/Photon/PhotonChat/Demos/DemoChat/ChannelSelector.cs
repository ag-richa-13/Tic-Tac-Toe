using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
    public class ChannelSelector : MonoBehaviour, IPointerClickHandler
    {
        public string Channel;

        public void SetChannel(string channel)
        {
            this.Channel = channel;
            Text t = this.GetComponentInChildren<Text>();
            t.text = this.Channel;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Use FindFirstObjectByType instead of FindObjectOfType
            ChatGui handler = Object.FindFirstObjectByType<ChatGui>();
            handler.ShowChannel(this.Channel);
        }
    }
}
