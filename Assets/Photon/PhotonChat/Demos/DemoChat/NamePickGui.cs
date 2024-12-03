using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
    [RequireComponent(typeof(ChatGui))]
    public class NamePickGui : MonoBehaviour
    {
        private const string UserNamePlayerPref = "NamePickUserName";

        public ChatGui chatNewComponent;

        public InputField idInput;

        public void Start()
        {
            // Use FindFirstObjectByType instead of FindObjectOfType
            this.chatNewComponent = Object.FindFirstObjectByType<ChatGui>();

            string prefsName = PlayerPrefs.GetString(UserNamePlayerPref);
            if (!string.IsNullOrEmpty(prefsName))
            {
                this.idInput.text = prefsName;
            }
        }

        // new UI will fire "EndEdit" event also when losing focus. So check "enter" key and only then StartChat.
        public void EndEditOnEnter()
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                this.StartChat();
            }
        }

        public void StartChat()
        {
            // Use FindFirstObjectByType instead of FindObjectOfType
            ChatGui chatNewComponent = Object.FindFirstObjectByType<ChatGui>();
            chatNewComponent.UserName = this.idInput.text.Trim();
            chatNewComponent.Connect();
            this.enabled = false;

            PlayerPrefs.SetString(UserNamePlayerPref, chatNewComponent.UserName);
        }
    }
}
