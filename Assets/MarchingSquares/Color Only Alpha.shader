Shader "2D/Color Only Alpha"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
    }

    Category
    {
        ZWrite Off
        Cull Off
        Tags {"Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        SubShader
        {
            Pass
            {
                Color [_Color]
            }
        }
    }
}