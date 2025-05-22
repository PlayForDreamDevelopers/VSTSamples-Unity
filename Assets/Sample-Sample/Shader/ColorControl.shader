Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color("Main Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" "Queue"="Geometry+1"}
        Cull Off
        ZTest Always
        ZWrite On
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 顶点变换
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; // 直接输出颜色，无光照计算
            }
            ENDCG
        }
    }
}
