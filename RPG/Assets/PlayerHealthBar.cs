using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Core.UI
{
    [RequireComponent(typeof(RawImage))]
    public class PlayerHealthBar : MonoBehaviour
    {
        RawImage healthBarRawImage;
        Player player;
        // Start is called before the first frame update
        void Start()
        {
            player = FindObjectOfType<Player>();
            healthBarRawImage = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {
            float xValue = -(player.healthAsPercentage / 2f) - 0.5f;
            healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }
    }
}

