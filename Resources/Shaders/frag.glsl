#version 440 core

layout(location = 4) out vec3 id_color;

// in uint id;
in vec3 Normal;
in vec3 FragPos;
out vec4 FragColor;

void main()
{
    // id_color = vec3(1,0,0);
    vec3 norm = normalize(Normal);
    FragColor = vec4(norm, 1.0f);
} 
