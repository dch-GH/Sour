#version 440 core

in vec3 UV;
out vec3 FragColor;

uniform sampler2D renderTexture;

void main()
{
    FragColor = texture(renderTexture, UV.xy).xyz;
    // FragColor = vec3(0,0,1);
} 
