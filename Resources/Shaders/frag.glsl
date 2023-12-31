#version 440 core

uniform vec3 lightPos;

in vec3 Normal;
in vec3 FragPos;
out vec4 FragColor;

void main()
{
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * vec3(1);
    vec3 result = (diffuse);
    FragColor = vec4(result, 1.0f);
} 
