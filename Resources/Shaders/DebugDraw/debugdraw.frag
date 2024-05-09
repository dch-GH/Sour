#version 440 core

out vec4 FragColor;

uniform float time;

void main()
{
    FragColor = vec4(abs(sin(time * 4)), 0.25f, 0.25f, 1.0f);
} 
