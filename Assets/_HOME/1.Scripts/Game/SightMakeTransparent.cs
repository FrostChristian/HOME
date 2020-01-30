using UnityEngine;

namespace HOME.Game {

    public class SightMakeTransparent : MonoBehaviour {

        // handels the ship material to fade out when camera comes near

        [SerializeField] private Material[] oldMaterials = null;
        private float m_Transparency = 1.0f;
        public float TargetTransparency { get; set; }
        public float FadeInTimeout { get; set; }
        public float FadeOutTimeout { get; set; }
        public Material TransparentMaterial { get; set; }
        public Shader TransparentShader { get; set; }
        private bool shouldBeTransparent = true;

        public void BeTransparent() {
            shouldBeTransparent = true;
        }

        private void Start() {
            m_Transparency = 1.0f; // reset the transparency;
            if (oldMaterials == null) {  // Save the current materials  
                oldMaterials = GetComponent<Renderer>().materials;
                Material[] materialsList = new Material[oldMaterials.Length];
                for (int i = 0; i < materialsList.Length; i++) {  // repalce material with transparent
                    materialsList[i] = Object.Instantiate(TransparentMaterial);
                    materialsList[i].SetColor("_Color", oldMaterials[i].GetColor("_Color"));
                }
                GetComponent<Renderer>().materials = materialsList;  // make transparent
            }
        }
        // Update is called once per frame
        private void Update() {
            if (!shouldBeTransparent && m_Transparency >= 1.0) {
                Destroy(this);
            }

            if (shouldBeTransparent) { //Are we fading in our out?
                                       //Fading out
                if (m_Transparency > TargetTransparency) {
                    m_Transparency -= ((1.0f - TargetTransparency) * Time.deltaTime) / FadeOutTimeout;
                }
            } else {
                //Fading in
                m_Transparency += ((1.0f - TargetTransparency) * Time.deltaTime) / FadeInTimeout;
            }

            Material[] materialsList = GetComponent<Renderer>().materials;
            for (int i = 0; i < materialsList.Length; i++) {
                Color C = oldMaterials[i].GetColor("_Color");

                C.a = m_Transparency;
                materialsList[i].color = C;
            }
            shouldBeTransparent = false; //The object will start to become visible again if BeTransparent() is not called
        }
        private void OnDestroy() { // restore old materials
            GetComponent<Renderer>().materials = oldMaterials;
        }
    }
}
