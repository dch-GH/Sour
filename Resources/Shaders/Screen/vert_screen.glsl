#version 440 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aTexCoords;

out vec3 TexCoords;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0); 
    TexCoords = aTexCoords;
} 