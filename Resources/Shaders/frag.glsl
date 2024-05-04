#version 440 core

layout(location = 0) out vec4 out_Color;
layout(location = 1) out vec4 out_ObjectId;

in vec3 Normal;
in vec3 FragPos;

void main()
{
    vec3 norm = normalize(Normal);
    out_ObjectId = vec4(1.0f, 0.0f, 0.0f, 1.0f);
    out_Color = vec4(norm, 1.0f);
} 
