using UnityEditor;
using UnityEngine;

namespace AutoAnimaker.Editor
{
    public class SpritePreviewPopup : EditorWindow
    {
        private Sprite sprite;

        private float windowWidth = 600;
        private float windowHeight = 600;

        private float maxLength;
        private float spriteWidth, spriteHeight;

        private int rowCount;
        private int columnCount;

        /** Properties **/
        public Sprite PreviewSprite { get => sprite; set => sprite = value; }
        public int RowCount { get => rowCount; set => rowCount = value; }
        public int ColumnCount { get => columnCount; set => columnCount = value; }

        void OnGUI()
        {
            if (sprite != null)
            {
                Handles.color = Color.red;
                Rect previewRect = GUILayoutUtility.GetRect(windowWidth, windowHeight);
                EditorGUI.DrawTextureTransparent(previewRect, sprite.texture, ScaleMode.ScaleToFit);

                maxLength = Mathf.Max(sprite.texture.width, sprite.texture.height);
                spriteWidth = windowWidth * sprite.texture.width / maxLength;
                spriteHeight = windowHeight * sprite.texture.height / maxLength;

                for (int i = 1; i < rowCount; i++)
                {
                    Handles.DrawLine(new Vector2((windowWidth - spriteWidth) / 2, (windowWidth - spriteHeight) / 2 + spriteHeight / rowCount * i),
                        new Vector2((windowWidth + spriteWidth) / 2, (windowWidth - spriteHeight) / 2 + spriteHeight / rowCount * i));
                }
                for (int i = 1; i < columnCount; i++)
                {
                    Handles.DrawLine(new Vector2((windowWidth - spriteWidth) / 2 + spriteWidth / columnCount * i, (windowHeight - spriteHeight) / 2),
                        new Vector2((windowWidth - spriteWidth) / 2 + spriteWidth / columnCount * i, (windowHeight + spriteHeight) / 2));
                }
            }
            else
            {
                Close();
            }
        }

        private void OnLostFocus()
        {
            Close();
        }
    }
}