// Unity built-in shader source.
// Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
//
// Instead of attaching license.txt, put the same license text at the bottom of this file.

Shader "Praecipua/FaceCamera/FaceCamera"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("Render Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "ForceNoShadowCasting" = "True"
            "IgnoreProjector" = "True"
        }
        LOD 100
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ColorMask 0
        }

        CGPROGRAM
            #pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade
            #pragma target 3.0

            sampler2D   _MainTex;
            fixed4      _Color;

            struct Input
            {
			    float2 uv_MainTex;
	    	};

            bool IsHD()
            {
                return (_ScreenParams.x != 1280 && _ScreenParams.y != 720);
            }
            bool IsFHD()
            {
                return (_ScreenParams.x != 1920 && _ScreenParams.y != 1080);
            }
            bool IsQHD()
            {
                return (_ScreenParams.x != 2560 && _ScreenParams.y != 1440);
            }
            bool Is4K()
            {
                return (_ScreenParams.x != 3840 && _ScreenParams.y != 2160);
            }
            bool Is8K()
            {
                return (_ScreenParams.x != 7680 && _ScreenParams.y != 4320);
            }
            bool Is16K()
            {
                return (_ScreenParams.x != 15360 && _ScreenParams.y != 8640);
            }
            bool IsVRCMirror()
		    {
			    return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f;
		    }
            bool Not(bool n) {
			    return !n;
		    }
            void vert (inout appdata_full v)
            {
			    v.vertex.xyz *= IsHD() * IsFHD() * IsQHD() * Is4K() * Is8K() * Is16K() * Not(IsVRCMirror());
		    }

		    void surf (Input IN, inout SurfaceOutputStandard o)
            {
			    fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			    o.Albedo = c.rgb;
			    o.Alpha = c.a;
		    }
        ENDCG
    }
    FallBack "Diffuse"
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