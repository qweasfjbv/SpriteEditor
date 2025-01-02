using TMPro;
using UnityEngine;

namespace AutoAnimaker.Demo
{
    public class ExampleConverter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI upperText;
        [SerializeField] private TextMeshProUGUI lowerText;

        private Vector3 enemyPos = new Vector3(0, 0, -1);
        private Vector3 knightPos = new Vector3(4, 0, -1);

        private int showIdx = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                showIdx = showIdx + 1;
                if (showIdx >= 2) showIdx = 0;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                showIdx = showIdx - 1;
                if (showIdx < 0) showIdx = 1;
            }

            ChangeShowcase(showIdx);
        }

        private int prevIdx = 0;
        private void ChangeShowcase(int idx)
        {
            if (prevIdx == idx) return;
            prevIdx = idx;

            switch (idx) {
                case 0:
                    transform.position = enemyPos;
                    upperText.text = "override example";
                    lowerText.text = "press 1-3";
                    break;
                case 1:
                    transform.position = knightPos;
                    upperText.text = "override example";
                    lowerText.text = "press 1-6";
                    break;
            }
        }

    }
}