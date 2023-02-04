// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Praecipua/Unlit/Transparent Color Non-Camera" {
Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
    Tags
    {
        "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            bool IsHD()
            {
                return (_ScreenParams.x == 1280 && _ScreenParams.y == 720);
            }
            bool IsFHD()
            {
                return (_ScreenParams.x == 1920 && _ScreenParams.y == 1080);
            }
            bool IsQHD()
            {
                return (_ScreenParams.x == 2560 && _ScreenParams.y == 1440);
            }
            bool Is4K()
            {
                return (_ScreenParams.x == 3840 && _ScreenParams.y == 2160);
            }
            bool Is8K()
            {
                return (_ScreenParams.x == 7680 && _ScreenParams.y == 4320);
            }
            bool Is16K()
            {
                return (_ScreenParams.x == 15360 && _ScreenParams.y == 8640);
            }

            bool IsVRCMirror()
            {
                return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f;
            }
            bool Not(bool n) {
			    return !n;
		    }

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.xyz *= Not(IsFHD()) * Not(IsQHD()) * Not(Is4K())* Not(Is8K()) * Not(Is16K()) * Not(IsVRCMirror());
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}
