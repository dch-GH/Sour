#version 440 core

out vec4 FragColor;

uniform float time;
uniform vec4 color;

uniform vec2  resolution;
uniform float dashSize;
uniform float gapSize;

// dash lines from https://stackoverflow.com/questions/52928678/dashed-line-in-opengl3
flat in vec3 startPos;
in vec3 vertPos;

void main()
{
    vec2  dir  = (vertPos.xy-startPos.xy) * resolution/2.0;
    float dist = length(dir);

    if (fract(dist / (dashSize + gapSize)) > dashSize/(dashSize + gapSize))
        discard; 

    FragColor = color;
} 
