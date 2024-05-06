#version 440 core

layout(location = 0) out vec4 out_Color;
layout(location = 1) out vec4 out_ObjectId;

in vec3 Normal;
in vec3 FragPos;
in vec4 ObjectId;

// vec4 hexToRgb(int hexColor) {
//     float red = float((hexColor >> 16) & 0xFF);
//     float green = float((hexColor >> 8) & 0xFF);
//     float blue = float(hexColor & 0xFF);

//     return vec4(red, green, blue, 1.0f);
// }

void main()
{
    vec3 norm = normalize(Normal);
    out_ObjectId = ObjectId;
    out_Color = vec4(norm, 1.0f);
} 
