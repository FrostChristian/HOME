Shader "Custom/StarsBackroundPlanetSelect"{
	Properties{
		_MainTex("Stars Texture (RGB)", 2D) = "black" {}
	}
		SubShader{
			Tags {"Queue" = "Background" "IgnoreProjector" = "True"}
			ZWrite Off

			Pass {

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				uniform sampler2D _MainTex;

				struct vertexInput {
					half4 vertex : POSITION;
					fixed4 texcoord : TEXCOORD0;
				};

				struct vertexOutput {
					half4 pos : SV_POSITION;
					fixed4 tex : TEXCOORD0;
				};

				vertexOutput vert(vertexInput v) {
					vertexOutput o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.tex = v.texcoord;
					return o;
				}

				half4 frag(vertexOutput i) : COLOR {
					half4 texMain = tex2D(_MainTex, i.tex.xy);
					return texMain;
				}

				ENDCG
			}
	}
		FallBack "Diffuse"
}
