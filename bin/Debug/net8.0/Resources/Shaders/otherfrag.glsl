#version 440 core

in vec3 Normal;
out vec4 FragColor;

void main()
{
    vec3 norm = normalize(Normal);
    FragColor = vec4(norm * 30, 1.0f);
} 
