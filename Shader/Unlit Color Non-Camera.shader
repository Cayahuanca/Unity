// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Praecipua/Unlit/Color Non-Camera" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
}

SubShader {
    Tags {
        "Queue"="Geometry" "RenderType"="Opaque"
    }
    LOD 100

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(0)
                UNITY_VERTEX_OUTPUT_STEREO
            };

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
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                UNITY_APPLY_FOG(i.fogCoord, col);
                UNITY_OPAQUE_ALPHA(col.a);
                return col;
            }
        ENDCG
    }
}

}
