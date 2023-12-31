#version 440 core

in vec3 Normal;
out vec4 FragColor;

void main()
{
    vec3 norm = normalize(Normal);
    FragColor = vec4(norm * 30, 1.0f);
    // FragColor = vec4(0.1f);
    // FragColor = vec3(0.1f, 2.0f);
} 
