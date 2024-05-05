#version 440 core

layout(location = 0) out vec4 out_Color;
layout(location = 1) out vec4 out_ObjectId;

in vec3 Normal;
in vec3 FragPos;
in float ObjectId;

vec4 hexToRgba(int hexColor) {
    float red = float((hexColor >> 24) & 0xFF) / 255.0;
    float green = float((hexColor >> 16) & 0xFF) / 255.0;
    float blue = float((hexColor >> 8) & 0xFF) / 255.0;
    float alpha = float(hexColor & 0xFF) / 255.0;

    return vec4(red, green, blue, alpha);
}

void main()
{
    vec3 norm = normalize(Normal);
    int fuck = int(ObjectId);
    out_ObjectId = hexToRgba(fuck);
    out_Color = vec4(norm, 1.0f);
} 
