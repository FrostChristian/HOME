Shader "Custom/StarBackroundPlanetSelect"{
	Properties{
		_MainTex("Stars Texture (RGB)", 2D) = "black" {}
		_StarsColor("Stars Color (RGB)", COLOR) = (1.0, 1.0, 1.0, 1.0)
		_StarsIntensity("Stars Intensity", FLOAT) = 1.0
	}
		SubShader{
			Tags { "Queue" = "Background" "RenderType" = "Background" }
			Cull Back
			ZWrite Off
			Fog { Mode Off }

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				uniform sampler2D _MainTex;
				uniform fixed4 _StarColor;
				uniform half _StarIntensity;

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
					return texMain * _StarColor * _StarIntensity;
				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}
