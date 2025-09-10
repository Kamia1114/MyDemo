using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.Grid
{
    public class GridUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI cityText;

        public void SetCityName(string cityName)
        {
            cityText.text = cityName;
        }
    }
}