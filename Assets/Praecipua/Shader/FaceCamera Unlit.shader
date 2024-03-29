// Unity built-in shader source.
// Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
//
// Instead of attaching license.txt, put the same license text at the bottom of this file.

Shader "Praecipua/FaceCamera/FaceCamera Unlit"
{
    Properties
    {
        _MainTex("Render Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "ForceNoShadowCasting" = "True"
        }
        LOD 100

        Zwrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ColorMask 0
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D   _MainTex;
            float4      _MainTex_ST;
            float       _Alpha;

            float _VRChatCameraMode;
            float _VRChatMirrorMode;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            uint isPlayerSelf()
            {
                float4 output = tex2D(_MainTex, float2(0.5, 0.5));
                // 0.5, 0.5 は、テクスチャの中心を表す。
                return output.a == 0 ? 0 : 1;
                // テクスチャの中心のアルファ値が 0 なら 0 を返す。そうでなければ 1 を返す。
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if(_VRChatCameraMode !=0 || _VRChatMirrorMode != 0)
                {
                    discard;
                }

                if(isPlayerSelf() == 0)
                {
                    discard;
                }

                fixed4 output = tex2D(_MainTex, i.texcoord);
                output.a = output.a * _Alpha;
                return output;
            }

            ENDCG
        }
    }
}


// MIT License
// Copyright (c) 2016 Unity Technologies
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.